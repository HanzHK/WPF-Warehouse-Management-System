using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
using System.Windows.Shapes;
using static system_sprava_skladu.SpravaDatabaze;
using static system_sprava_skladu.OsetreniVstupu;
using MahApps.Metro.Controls;


namespace system_sprava_skladu
{
    /// <summary>
    /// Interakční logika pro Window1.xaml
    /// </summary>
    public partial class okno_pridej_zasilku : UserControl
    {
        private NacitaniDatzDatabaze _nacitaniDatzDatabaze;

        // Deklarace proměnné pro uchování instance okna pro přidání dodavatele.
        private okno_pridej_dodavatele OknoPridejDodavatele;
        private bool oknoPridejDodavateleOtevreno = false;
        private RefreshCbox refreshCbox;

        private void sirkaZasilkyTxt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Vytzvoření instance a nastavení maximální délky na 5
            OsetreniVstupuCisel osetreniCisel = new OsetreniVstupuCisel(sirkaZasilkyTxt, 5);

            // Volání metody na ošetření čísel
            osetreniCisel.OsetriVstup(e);
        }
        private void vyskaZasilkyTxt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Vytzvoření instance a nastavení maximální délky na 5
            OsetreniVstupuCisel osetreniCisel = new OsetreniVstupuCisel(vyskaZasilkyTxt, 5);

