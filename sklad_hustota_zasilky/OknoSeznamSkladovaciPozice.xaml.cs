using System.Collections.Generic;
using System.Windows.Controls;

namespace system_sprava_skladu
{
    /// <summary>
    /// Interakční logika pro OknoSeznamSkladovaciPozice.xaml
    /// </summary>
    public partial class OknoSeznamSkladovaciPozice : UserControl
    {

        private readonly SpravaDatabaze.NacitaniDatzDatabazeSkladovaciPozice _nacitani = new();

        public OknoSeznamSkladovaciPozice()
        {
            InitializeComponent();
            NactiSkladovaciPozice();
        }
        private async void NactiSkladovaciPozice()
        {
            List<SpravaDatabaze.NacitaniDatzDatabazeSkladovaciPozice.SkladovaciPoziceDTO> data = await _nacitani.NactiSkladovaciPoziceAsync();
            dataGridSkladovaciPozice.ItemsSource = data;
        }
    }
}
