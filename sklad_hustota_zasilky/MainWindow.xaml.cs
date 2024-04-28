using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using MahApps.Metro.Controls;

namespace system_sprava_skladu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        // Obsluha události pro kliknutí na tlačítko pro otevření druhého okna
        private void OtevritDruheOkno_Click(object sender, RoutedEventArgs e)
        {
            // Vytvoření instance UserControlu "okno_pridej_zasilku"
            okno_pridej_zasilku userControl = new okno_pridej_zasilku();

            // Nastavení obsahu ContentControlu na tento UserControl
            contentControl.Content = userControl;
        }

    }
    public class pripojeniSQL
    {
        string pripojeniDatabaze = "Server=DESKTOP-PHD2MVI;Database=Warehouseapp;User Id=AdminWH;Password=hovno02;"; // přístup k SQL databázi
        SqlConnection connection;

        public pripojeniSQL()
        {
            connection = new SqlConnection(pripojeniDatabaze);
            connection.Open();
        }
    }

}
