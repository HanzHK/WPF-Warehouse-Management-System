using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            DataTable dodavateleTable = await SpravaDatabaze.NacitaniDatZDatabazeSeznamdodavatelu.NactiDodavatelezDatabazeAsync();
            dodavateleDataGrid.ItemsSource = dodavateleTable.DefaultView;
        }
        private async void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is DataRowView radek)
            {
                int dodavatelID = (int)radek["DodavatelID"];

                
                if (Window.GetWindow(this) is MainWindow hlavniOkno)
                {
                   await SpravaZobrazeniDetailuDodavatele.OtevriDetailDodavatele(hlavniOkno.contentControl, dodavatelID);
                }
            }
        }
    }

}
