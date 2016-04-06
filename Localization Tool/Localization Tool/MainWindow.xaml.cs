using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Localization_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand AddRowRoutedCmd = new RoutedCommand();
        public static RoutedCommand ClearTableRoutedCmd = new RoutedCommand();
        public static RoutedCommand OpenJsonRoutedCmd = new RoutedCommand();
        public static RoutedCommand SaveAsJsonRoutedCmd = new RoutedCommand();

        public ObservableCollection<Translation> Translations { get; set; }
        public const string Url = "https://translate.yandex.net/api/v1.5/tr.json/translate?key=trnsl.1.1.20160322T103501Z.10b2b142f2f8bf7f.66c4f9f75232ede5cb9d8cc5ce17df5fd1d02d32&lang=";

        public MainWindow()
        {
            InitializeComponent();

            Translations = new ObservableCollection<Translation>();
            foreach (Lang value in Enum.GetValues(typeof(Lang)))
            {
                ReadJson(value);
            }
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Table.ItemsSource = Translations;
        }

        private void Add_Row(object sender, RoutedEventArgs e)
        {
            Translations.Add(new Translation());
        }

        private void Save_JSON(object sender, RoutedEventArgs e)
        {
            RemoveEmptyRows();
            foreach (Lang value in Enum.GetValues(typeof(Lang)))
            {
                File.WriteAllText(GetFileName(value), BuildJson(value));
            }
        }

        private void Open_JSON(object sender, RoutedEventArgs e)
        {
            Translations.Clear();
            foreach (Lang value in Enum.GetValues(typeof(Lang)))
            {
                ReadJson(value);
            }
            RemoveEmptyRows();
        }

        private void ReadJson(Lang language)
        {
            string fileName = GetFileName(language);
            try
            {
                var fileText = File.ReadAllText(fileName);
                JObject file = JObject.Parse(fileText);
                JObject translation = file.Value<JObject>(GetLang(language));
                foreach (var element in translation)
                {
                    int index = Contains(element.Key);
                    if (index == -1)
                    {
                        Translation line = new Translation(element.Key);
                        line[language] = element.Value.ToString();
                        Translations.Add(line);
                    }
                    else
                    {
                        Translations[index][language] = element.Value.ToString();
                    }
                }
            }
            catch (JsonReaderException jsonEx)
            {
                MessageBox.Show("Неверный формат json\n" + jsonEx.Message);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                MessageBox.Show("Файл отсутствует\n" + fileNotFoundException.Message + "\n" +
                                "При сохранении создастся автоматически");
            }
            catch (Exception ex) { MessageBox.Show("Неизвестная ошибка\n" + ex.Message); }
        }

        private string BuildJson(Lang language)
        {
            RemoveEmptyRows();
            var result = new StringBuilder();
            result.Append("{\"");
            result.Append(GetLang(language));
            result.Append("\":{");
            if (Translations.Count != 0)
            {
                for (var i = 0; i < Translations.Count - 1; i++)
                {
                    result.Append("\"" + Translations[i].Name + "\"");
                    result.Append(":");
                    result.Append("\"" + Translations[i][language] + "\"");
                    result.Append(",");
                }
                result.Append("\"" + Translations[Translations.Count - 1].Name + "\"");
                result.Append(":");
                result.Append("\"" + Translations[Translations.Count - 1][language] + "\"");
            }
            result.Append("}}");
            return result.ToString();
        }

        private void Clear_Table(object sender, RoutedEventArgs e)
        {
            Translations.Clear();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Translate(string sourceLanguage, string targetLanguage)
        {
            Lang sourceLanguageEnum = GetLang(sourceLanguage);
            Lang targetLanguageEnum = GetLang(targetLanguage); 
            sourceLanguage = sourceLanguage == "us" ? "en" : sourceLanguage;
            targetLanguage = targetLanguage == "us" ? "en" : targetLanguage;
            var webClient = new WebClient();
            var textToTranslate = new StringBuilder();
            foreach (var translation in Translations)
            {
                textToTranslate.Append("&text=");
                textToTranslate.Append(translation[sourceLanguageEnum]);
            }
            webClient.Encoding = Encoding.UTF8;
            var result = JObject.Parse(webClient.DownloadString(Url + sourceLanguage + "-" + targetLanguage + textToTranslate), new JsonLoadSettings() { });

            var i = 0;
            foreach (var item in result.GetValue("text"))
            {
                Translations[i][targetLanguageEnum] = item.ToString();
                i++;
            }
        }

        private void Translate_button(object sender, RoutedEventArgs e)
        {
            string sourceLanguage = SourceLanguage.Text;
            string targetLanguage = TargetLanguage.Text;
            Translate(sourceLanguage, targetLanguage);
        }

        private Lang GetLang(string lang)
        {
            switch (lang)
            {
                case "uk":
                case "UK":
                case "uk-UK":
                case "uk-UK.json":
                    return Lang.UK;
                case "us":
                case "US":
                case "en-Us":
                case "en-Us.json":
                    return Lang.US;
                case "ru":
                case "RU":
                case "ru-RU":
                case "ru-RU.json":
                    return Lang.RU;
                default:
                    throw new Exception("Incorrect Lang in Method GetLang");
            }
        }

        private string GetLang(Lang lang)
        {
            switch (lang)
            {
                case Lang.UK:
                    return "uk-UK";
                case Lang.US:
                    return "en-US";
                case Lang.RU:
                    return "ru-RU";
                default:
                    throw new Exception("Incorrect Lang in Method GetLang");
            }
        }
       
        private string GetFileName(Lang lang)
        {
            switch (lang)
            {
                case Lang.UK:
                    return "uk-UK.json";
                case Lang.US:
                    return "en-US.json";
                case Lang.RU:
                    return "ru-RU.json";
                default:
                    throw new Exception("Incorrect Lang in Method GetLang");
            }
        }
        
        private void LanguageChange(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            foreach (DataGridColumn column in Table.Columns)
            {
                if ((string)column.Header == cb.Name)
                {
                    column.Visibility = column.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
                }
            }
        }

        private int Contains(string key)
        {
            for (int i = 0; i < Translations.Count; i++)
            {
                if (Translations[i].Name == key)
                    return i;
            }
            return -1;
        }

        private void RemoveEmptyRows()
        {
            for (var i = 0; i < Translations.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(Translations[i].Name))
                    Translations.Remove(Translations[i--]);
            }
        }
    }
}
