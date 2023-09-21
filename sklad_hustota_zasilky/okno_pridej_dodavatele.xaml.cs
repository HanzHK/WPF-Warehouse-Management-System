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
using static sklad_hustota_zasilky.okno_pridej_dodavatele.OsetreniVstupu;


namespace sklad_hustota_zasilky
{
    /// <summary>
    /// Interakční logika pro okno_pridej_dodavatele.xaml
    /// </summary>
    public partial class okno_pridej_dodavatele : Window
    {
        
        // Přidejte proměnnou pro indikaci, zda je okno otevřeno nebo zavřeno
        private bool oknoPridejDodavateleOtevreno = false;

        private OsetreniICO osetreniIco;
        private OsetreniDIC osetreniDic;

        public okno_pridej_dodavatele()
        {
            InitializeComponent();
            osetreniIco = new OsetreniICO(txtBoxIco);
            osetreniDic = new OsetreniDIC(txtBoxDic);

            // Vytvořte instanci třídy SpravaDatabase
            SpravaDatabaze spravaDatabaze = new SpravaDatabaze();

            // Zavolejte metodu pro naplnění ComboBoxu
            spravaDatabaze.NaplnComboBoxTypyDodavatelu(cBoxTypyDodavatelu);
        }

        private void txtBoxIco_KeyDown(object sender, KeyEventArgs e)
        {
            osetreniIco.OsetritVstup(e);
        }

        private void txtBoxDic_KeyDown(object sender, KeyEventArgs e)
        {
            osetreniDic.OsetritVstup(e);
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

            public class OsetreniICO : OsetreniVstupu
            {
                public OsetreniICO(TextBox textBox) : base(textBox)
                {
                }

                public override void OsetritVstup(KeyEventArgs e)
                {
                    if (!IsNumericKey(e.Key) && e.Key != Key.Back && e.Key != Key.Delete)
                    {
                        e.Handled = true;
                    }

                    // Kontrola délky - maximálně 8 znaků
                    if (txtBox.Text.Length >= 8 && e.Key != Key.Back && e.Key != Key.Delete)
                    {
                        e.Handled = true;
                    }

                }
            }

            public class OsetreniDIC : OsetreniVstupu
            {
                public OsetreniDIC(TextBox textBox) : base(textBox)
                {
                }

                public override void OsetritVstup(KeyEventArgs e)
                {
                    
                        if (!IsNumericKey(e.Key) && e.Key != Key.Back && e.Key != Key.Delete)
                        {
                            e.Handled = true;
                        }

                    // Kontrola délky - maximálně 8 znaků
                    if (txtBox.Text.Length > 8 && e.Key != Key.Back && e.Key != Key.Delete)
                    {
                        e.Handled = true;
                    }

                    // Zde můžete provádět další specifické kontroly pro DIČ
                }
            }


        }

       
    }

}
