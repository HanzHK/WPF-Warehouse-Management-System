using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace sklad_hustota_zasilky
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
    internal class OsetreniVstupuCisel : OsetreniVstupu
    {
        internal OsetreniVstupuCisel(TextBox textBox, int maxDelka = 6) : base(textBox, maxDelka)
        {
        }

        internal override void OsetriVstup(KeyEventArgs e)
        {
            // Pokud uživatel zkopíruje číslo, které obsahuje mezery, přidáme mezery automaticky
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

            if (!IsNumericKey(e.Key) && e.Key != Key.Back && e.Key != Key.Delete)
            {
                e.Handled = true;
            }

            base.OsetriVstup(e); // Zavoláme obecnou metodu pro ošetření vstupu
        }

    }
    internal class OsetreniNve : OsetreniVstupuCisel
    {
        internal OsetreniNve(TextBox textBox) : base(textBox, maxDelka: 18)
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
        internal OsetreniVstupuTextChanged(TextBox txtBox) : this(txtBox, 25) // Implicitně 25
        {
        }

        private void TxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Kontrola, zda text dosáhl délky 3 a nekončí mezerou
            if (txtBox.Text.Length == 3 && !txtBox.Text.EndsWith(" "))
            {
                // Vložení mezery na 3. pozici
                txtBox.Text = txtBox.Text.Insert(3, " ");
                // Posunutí kurzoru na 4. pozici
                txtBox.CaretIndex = 4;
            }
        }
    }

}
