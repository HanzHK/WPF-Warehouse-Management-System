using System.Data;
using System.Windows.Controls;
using static system_sprava_skladu.SpravaDatabaze;

namespace system_sprava_skladu
{
    /// <summary>
    /// Interakční logika pro OknoSeznamDodavatelu.xaml
    /// </summary>
    public partial class OknoSeznamDodavatelu : UserControl
    {
        public OknoSeznamDodavatelu()
        {
            InitializeComponent();
            NactiDataGrid();
        }
        private async void NactiDataGrid()
        {
            DataTable dodavateleTable = await NacitaniDatZDatabazeSeznamdodavatelu.NactiDodavatelezDatabazeAsync();
            dodavateleDataGrid.ItemsSource = dodavateleTable.DefaultView;
        }
    }
}
