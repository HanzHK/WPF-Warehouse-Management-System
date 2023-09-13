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
using System.Windows.Shapes;
using static sklad_hustota_zasilky.SpravaDatabaze;

namespace sklad_hustota_zasilky
{
    /// <summary>
    /// Interakční logika pro Window1.xaml
    /// </summary>
    public partial class okno_pridej_zasilku : Window
    {
        // Deklarace proměnné pro uchování instance okna pro přidání dodavatele.
        private okno_pridej_dodavatele OknoPridejDodavatele;
        private bool oknoPridejDodavateleOtevreno = false;

        public okno_pridej_zasilku()
        {
            InitializeComponent();

            // Vytvoření instance okna pro přidání dodavatele, ale zatím se neotevře.
            OknoPridejDodavatele = new okno_pridej_dodavatele();

           
            
            /*
           Tato část kódu se stará o přidání položek do comboboxu,
           ve kterém se vybírají ze seznamu.
           */
            cBoxDodavatele.Items.Add("Vyberte dodavatele");
            cBoxDodavatele.Items.Add("Přidat nového dodavatele");
            NacitaniDatzDatabaze nacitaniDatabaze = new NacitaniDatzDatabaze();
            nacitaniDatabaze.NaplnComboBoxDodavatelu(cBoxDodavatele);


            // Připojit události Changed na TextBoxy
            sirkaZasilkyTxt.TextChanged += AktualizujUdaje;
            delkaZasilkyTxt.TextChanged += AktualizujUdaje;
            vyskaZasilkyTxt.TextChanged += AktualizujUdaje;
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

        /* část programu s metodou, která se stará o to, aby se otevřelo nové okno pro přidávání dodavatelů, 
         * pokud uživatel vybere "Přidat nového dodavatele z comboboxu na výběr dodavatele zásilky" 
         */
        private void cBoxDodavatele_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string vybranaPolozka = cBoxDodavatele.SelectedItem as string;

            if (vybranaPolozka == "Přidat nového dodavatele")
            {
                if (!oknoPridejDodavateleOtevreno)
                {
                    OknoPridejDodavatele = new okno_pridej_dodavatele();
                    OknoPridejDodavatele.Closed += OknoPridejDodavatele_Closed;
                    OknoPridejDodavatele.Show();
                    oknoPridejDodavateleOtevreno = true;
                    
                }
                else
                {
                    OknoPridejDodavatele.Activate();
                }
            }
        }

    }
}

