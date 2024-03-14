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
using System.Data.SqlClient;
using static sklad_hustota_zasilky.OsetreniVstupu;


namespace sklad_hustota_zasilky
{
    /// <summary>
    /// Interakční logika pro okno_pridej_dodavatele.xaml
    /// </summary>
    public partial class okno_pridej_dodavatele : Window
    {
        
        // Přidejte proměnnou pro indikaci, zda je okno otevřeno nebo zavřeno
        private bool oknoPridejDodavateleOtevreno = false;

        public okno_pridej_dodavatele()
        {
            InitializeComponent();

            // Vytvořte instanci třídy SpravaDatabase
            SpravaDatabaze spravaDatabaze = new SpravaDatabaze();

            // Volá metody pro naplnění ComboBoxů
            spravaDatabaze.NaplnComboBoxTypyDodavatelu(cBoxTypyDodavatelu);
            spravaDatabaze.NaplnComboBoxZeme(cBoxZeme);
        }

        private void txtBoxIco_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Vytzvoření instance a nastavení maximální délky na 8
            OsetreniVstupuCisel osetreniCisel = new OsetreniVstupuCisel(txtBoxIco, 8);

            // Volání metody na ošetření čísel
            osetreniCisel.OsetriVstup(e);
        }
        private void txtBoxDic_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Vytzvoření instance a nastavení maximální délky na 8
            OsetreniVstupuCisel osetreniCisel = new OsetreniVstupuCisel(txtBoxDic, 8);

            // Volání metody na ošetření čísel
            osetreniCisel.OsetriVstup(e);
        }
        private void txtBoxCisloPopisne_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Vytvoření instance a nastavení maximální délky na 10
            OsetreniVstupuCisel osetreniCisel = new OsetreniVstupuCisel(txtBoxCisloPopisne, 10);

            // Volání metody na ošetření čísel
            osetreniCisel.OsetriVstup(e);
        }
        private void txtBoxPsc_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Vytvoření instance třídy OsetreniVstupuCisel
            OsetreniVstupuCisel instance = new OsetreniVstupuCisel(txtBoxPsc);

            // Volání metody na ošetření vstupu čísel
            instance.OsetriVstup(e);

        }
        private void txtBoxPsc_TextChanged(object sender, TextChangedEventArgs e)
        {
            OsetreniVstupuTextChanged osetreniTextChanged = new OsetreniVstupuTextChanged(txtBoxPsc);
            osetreniTextChanged.OsetriVstup(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, Key.Enter));
        }


        // Přidejte událost pro uzávěrku okna
        private void OknoPridejDodavatele_Closed(object sender, EventArgs e)
        {
            // Okno bylo zavřeno, takže nastavte indikátor na false
            oknoPridejDodavateleOtevreno = false;
        }

        // Přidejte metodu, která bude volána při otevření okna z jiného místa
        public void OtevritOkno()
        {
            if (!oknoPridejDodavateleOtevreno)
            {
                Show();
                oknoPridejDodavateleOtevreno = true;
            }
            else
            {
                Activate();
            }
        }
        private void pridatDodavateleDbButton_Click(object sender, RoutedEventArgs e)
        {
            // Obecné - přiřazení hodnot do proměnných
            string nazev = txtBoxNazevDodavatele.Text;
            string ico = txtBoxIco.Text;
            string dic = txtBoxDic.Text;
            string popis = txtBoxPopis.Text;
            string typDodavatele = cBoxTypyDodavatelu.SelectedItem.ToString();

            // Adresa - přiřazení hodnot do proměnných
            string ulice = txtBoxUlice.Text;
            string cislopopisne = txtBoxCisloPopisne.Text;
            string psc = txtBoxPsc.Text;
            string obec = txtBoxObec.Text;
            string zeme = cBoxZeme.SelectedItem.ToString();

            // Vytvoření instance třídy PridejDodavateleSql
            SpravaDatabaze.PridejDodavateleSql pridejDodavateleSql = new SpravaDatabaze.PridejDodavateleSql();

            // Volání metody pro uložení dodavatele
            pridejDodavateleSql.UlozitDodavatele(nazev, ico, dic, popis, typDodavatele, ulice, cislopopisne, psc, obec, zeme);

            // Aktualizace uživatelského rozhraní - vyčištění polí
            txtBoxNazevDodavatele.Clear();
            txtBoxIco.Clear();
            txtBoxDic.Clear();
            txtBoxPopis.Clear();
            txtBoxUlice.Clear();
            txtBoxObec.Clear();
            txtBoxPsc.Clear();
            txtBoxCisloPopisne.Clear();
            // Zavření okna
            this.Close();

        }



    }

    }
