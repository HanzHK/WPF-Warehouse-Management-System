using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace sklad_hustota_zasilky
{
    internal abstract class OsetreniVstupu
    {
        protected TextBox txtBox;
        protected int maxDelka;

        // Konstruktor pro třídu OsetreniVstupu
        internal OsetreniVstupu(TextBox txtBox, int maxDelka)
        {
            this.txtBox = txtBox;
            this.maxDelka = maxDelka;   
        }
        internal virtual void OsetriVstup(KeyEventArgs e) 
        {
            // Zde budu provadět obecné kontroly délky, formátu, atd.
            if (txtBox.Text.Length >= maxDelka && e.Key != Key.Back && e.Key != Key.Delete)
            {
                e.Handled = true;
            }
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
        internal OsetreniVstupuCisel(TextBox textBox, int maxDelka) : base(textBox, maxDelka)
        {
        }

        internal override void OsetriVstup(KeyEventArgs e)
        {
            if (!IsNumericKey(e.Key) && e.Key != Key.Back && e.Key != Key.Delete)
            {
                e.Handled = true;
            }

            base.OsetriVstup(e); // Zavoláme obecnou metodu pro ošetření vstupu
        }
    }
}
