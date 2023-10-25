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
using static sklad_hustota_zasilky.okno_pridej_dodavatele.OsetreniVstupu;
using static sklad_hustota_zasilky.okno_pridej_zasilku.OsetreniVstupu;
using static sklad_hustota_zasilky.SpravaDatabaze;

namespace sklad_hustota_zasilky
{
    /// <summary>
    /// Interakční logika pro Window1.xaml
    /// </summary>
    public partial class okno_pridej_zasilku : Window
    {
        private NacitaniDatzDatabaze _nacitaniDatzDatabaze;

        // Deklarace proměnné pro uchování instance okna pro přidání dodavatele.
        private okno_pridej_dodavatele OknoPridejDodavatele;
        private bool oknoPridejDodavateleOtevreno = false;
        private RefreshCbox refreshCbox;
        private OsetreniNVE osetreniNve;



        public okno_pridej_zasilku()
        {
            InitializeComponent();


            osetreniNve = new OsetreniNVE(txtBoxNveZasilky);

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
                _nacitaniDatzDatabaze.NactiAdresu(vybranyDodavatel, vybranyDodavatelUliceTxt, vybranyDodavatelCisloPopisneTxt, vybranyDodavatelPscTxt, vybranyDodavatelObecTxt);
            }
            else
            {
                // Vybraný dodavatel je prázdný
                // Nastavte TextBlocky na prázdný text nebo jinou chybovou zprávu
                vybranyDodavatelUliceTxt.Text = "";
                vybranyDodavatelCisloPopisneTxt.Text = "";
                vybranyDodavatelPscTxt.Text = "";
                vybranyDodavatelObecTxt.Text = "";
            }
        }


        // Metoda pro ošetření pole NVE zásilky

        private void txtBoxNveZasilky_KeyDown(object sender, KeyEventArgs e)
        {
            osetreniNve.OsetritVstup(e);
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

        public class OsetreniVstupu
        {
            protected TextBox txtBox;

            public OsetreniVstupu(TextBox textBox)
            {
                txtBox = textBox;
            }
            public virtual void OsetritVstup(KeyEventArgs e)
            {
                // Zde budu provadět ošetření vstupu
                // Například kontroly délky, formátu, atd.
            }
            protected bool JePlatnyFormat(string text, string format)
            {
                // Zde provádím ověření formátu
                return text.Length == format.Length && text.All(char.IsDigit);
            }
            public static bool IsNumericKey(Key key)
            {
                // Převede klávesy na jejich kód a ověří, zda odpovídají číselným klávesám.
                int keyInt = (int)key;
                return (keyInt >= 34 && keyInt <= 43) || (keyInt >= 74 && keyInt <= 83);
            }
            public class OsetreniNVE : OsetreniVstupu
            {
                public OsetreniNVE(TextBox textBox) : base(textBox)
                {
                }

                public override void OsetritVstup(KeyEventArgs e)
                {
                    if (!IsNumericKey(e.Key) && e.Key != Key.Back && e.Key != Key.Delete)
                    {
                        e.Handled = true;
                    }

                    // Kontrola délky - maximálně 18 znaků
                    if (txtBox.Text.Length >= 18 && e.Key != Key.Back && e.Key != Key.Delete)
                    {
                        e.Handled = true;
                    }

                }
            }
        }

        private void otevritOknoPridatDodavateleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!oknoPridejDodavateleOtevreno)
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
        }
    }
}