            // Volání metody na ošetření čísel
            osetreniCisel.OsetriVstup(e);
        }
        private void delkaZasilkyTxt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Vytzvoření instance a nastavení maximální délky na 5
            OsetreniVstupuCisel osetreniCisel = new OsetreniVstupuCisel(delkaZasilkyTxt, 5);

            // Volání metody na ošetření čísel
            osetreniCisel.OsetriVstup(e);
        }
        private void vahaZasilkyTxt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Vytzvoření instance a nastavení maximální délky na 5
            OsetreniVstupuCisel osetreniCisel = new OsetreniVstupuCisel(vahaZasilkyTxt, 5);

            // Zavolejte metodu OsetritVstup pro osetreniCisel
            osetreniCisel.OsetriVstup(e);
        }
        private void txtBoxNveZasilky_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Vytzvoření instance a nastavení maximální délky na 18
            OsetreniNve osetreniNve = new OsetreniNve(txtBoxNveZasilky);

            // Zavolejte metodu OsetritVstup pro osetreniCisel
            osetreniNve.OsetriVstup(e);
        }
        private void txtBoxNveZasilky_TextChanged(object sender, TextChangedEventArgs e)
        {
            OsetreniVstupuTextChanged osetreniTextChanged = new OsetreniVstupuTextChanged(txtBoxNveZasilky, 35);
            osetreniTextChanged.OsetriVstup(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, Key.Enter));
        }

        public okno_pridej_zasilku()
        {
            InitializeComponent();

            // Vytvoření instance okna pro přidání dodavatele, ale zatím se neotevře.
            OknoPridejDodavatele = new okno_pridej_dodavatele();

            /*
           Tato část kódu se stará o přidání položek do comboboxu,
           ve kterém se vybírají ze seznamu.
           */
            // Vytvoření instance RefreshCbox a NacitaniDatzDatabaze
            NacitaniDatzDatabaze nacitaniDatabaze = new NacitaniDatzDatabaze();
            refreshCbox = new RefreshCbox(nacitaniDatabaze);
            
            // Naplnění comboboxu
           nacitaniDatabaze.NaplnComboBoxDodavatelu(cBoxDodavatele);

            DataContext = refreshCbox;

            // Připojit události Changed na TextBoxy
            sirkaZasilkyTxt.TextChanged += AktualizujUdaje;
            delkaZasilkyTxt.TextChanged += AktualizujUdaje;
            vyskaZasilkyTxt.TextChanged += AktualizujUdaje;
        }

        // Pomocná třída pro automatický refresh combnoboxu s dodavateli
        // Pro správně fungující binding

        public class RefreshCbox
            {
            private NacitaniDatzDatabaze _nacitaniDatzDatabaze;

            // Konstruktor, který přijímá instanci NacitaniDatzDatabaze
            public RefreshCbox(NacitaniDatzDatabaze nacitaniDatzDatabaze)
            {
                _nacitaniDatzDatabaze = nacitaniDatzDatabaze;
            }

            // Vlastnost pro data, která chcete použít pro binding
            public ObservableCollection<string> SeznamDodavatelu => _nacitaniDatzDatabaze.SeznamDodavatelu;

            // Druhá metoda pro načítání dat
            public void NacistDataDoComboBoxu(ComboBox comboBox)
            {
               _nacitaniDatzDatabaze.NaplnComboBoxDodavatelu(comboBox);
            }
        }
        private void cBoxDodavatele_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _nacitaniDatzDatabaze = new NacitaniDatzDatabaze();
            // Získáme vybraného dodavatele z comboboxu
            string vybranyDodavatel = cBoxDodavatele.SelectedItem as string;

            if (!string.IsNullOrEmpty(vybranyDodavatel))
            {
                // Zavoláme upravenou metodu NactiAdresu a předáme jí textové bloky pro jednotlivé části adresy.
                _nacitaniDatzDatabaze.NactiAdresu(vybranyDodavatel, vybranyDodavatelUliceTxt, vybranyDodavatelCisloPopisneTxt, vybranyDodavatelPscTxt, vybranyDodavatelObecTxt, vybranyDodavatelZemeTxt);
                _nacitaniDatzDatabaze.NactiObecneinformace(vybranyDodavatel, vybranyDodavatelNazevTxt, vybranyDodavatelIcoTxt, vybranyDodavatelDicTxt);
            }
            else
            {
                // Vybraný dodavatel je prázdný
                // Nastavte TextBlocky na prázdný text nebo jinou chybovou zprávu
                vybranyDodavatelUliceTxt.Text = "";
                vybranyDodavatelCisloPopisneTxt.Text = "";
                vybranyDodavatelPscTxt.Text = "  ";
                vybranyDodavatelObecTxt.Text = "";
                vybranyDodavatelZemeTxt.Text = "";

                vybranyDodavatelNazevTxt.Content = "";
                vybranyDodavatelIcoTxt.Text = "";
                vybranyDodavatelDicTxt.Text = "";

            }
        }

        private void AktualizujUdaje(object sender, TextChangedEventArgs e)
        {
            // Získat hodnoty z TextBoxů
            if (!string.IsNullOrWhiteSpace(vyskaZasilkyTxt.Text) && !string.IsNullOrWhiteSpace(delkaZasilkyTxt.Text) && !string.IsNullOrWhiteSpace(sirkaZasilkyTxt.Text))
            {
                if (double.TryParse(vyskaZasilkyTxt.Text, out double vyska) &&
                    double.TryParse(delkaZasilkyTxt.Text, out double delka) &&
                    double.TryParse(sirkaZasilkyTxt.Text, out double sirka))
                {
                    // Vypočítání objem v kubických metrech
                    double objem = vyska * delka * sirka / 1_000_000;

                    // Aktualizování obsahu TextBlocku s výsledkem real-time
                    objemZasilkyTxt.Text = $"Objem zásilky: {objem} m³";
                }
                else
                {
                    // Pokud některá z hodnot není číslo, zobrazit chybovou zprávu
                    objemZasilkyTxt.Text = "Nesprávný vstup";
                }

            }
            else
            {
                // Objem zásilky nebude zobrazen pokud nebudou vyplněna všechna pole
                objemZasilkyTxt.Text = "";
            }
        }

        // Obsluha události DodavatelClosed
        private void OknoPridejDodavatele_Closed(object sender, EventArgs e)
        {
            // Okno bylo zavřeno, takže nastavte indikátor na false
            oknoPridejDodavateleOtevreno = false;

            // Inicializujte proměnnou OknoPridejDodavatele znovu
            OknoPridejDodavatele = null;
        }

        private void otevritOknoPridatDodavateleButton_Click(object sender, RoutedEventArgs e)
        {
            /*   if (!oknoPridejDodavateleOtevreno)
               {
                   OknoPridejDodavatele = new okno_pridej_dodavatele();
                   OknoPridejDodavatele.Closed += OknoPridejDodavatele_Closed;
                   OknoPridejDodavatele.Show();
                   oknoPridejDodavateleOtevreno = true;

                   // Nejdříve vyčištění seznamu
                   // cBoxDodavatele.Items.Clear();

                  // Aktualizujte ComboBox po zavření okna
                  OknoPridejDodavatele.Closed += (s, args) =>
                  {
                       refreshCbox.NacistDataDoComboBoxu(cBoxDodavatele);
                   };
               }
               else
               {
                   OknoPridejDodavatele.Activate();
               }
            */
        }

    }
}

