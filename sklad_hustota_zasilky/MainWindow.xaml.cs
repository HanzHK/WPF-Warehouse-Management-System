﻿using MahApps.Metro.Controls;
using System.Windows;

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
            // Vytvoření instance UserControlu "OknoPridejZasilku"
            OknoPridejZasilku userControl = new();

            // Nastavení obsahu ContentControlu na tento UserControl
            contentControl.Content = userControl;
        }
        private void OtevritPridejDodavateleOkno_Click(object sender, RoutedEventArgs e)
        {
            // Vytvoření instance UserControlu "okno_pridej_dodavatele"
            OknoPridejDodavatele userControl = new();

            // Nastavení obsahu ContentControlu na tento UserControl
            contentControl.Content = userControl;
        }
        private void OtevritSeznamDodavateluOkno_Click(object sender, RoutedEventArgs e)
        {
            // Vytvoření instance UserControlu "OknoSeznamDodavatelu"
            OknoSeznamDodavatelu userControl = new();

            // Nastavení obsahu ContentControlu na tento UserControl
            contentControl.Content = userControl;
        }

        private void OtevritPridejSkladovaciPoziciOkno_Click(object sender, RoutedEventArgs e)
        {
            // Vytvoření instance UserControlu "OknoPridejSkladovaciPozice"
            OknoPridejSkladovaciPozice userControl = new();

            // Nastavení obsahu ContentControlu na tento UserControl
            contentControl.Content = userControl;
        }
        private void OtevritSeznamSkladovaciPoziceOkno_Click(object sender, RoutedEventArgs e)
        {
            // Vytvoření instance UserControlu "OknoSeznamSkladovaciPozice"
            OknoSeznamSkladovaciPozice userControl = new();

            // Nastavení obsahu ContentControlu na tento UserControl
            contentControl.Content = userControl;
        }

    }


}
