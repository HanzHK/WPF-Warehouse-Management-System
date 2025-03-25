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
    /// Interakční logika pro okno_seznam_skladovaci_pozice.xaml
    /// </summary>
    public partial class okno_seznam_skladovaci_pozice : UserControl
    {
        private readonly NacitaniDatzDatabazeSkladovaciPozice _nacitani = new NacitaniDatzDatabazeSkladovaciPozice();

        public okno_seznam_skladovaci_pozice()
        {
            InitializeComponent();
            NactiSkladovaciPozice();
        }
        private async void NactiSkladovaciPozice()
        {
            List<NacitaniDatzDatabazeSkladovaciPozice.SkladovaciPoziceDTO> data = await _nacitani.NactiSkladovaciPoziceAsync();
            dataGridSkladovaciPozice.ItemsSource = data;
        }
    }
}
