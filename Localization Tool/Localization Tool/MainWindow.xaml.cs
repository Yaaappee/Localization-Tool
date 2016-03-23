using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
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
        private readonly SaveFileDialog _sf;
        private readonly OpenFileDialog _of;
        private string backUpFileName = "backup.json";

        public MainWindow()
        {

            InitializeComponent();
            Translations = new ObservableCollection<Translation>
            {
                new Translation("", "")
            };
            ReadJson(backUpFileName);

            _sf = new SaveFileDialog
            {
                Filter = "JSON-files (*.json)|*.json",
                CheckPathExists = true,
                DefaultExt = "json",
                RestoreDirectory = true
            };

            _of = new OpenFileDialog
            {
                Filter = "JSON-files (*.json)|*.json",
                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true
            };
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Table.ItemsSource = Translations;
        }

        private void Add_Row(object sender, RoutedEventArgs e)
        {
            Translations.Add(new Translation("", ""));
        }

        private void Save_to_JSON(object sender, RoutedEventArgs e)
        {
            _sf.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (_sf.FileName != "")
            {
                File.WriteAllText(_sf.FileName, BuildJson());
                MessageBox.Show("Saved");
            }
        }

        private void Open_JSON(object sender, RoutedEventArgs e)
        {
            _of.ShowDialog();
            if (_of.FileName != "")
            {
                ReadJson(_of.FileName);
                MessageBox.Show("Opened");
            }
        }

        private void ReadJson(string fileName)
        {
            try
            {
                var fileText = File.ReadAllText(fileName);
                var o = JObject.Parse(fileText);
                Translations.Clear();
                foreach (var element in o)
                {
                    Translations.Add(new Translation(element.Key, element.Value.ToString()));
                }
            }
            catch (JsonReaderException jsonEx)
            {
                MessageBox.Show("Неверный формат json\n" + jsonEx.Message);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                if (fileName != backUpFileName)
                    MessageBox.Show("Файл отсутствует\n" + fileNotFoundException.Message);
            }
            catch (Exception ex) { MessageBox.Show("Неизвестная ошибка\n" + ex.Message); }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            RemoveEmptyRows();
            File.WriteAllText(backUpFileName, BuildJson());
            Close();
        }

        private string BuildJson()
        {
            RemoveEmptyRows();
            var result = new StringBuilder();
            result.Append("{");
            for (var i = 0; i < Translations.Count - 1; i++)
            {
                result.Append("\"" + Translations[i].Name + "\"");
                result.Append(":");
                result.Append("\"" + Translations[i].Value + "\"");
                result.Append(",");
            }
            result.Append("\"" + Translations[Translations.Count - 1].Name + "\"");
            result.Append(":");
            result.Append("\"" + Translations[Translations.Count - 1].Value + "\"");

            result.Append("}");
            return result.ToString();
        }

        private void Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RemoveEmptyRows();
            File.WriteAllText(backUpFileName, BuildJson());
        }

        private void RemoveEmptyRows()
        {
            for (int i = 0; i < Translations.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(Translations[i].Value) && string.IsNullOrWhiteSpace(Translations[i].Name))
                    Translations.Remove(Translations[i]);
            }
        }

        private void Clear_Table(object sender, RoutedEventArgs e)
        {
            Translations.Clear();
        }
    }
}
