using Serilog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace system_sprava_skladu
{
    /// <summary>
    /// Interakční logika pro Window1.xaml
    /// </summary>
    public partial class OknoPridejZasilku : UserControl
    {
        private readonly SpravaDatabaze.NacitaniDatzDatabaze _nacitaniDatzDatabaze = new();
        private readonly SpravaDatabaze.NacitaniDatzDatabazeSkladovaciPozice _nacitaniDatzDatabazeSkladovaciPozice = new();

        private void SirkaZasilkyTxt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Vytzvoření instance a nastavení maximální délky na 5
            OsetreniVstupuCisel osetreniCisel = new(SirkaZasilkyTxt, 5);

            // Volání metody na ošetření čísel
            osetreniCisel.OsetriVstup(e);
        }
        private void VyskaZasilkyTxt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Vytzvoření instance a nastavení maximální délky na 5
            OsetreniVstupuCisel osetreniCisel = new(VyskaZasilkyTxt, 5);

            // Volání metody na ošetření čísel
            osetreniCisel.OsetriVstup(e);
        }
        private void DelkaZasilkyTxt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Vytzvoření instance a nastavení maximální délky na 5
            OsetreniVstupuCisel osetreniCisel = new(DelkaZasilkyTxt, 5);

            // Volání metody na ošetření čísel
            osetreniCisel.OsetriVstup(e);
        }
        private void VahaZasilkyTxt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Vytzvoření instance a nastavení maximální délky na 5
            OsetreniVstupuCisel osetreniCisel = new(VahaZasilkyTxt, 5);

            // Zavolejte metodu OsetritVstup pro osetreniCisel
            osetreniCisel.OsetriVstup(e);
        }
        private void TxtBoxNveZasilky_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Vytzvoření instance a nastavení maximální délky na 18
            OsetreniVstupuCisel osetreniNve = new(TxtBoxNveZasilky, maxDelka: 18);

            // Zavolejte metodu OsetritVstup pro osetreniCisel
            osetreniNve.OsetriVstup(e);
        }
       

        public OknoPridejZasilku()
        {
            InitializeComponent();

            // Připojit události Changed na TextBoxy
            SirkaZasilkyTxt.TextChanged += AktualizujUdaje;
            DelkaZasilkyTxt.TextChanged += AktualizujUdaje;
            VyskaZasilkyTxt.TextChanged += AktualizujUdaje;

            InicializujOknoAsync();
        }
        private async void InicializujOknoAsync()
        {
            // Dodavatelé combobox 
            try
            {
                await _nacitaniDatzDatabaze.NaplnComboBoxDodavateluAsync(cBoxDodavatele);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání dodavatelů: " + ex.Message);
                Log.Error(ex, "Chyba při načítání dodavatelů");
            }
            // Skladovací pozice Combobox
            try
            {
                await _nacitaniDatzDatabazeSkladovaciPozice.NaplnComboBoxSkladovaciPoziceAsync(cBoxSkladovaciPozice);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání skladovacích pozic: " + ex.Message);
                Log.Error(ex, "Chyba při načítání skladovacích pozic");
            }
        }
        private void ZobrazitBarcodeNveButton_Click(object sender, RoutedEventArgs e)
        {
            if (KontrolaFormatuNve())
            {
                string nveKod = TxtBoxNveZasilky.Text;
                OknoGenerovaniBarcode oknoBarcode = new(nveKod);
                oknoBarcode.Show();
            }
            else
            {
                MessageBox.Show("NVE kód musí být vyplněný a mít přesně " + TxtBoxNveZasilky.MaxLength + " znaků.",
                                "Neplatný vstup", MessageBoxButton.OK, MessageBoxImage.Warning);
               
            }
        }
        private bool KontrolaFormatuNve()
        {
            string nveKod = TxtBoxNveZasilky.Text;
            if (string.IsNullOrEmpty(nveKod)) { return false; }
            if (nveKod.Length != TxtBoxNveZasilky.MaxLength) { return false; }
            return true;

        }
        private async void CboxDodavatele_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
          // _nacitaniDatzDatabaze = new spravaDatabaze.NacitaniDatzDatabaze();
            // Získáme vybraného dodavatele z comboboxu
            string vybranyDodavatel = cBoxDodavatele.SelectedItem as string ?? String.Empty;

            if (!string.IsNullOrEmpty(vybranyDodavatel))
            {
                // Zavoláme upravenou metodu NactiAdresu a předáme jí textové bloky pro jednotlivé části adresy.
               await _nacitaniDatzDatabaze.NactiAdresu(vybranyDodavatel, vybranyDodavatelUliceTxt, vybranyDodavatelCisloPopisneTxt, vybranyDodavatelPscTxt, vybranyDodavatelObecTxt, vybranyDodavatelZemeTxt);
               await _nacitaniDatzDatabaze.NactiObecneinformaceAsync(vybranyDodavatel, vybranyDodavatelNazevTxt, vybranyDodavatelIcoTxt, vybranyDodavatelDicTxt);
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
            if (!string.IsNullOrWhiteSpace(VyskaZasilkyTxt.Text) && !string.IsNullOrWhiteSpace(DelkaZasilkyTxt.Text) && !string.IsNullOrWhiteSpace(SirkaZasilkyTxt.Text))
            {
                if (double.TryParse(VyskaZasilkyTxt.Text, out double vyska) &&
                    double.TryParse(DelkaZasilkyTxt.Text, out double delka) &&
                    double.TryParse(SirkaZasilkyTxt.Text, out double sirka))
                {
                    // Vypočítání objem v kubických metrech
                    double objem = vyska * delka * sirka / 1_000_000;

                    // Aktualizování obsahu TextBlocku s výsledkem real-time
                    ObjemZasilkyTxt.Text = $"Objem zásilky: {objem} m³";
                }
                else
                {
                    // Pokud některá z hodnot není číslo, zobrazit chybovou zprávu
                    ObjemZasilkyTxt.Text = "Nesprávný vstup";
                }

            }
            else
            {
                // Objem zásilky nebude zobrazen pokud nebudou vyplněna všechna pole
                ObjemZasilkyTxt.Text = "";
            }
        }
    }
}

