using System.Threading.Tasks;
using System.Windows;

namespace system_sprava_skladu
{
    /// <summary>
    /// Interakční logika pro LoadingDB.xaml
    /// </summary>
    public partial class LoadingDB : Window
    {
        public LoadingDB()
        {
            InitializeComponent();
            
        }
        public void StartLoading()
        {
            ProbuzeniPripojeniDatabaze(); 
        }

        private async void ProbuzeniPripojeniDatabaze()
            {
                RingNacitani.IsActive = true;
                nacitaniText.Content = "Probíhá připojování k databázi...";

                await Task.Delay(5000);
               
                SpravaDatabaze.PripojeniDatabazeObecne pripojeniDatabazeObecne = new();

                if (pripojeniDatabazeObecne.ProbuzeniDatabaze())
                {
                    RingNacitani.IsActive = false;
                    nacitaniText.Content = "Připojení bylo úspěšné, program se spustí.";
                    await Task.Delay(5000);
                    ZavreniOkna();
                }
                else
                {
                    RingNacitani.IsActive = false;
                    MessageBoxResult vysledek = MessageBox.Show
                    (
                    "Připojení selhalo. Chcete opakovat pokus?",
                    "Chyba připojení",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                    );

                    if (vysledek == MessageBoxResult.Yes)
                    {
                        RingNacitani.IsActive = true;
                        ProbuzeniPripojeniDatabaze();
                    }
                    else
                    {
                        ZavreniOkna();
                    }
                }   
            }


        private void ZavreniOkna()
        {
            this.Close();
        }
    }
}
