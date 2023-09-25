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

        //deklarace soukromých lokálních proměnných pro ošetření textových polí
        private OsetreniICO osetreniIco;
        private OsetreniDIC osetreniDic;
        private OsetreniPSC osetreniPsc;

        public okno_pridej_dodavatele()
        {
            InitializeComponent();
            osetreniIco = new OsetreniICO(txtBoxIco);
            osetreniDic = new OsetreniDIC(txtBoxDic);
            osetreniPsc = new OsetreniPSC(txtBoxPsc);

            // Přiřazení oobslužných metod pro TextChanged a PreviewKeyDown
            txtBoxPsc.TextChanged += txtBoxPsc_TextChanged;
            txtBoxPsc.PreviewKeyDown += txtBoxPsc_PreviewKeyDown;

            // Vytvořte instanci třídy SpravaDatabase
            SpravaDatabaze spravaDatabaze = new SpravaDatabaze();

            // Volá metody pro naplnění ComboBoxů
            spravaDatabaze.NaplnComboBoxTypyDodavatelu(cBoxTypyDodavatelu);
            spravaDatabaze.NaplnComboBoxZeme(cBoxZeme);


        }

        private void txtBoxIco_KeyDown(object sender, KeyEventArgs e)
        {
            osetreniIco.OsetritVstup(e);
        }

        private void txtBoxDic_KeyDown(object sender, KeyEventArgs e)
        {
            osetreniDic.OsetritVstup(e);
        }
            
        private void txtBoxPsc_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            osetreniPsc.OsetritPreviewKeyDown(e);
        }
        private void txtBoxPsc_TextChanged(object sender, TextChangedEventArgs e)
        {
            osetreniPsc.OsetritTextChanged();
        }

        private void txtBoxPsc_KeyDown(object sender, KeyEventArgs e)
        {
            osetreniPsc.OsetritVstup(e);
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
            string obec = txtBoxObec.Text;
            string psc = txtBoxPsc.Text;
            string zeme = cBoxZeme.SelectedItem.ToString();

            // Vytvoření instance třídy PridejDodavateleSql
            SpravaDatabaze.PridejDodavateleSql pridejDodavateleSql = new SpravaDatabaze.PridejDodavateleSql();

            // Volání metody pro uložení dodavatele
            pridejDodavateleSql.UlozitDodavatele(nazev, ico, dic, popis, typDodavatele, ulice, cislopopisne, obec, psc, zeme);

            // Aktualizace uživatelského rozhraní - vyčištění polí
            txtBoxNazevDodavatele.Clear();
            txtBoxIco.Clear();
            txtBoxDic.Clear();
            txtBoxPopis.Clear();
            txtBoxUlice.Clear();
            txtBoxObec.Clear();
            txtBoxPsc.Clear();
            txtBoxCisloPopisne.Clear();
        
        }

        /* Třídy pro ošetřování textových polí ve formuláři
         * 
        */

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
            public class OsetreniPSC : OsetreniVstupu
            {
                public OsetreniPSC(TextBox textBox) : base(textBox)
                {
                }

                public void OsetritPreviewKeyDown(KeyEventArgs e)
                {
                    // Zde provádíme kontrolu klávesového stisku
                    if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        // Můžete provádět další kontroly, pokud jsou potřeba
                    }
                }

                public void OsetritTextChanged()
                {
                    // Zde provádíme kontrolu textu po změně
                    string text = txtBox.Text;

                    // Odstranění všech mezer z textu
                    string textBezMezer = text.Replace(" ", "");

                    // Pokud text má délku 0-5 a obsahuje pouze číslice, ponecháme ho
                    if (text.Length >= 0 && text.Length <= 5 && textBezMezer.All(char.IsDigit))
                    {
                        return;
                    }
                    // Pokud Neobsahuje číslice smažeme ho
                    if (text.Length >= 0 && !textBezMezer.All(char.IsDigit))
                    {
                        txtBox.Text = text.Substring(0, Math.Max(0, text.Length - 1));
                        txtBox.CaretIndex = txtBox.Text.Length;
                        return;
                    }

                    // Pokud text má délku 5 a obsahuje pouze číslice, přidáme mezera za třetím číslem
                    if (text.Length == 5 && textBezMezer.All(char.IsDigit))
                    {
                        txtBox.Text = text.Substring(0, 3) + " " + text.Substring(3);
                        txtBox.CaretIndex = txtBox.Text.Length;
                        return;
                    }

                    // Pokud text má délku 6 a odpovídá formátu pět číslic + mezera za třetím číslem, ponecháme ho
                    if (text.Length == 6 && text[0] >= '0' && text[0] <= '9' &&
                        text[1] >= '0' && text[1] <= '9' &&
                        text[2] == ' ' &&
                        text[3] >= '0' && text[3] <= '9' &&
                        text[4] >= '0' && text[4] <= '9' &&
                        text[5] >= '0' && text[5] <= '9')
                    {
                        return;
                    }

                    // Pokud text nesplňuje žádný z těchto podmínek, zamezíme dalšímu psaní
                    txtBox.Text = text.Substring(0, Math.Min(6, text.Length));
                    txtBox.CaretIndex = txtBox.Text.Length;
                }
            }


        }

    }

    }
