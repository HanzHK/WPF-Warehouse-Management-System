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
        private NacitaniDatzDatabazeSkladovaciPozice _nacitaniDatzDatabazeSkladovaciPozice;

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
            OsetreniVstupuCisel osetreniNve = new OsetreniVstupuCisel(txtBoxNveZasilky, maxDelka: 18);

            // Zavolejte metodu OsetritVstup pro osetreniCisel
            osetreniNve.OsetriVstup(e);
        }
       

        public okno_pridej_zasilku()
        {
            InitializeComponent();
            // Vytvoření instance RefreshCbox a NacitaniDatzDatabaze
            _nacitaniDatzDatabaze = new NacitaniDatzDatabaze();
            _nacitaniDatzDatabazeSkladovaciPozice = new NacitaniDatzDatabazeSkladovaciPozice();

            // Připojit události Changed na TextBoxy
            sirkaZasilkyTxt.TextChanged += AktualizujUdaje;
            delkaZasilkyTxt.TextChanged += AktualizujUdaje;
            vyskaZasilkyTxt.TextChanged += AktualizujUdaje;

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
            }
            // Skladovací pozice Combobox
            try
            {
                await _nacitaniDatzDatabazeSkladovaciPozice.NaplnComboBoxSkladovaciPoziceAsync(cBoxSkladovaciPozice);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání skladovacích pozic: " + ex.Message);
            }
        }
        private void zobrazitBarcodeNveButton_Click(object sender, RoutedEventArgs e)
        {
            string nveKod = txtBoxNveZasilky.Text;
            okno_generovani_barcode oknoBarcode = new okno_generovani_barcode(nveKod);
            oknoBarcode.Show();
        }

        private async void cBoxDodavatele_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            _nacitaniDatzDatabaze = new NacitaniDatzDatabaze();
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
    }
}

