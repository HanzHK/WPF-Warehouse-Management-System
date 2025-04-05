using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace system_sprava_skladu
{
    // Třída pro PreviewKeyDown ošetření textových polí
    internal abstract class OsetreniVstupu
    {
        protected TextBox txtBox;
        protected int maxDelka;

        // Konstruktor pro třídu OsetreniVstupu
        internal OsetreniVstupu(TextBox txtBox, int maxDelka = 7)
        {
            this.txtBox = txtBox;
            this.maxDelka = maxDelka;  
        }

        
        internal virtual void OsetriVstup(KeyEventArgs e) 
        {
            // Zde budu provadět obecné kontroly délky, formátu, atd.
            if (txtBox.Text.Length >= maxDelka && e.Key != Key.Back && e.Key != Key.Delete && e.Key != Key.Space)
            {
                e.Handled = true;
            }
        }

        internal virtual void OsetriVstup(TextChangedEventArgs e)
        {

        }
        protected bool JePlatnyFormat(string text)
        {
            // Zde můžete provádět ověření formátu, pokud je to potřeba
            return true;
        }

        protected static bool IsNumericKey(Key key)
        {
            // Převede klávesy na jejich kód a ověří, zda odpovídají číselným klávesám.
            int keyInt = (int)key;
            return (keyInt >= 34 && keyInt <= 43) || (keyInt >= 74 && keyInt <= 83);
        }

    }
    internal class OsetreniObecnehoVstupu : OsetreniVstupu
    {
        private readonly string povoleneZnaky;

        //Kontruktor třídy
        internal OsetreniObecnehoVstupu(TextBox textBox, int maxDelka = 20, string povoleneZnaky = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
            : base(textBox, maxDelka)
        {
            this.povoleneZnaky = povoleneZnaky;
            if (textBox == null)
                throw new ArgumentNullException(nameof(textBox), "TextBox nesmí být null.");

        }

        internal override void OsetriVstup(KeyEventArgs e)
        {
            string stisknutyZnak = e.Key.ToString();
            if (!string.IsNullOrEmpty(stisknutyZnak) && !JePovolenyZnak(stisknutyZnak))
            {
                e.Handled = true;
            }
            base.OsetriVstup(e);
        }
        private bool JePovolenyZnak(string znak)
        {
            return povoleneZnaky.Contains(znak);
        }

    }
    internal class OsetreniSkladovaciPozice : OsetreniObecnehoVstupu
    {
        internal OsetreniSkladovaciPozice(TextBox textBox, int maxDelka = 6) : base(textBox, maxDelka, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ")
        { 
        }
        internal override void OsetriVstup(KeyEventArgs e)
        {
            base.OsetriVstup(e);

        }
    }
    internal class OsetreniVstupuCisel : OsetreniVstupu
    {
        internal OsetreniVstupuCisel(TextBox textBox, int maxDelka = 6) : base(textBox, maxDelka)
        {
        }

        internal override void OsetriVstup(KeyEventArgs e)
        {
            // Ošetření kopírování čísel do textového pole
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                string text = Clipboard.GetText();
                if (text != null)
                {
                    // Odstraníme mezery ve zkopírovaném textu
                    text = text.Replace(" ", "");

                    // Pokud je text v očekávaném formátu, přidáme mezery na správná místa
                    if (text.Length == 5)
                    {
                        txtBox.Text = text.Insert(3, " ");
                        txtBox.CaretIndex = txtBox.Text.Length;
                        e.Handled = true;
                    }
                }
            }

            // Ošetření když chce uživatel vymazat číslo a začít znovu, umožňuje mazat i přidanou mezeru
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                // Aktuální pozice kurzoru
                int poziceKurzoru = txtBox.CaretIndex;

                // Zjistí, jestli uživatel zrovna chce smazat mezeru
                if (poziceKurzoru > 0 && txtBox.Text.Length > poziceKurzoru - 1)
                {
                    // Pokud je znak na aktuální pozici mezera
                    if (txtBox.Text[poziceKurzoru - 1] == ' ')
                    {
                        txtBox.Text = txtBox.Text.Remove(poziceKurzoru - 1, 1);
                        txtBox.CaretIndex = poziceKurzoru - 1;
                        e.Handled = true;
                        return;
                    }
                }
            }

            if (e.Key == Key.Left)
            {
                // Přesun kurzoru o jeden index doleva
                txtBox.CaretIndex = Math.Max(0, txtBox.CaretIndex - 1);
                e.Handled = true; // Zabraňuje výchozí akci klávesy
            }
            else if (e.Key == Key.Right)
            {
                // Přesun kurzoru o jeden index doprava
                txtBox.CaretIndex = Math.Min(txtBox.Text.Length, txtBox.CaretIndex + 1);
                e.Handled = true; // Zabraňuje výchozí akci klávesy
            }

            if (!IsNumericKey(e.Key) && e.Key != Key.Back && e.Key != Key.Delete)
            {
                e.Handled = true;
            }

            base.OsetriVstup(e); // Zavoláme obecnou metodu pro ošetření vstupu
        }

    }
    internal class OsetreniNve : OsetreniVstupuCisel
    {
        internal OsetreniNve(TextBox textBox) : base(textBox, maxDelka: 26)
        {
        }
    }

    internal class OsetreniVstupuTextChanged : OsetreniVstupu
    {
        internal OsetreniVstupuTextChanged(TextBox txtBox, int maxDelka) : base(txtBox, maxDelka)
        {
            // Registrace metody TxtBox_TextChanged pro obsluhu události TextChanged
            txtBox.TextChanged += TxtBox_TextChanged;
        }
        internal OsetreniVstupuTextChanged(TextBox txtBox) : this(txtBox, 35) // Implicitní nastavení
        {
        }

        private void TxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Uložení pozice kurzoru
            int caretIndex = txtBox.CaretIndex;

            // Pokud se vkládá znak na pozici, která je dělitelná třemi (ale není na začátku), přidej mezery
            if (caretIndex % 4 == 3 && caretIndex != 0 && !e.Handled)
            {
                // Vložení mezery na pozici za trojicí čísel
                txtBox.Text = txtBox.Text.Insert(caretIndex, " ");
                // Posunutí kurzoru na správnou pozici
                txtBox.CaretIndex = caretIndex + 1;
            }
        }
    }

}
