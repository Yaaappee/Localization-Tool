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
            Translations = new ObservableCollection<Translation>
            {
                new Translation()
            };
            DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory());

            foreach (FileInfo file in d.GetFiles("*.json"))
            {
                MessageBox.Show(file.Name);
            }
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
            //_sf.ShowDialog();

            //if (_sf.FileName != "")
            //{
            //    File.WriteAllText(_sf.FileName, BuildJson());
            //    MessageBox.Show("Saved");
            //}
        }

        private void Open_JSON(object sender, RoutedEventArgs e)
        {
            //_of.ShowDialog();
            //if (_of.FileName != "")
            //{
            //    ReadJson(_of.FileName);
            //    MessageBox.Show("Opened");
            //}
        }

        private void ReadJson(Lang language)
        {
            string fileName = GetFileName(language);
            try
            {
                var fileText = File.ReadAllText(fileName);
                var o = JObject.Parse(fileText);
                Translations.Clear();
                foreach (var element in o)
                {
                    Translation line = new Translation(element.Key);
                    line[language] = element.Value.ToString();
                    Translations.Add(line);
                }
            }
            catch (JsonReaderException jsonEx)
            {
                MessageBox.Show("Неверный формат json\n" + jsonEx.Message);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                //if (fileName != BackUpFileName)
                    MessageBox.Show("Файл отсутствует\n" + fileNotFoundException.Message);

            }
            catch (Exception ex) { MessageBox.Show("Неизвестная ошибка\n" + ex.Message); }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            RemoveEmptyRows();
            foreach (Lang value in Enum.GetValues(typeof(Lang)))
            {
                File.WriteAllText(GetFileName(value), BuildJson(value));
            }
            Close();
        }

        private string BuildJson(Lang language)
        {
            RemoveEmptyRows();
            var result = new StringBuilder();
            result.Append("{");
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
            result.Append("}");
            return result.ToString();
        }

        private void Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RemoveEmptyRows(); 
            foreach (Lang value in Enum.GetValues(typeof(Lang)))
            {
                File.WriteAllText(GetFileName(value), BuildJson(value));
            }
        }

        private void RemoveEmptyRows()
        {
            for (var i = 0; i < Translations.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(Translations[i].Name))
                    Translations.Remove(Translations[i]);
            }
        }

        private void Clear_Table(object sender, RoutedEventArgs e)
        {
            Translations.Clear();
        }

        private void Translate(string lang)
        {
            Lang language = GetLang(lang);
            var webClient = new WebClient();
            var textToTranslate = new StringBuilder();
            foreach (var translation in Translations)
            {
                textToTranslate.Append("&text=");
                textToTranslate.Append(translation[language]);
            }
            webClient.Encoding = Encoding.UTF8;
            var result = JObject.Parse(webClient.DownloadString(Url + lang + textToTranslate), new JsonLoadSettings() { });

            var i = 0;
            foreach (var item in result.GetValue("text"))
            {
                Translations[i][language] = item.ToString();
                i++;
            }
        }

        private Lang GetLang(string lang)
        {
            switch (lang)
            {
                case "UK":
                    return Lang.UK;
                case "US":
                    return Lang.US;
                case "RU":
                    return Lang.RU;
                default:
                    throw new Exception("Incorrect Lang in Method GetLang");
            }
        }

        private string GetFileName(string lang)
        {
            switch (lang)
            {
                case "UK":
                    return "uk-UK.json";
                case "US":
                    return "en-US.json";
                case "RU":
                    return "ru-RU.json";
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

        private void Translate_to_UA(object sender, RoutedEventArgs e)
        {
            Translate("uk");
        }

        private void Translate_to_EN(object sender, RoutedEventArgs e)
        {
            Translate("en");
        }

        private void Translate_to_RU(object sender, RoutedEventArgs e)
        {
            Translate("ru");
        }

        private void Translate_button(object sender, RoutedEventArgs e)
        {

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
    }
}
