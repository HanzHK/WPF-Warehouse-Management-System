using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using Serilog;

namespace system_sprava_skladu
{
    /// <summary>
    /// Interakční logika pro DetailDodavatele.xaml
    /// </summary>
    public partial class OknoDetailDodavatele : UserControl
    {
        public OknoDetailDodavatele()
        {
            InitializeComponent();
        }

        private async Task NastavDodavatelePrivateAsync(int dodavatelID)
        {
            try
            {
                DataTable dodavateleTabulka = await SpravaDatabaze.NacitaniDatZDatabazeSeznamdodavatelu.NactiDodavatelezDatabazeAsync();

                DataRow[] vysledky = dodavateleTabulka.Select($"DodavatelID = {dodavatelID}");

                if (vysledky.Length > 0)
                {
                    DataRow dodavatel = vysledky[0];

                    string nazevDodavatele = dodavatel["Nazev"]?.ToString() ?? string.Empty;
                    groupBoxDetail.Header = $"Dodavatel: {nazevDodavatele}";

                    textBlockDodavatelID.Text = dodavatel["DodavatelID"]?.ToString() ?? string.Empty;
                    textBlockNazev.Text = nazevDodavatele?.ToString() ?? string.Empty;
                    textBlockICO.Text = dodavatel["ICO"]?.ToString() ?? string.Empty;
                    textBlockDIC.Text = dodavatel["DIC"]?.ToString() ?? string.Empty;
                    textBlockTypDodavatele.Text = dodavatel["TypDodavatele"]?.ToString() ?? string.Empty;

                    textBlockUlice.Text = dodavatel["Ulice"]?.ToString() ?? string.Empty;
                    textBlockCisloPopisne.Text = dodavatel["CisloPopisne"]?.ToString() ?? string.Empty;
                    textBlockPSC.Text = dodavatel["PSC"]?.ToString() ?? string.Empty;
                    textBlockObec.Text = dodavatel["Obec"]?.ToString() ?? string.Empty;
                    textBlockZeme.Text = dodavatel["ZemeNazev"]?.ToString() ?? string.Empty;
                    
                }
                else
                {
                    groupBoxDetail.Header = "Dodavatel nenalezen";
                }
            }
            catch (Exception ex)
            {
                groupBoxDetail.Header = "Chyba při načítání";
                Log.Error(ex, "Chyba v NastavDodavateleAsync");
            }
        }

        internal async Task NastavDodavateleAsync(int dodavatelID)
        {
           await NastavDodavatelePrivateAsync(dodavatelID);
        }
    }
}
