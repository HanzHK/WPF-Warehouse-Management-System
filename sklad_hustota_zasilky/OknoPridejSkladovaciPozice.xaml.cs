using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace system_sprava_skladu
{
    /// <summary>
    /// Interakční logika pro OknoSkladovaciPozice.xaml
    /// </summary>
    public partial class OknoPridejSkladovaciPozice : UserControl
    {
        // Instance
        private readonly SpravaDatabaze.VlozdoDatabazeSkladovaciPozice pozice = new();

        //Deklarace
        private readonly OsetreniObecnehoVstupu osetreniSkladovaciPozice;

        public OknoPridejSkladovaciPozice()
        {
            InitializeComponent();

            osetreniSkladovaciPozice = new OsetreniObecnehoVstupu(SkladovaciPoziceTextBox, maxDelka: 6);
        }
        private void SkladovaciPoziceTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            osetreniSkladovaciPozice.OsetriVstup(e); 
        }

        private async void PridatSkladovaciPoziciButton_Click(object sender, RoutedEventArgs e)
        {
            string skladovaciPoziceNazev = SkladovaciPoziceTextBox.Text;

            // Kontrola jestli název skladovací pozice není duplicitní
            if (await pozice.KontrolaDuplicityNazvuPozice(skladovaciPoziceNazev))
            {
                MessageBox.Show("Tato skladovací pozice již existuje!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int id = await SpravaDatabaze.VlozdoDatabazeSkladovaciPozice.UlozitSkladovaciPoziciAsync(skladovaciPoziceNazev);

            MessageBox.Show($"Skladovací pozice uložena s ID: {id}", "Úspěch", MessageBoxButton.OK, MessageBoxImage.Information);
            SkladovaciPoziceTextBox.Text = string.Empty;

        }
    }
}
