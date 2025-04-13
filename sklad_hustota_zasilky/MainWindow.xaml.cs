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
            LoadingDatabasePopUpShow();
        }
        // Obsluha události pro kliknutí na tlačítko pro otevření "okna" přidat zásilku

        private void LoadingDatabasePopUpShow()
        {
            this.Hide();
            LoadingDB loadingDB = new();
            loadingDB.Closed += (s, e) =>
            {
                this.Show(); 
            };

            loadingDB.Show();
            loadingDB.StartLoading();
            
        }


        private void OtevritPridejZasilkuOkno_Click(object sender, RoutedEventArgs e)
        {
            // Vytvoření instance UserControlu "okno_pridej_zasilku"
            okno_pridej_zasilku userControl = new();

            // Nastavení obsahu ContentControlu na tento UserControl
            contentControl.Content = userControl;
        }
        private void OtevritPridejDodavateleOkno_Click(object sender, RoutedEventArgs e)
        {
            // Vytvoření instance UserControlu "okno_pridej_dodavatele"
            okno_pridej_dodavatele userControl = new();

            // Nastavení obsahu ContentControlu na tento UserControl
            contentControl.Content = userControl;
        }
        private void OtevritSeznamDodavateluOkno_Click(object sender, RoutedEventArgs e)
        {
            // Vytvoření instance UserControlu "okno_seznam_dodavatelu"
            okno_seznam_dodavatelu userControl = new();

            // Nastavení obsahu ContentControlu na tento UserControl
            contentControl.Content = userControl;
        }

        private void OtevritPridejSkladovaciPoziciOkno_Click(object sender, RoutedEventArgs e)
        {
            // Vytvoření instance UserControlu "okno_pridej_skladovaci_pozice"
            okno_pridej_skladovaci_pozice userControl = new();

            // Nastavení obsahu ContentControlu na tento UserControl
            contentControl.Content = userControl;
        }
        private void OtevritSeznamSkladovaciPoziceOkno_Click(object sender, RoutedEventArgs e)
        {
            // Vytvoření instance UserControlu "okno_pridej_skladovaci_pozice"
            okno_seznam_skladovaci_pozice userControl = new();

            // Nastavení obsahu ContentControlu na tento UserControl
            contentControl.Content = userControl;
        }

    }


}
