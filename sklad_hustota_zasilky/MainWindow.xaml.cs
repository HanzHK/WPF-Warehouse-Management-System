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

namespace sklad_hustota_zasilky
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string pripojeniDatabaze = "Server=DESKTOP-PHD2MVI;Database=Warehouseapp;User Id=AdminWH;Password=hovno02;";    //přistup k SQL databázi

        }
        // Obsluha události pro kliknutí na tlačítko pro otevření druhého okna
        private void OtevritDruheOkno_Click(object sender, RoutedEventArgs e)
        {
            // Vytvoříme instanci druhého okna
            okno_pridej_zasilku druheOkno = new okno_pridej_zasilku();

            // Zobrazíme druhé okno
            druheOkno.Show();
        }
    }
}
