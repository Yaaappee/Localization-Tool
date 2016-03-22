using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Localization_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Translation> Translations { get; set; }
        public static RoutedCommand StartServiceRoutedCmd = new RoutedCommand();
        public MainWindow()
        {
            InitializeComponent();
            Translations = new ObservableCollection<Translation>()
            {
                new Translation("", "")
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
    }
}
