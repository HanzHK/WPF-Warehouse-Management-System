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
using MahApps.Metro.Controls;
using static system_sprava_skladu.SpravaDatabaze;

namespace system_sprava_skladu
{
    /// <summary>
    /// Interakční logika pro okno_skladovaci_pozice.xaml
    /// </summary>
    public partial class okno_pridej_skladovaci_pozice : UserControl
    {
        // Instance
        private VlozdoDatabazeSkladovaciPozice pozice = new VlozdoDatabazeSkladovaciPozice();

        public okno_pridej_skladovaci_pozice()
        {
            InitializeComponent();
        }

        private async void pridatSkladovaciPoziciButton_Click(object sender, RoutedEventArgs e)
        {
            string skladovaciPoziceNazev = skladovaciPoziceTextBox.Text; 

            int id = await pozice.UlozitSkladovaciPoziciAsync(skladovaciPoziceNazev);  

            MessageBox.Show($"Skladovací pozice uložena s ID: {id}", "Úspěch", MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}
