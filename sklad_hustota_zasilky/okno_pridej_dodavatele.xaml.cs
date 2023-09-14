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

            // Zavolejte metodu pro naplnění ComboBoxu
            spravaDatabaze.NaplnComboBoxTypyDodavatelu(cBoxTypyDodavatelu);
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
            string nazev = txtBoxNazevDodavatele.Text;
            string ico = txtBoxIco.Text;
            string dic = txtBoxDic.Text;
            string popis = txtBoxPopis.Text;
            string typDodavatele = cBoxTypyDodavatelu.SelectedItem.ToString();

            // Vytvoření instance třídy PridejDodavateleSql
            SpravaDatabaze.PridejDodavateleSql pridejDodavateleSql = new SpravaDatabaze.PridejDodavateleSql();

            // Volání metody pro uložení dodavatele
            pridejDodavateleSql.UlozitDodavatele(nazev, ico, dic, popis, typDodavatele);

            // Aktualizace uživatelského rozhraní - vyčištění polí
            txtBoxNazevDodavatele.Clear();
            txtBoxIco.Clear();
            txtBoxDic.Clear();
            txtBoxPopis.Clear();
        }
        public class OsetreniVstupu
        {
            private TextBox txtBoxIco;

            public OsetreniVstupu(TextBox textBox)
            {
                txtBoxIco = textBox; // Přiřazení předaného TextBoxu do privátní proměnné
                txtBoxIco.KeyDown += txtBoxIco_KeyDown; // Přidání obsluhy události KeyDown pro TextBox
            }
            private void txtBoxIco_KeyDown(object sender, KeyEventArgs e)
            {
                // Ověříme, zda byla stisknuta číslice, backspace nebo delete
                if ((e.Key < Key.D0 || e.Key > Key.D9) && e.Key != Key.Back && e.Key != Key.Delete)
                {
                    // Pokud ne, zakážeme tuto klávesu
                    e.Handled = true;
                }

                // Získáme text z textového pole
                string text = txtBoxIco.Text;

                // Omezíme délku na 8 číslic
                if (text.Length >= 8 && e.Key != Key.Back && e.Key != Key.Delete)
                {
                    e.Handled = true;
                }
            }
        }
       
    }

}
