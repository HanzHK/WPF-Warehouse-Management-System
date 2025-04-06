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
        protected string povoleneZnaky = "";

        // Konstruktor pro třídu OsetreniVstupu
        internal OsetreniVstupu(TextBox txtBox, int maxDelka = 7, string povoleneZnaky = "")
        {
            this.txtBox = txtBox;
            this.maxDelka = maxDelka;
            this.povoleneZnaky = povoleneZnaky;
        }

        // Kontrola je-li znak povolen
        protected virtual bool JePovolenyZnak(Key key)
        {
            string stisknutyZnak = key.ToString();

            if (stisknutyZnak.StartsWith("D") && stisknutyZnak.Length == 2 && char.IsDigit(stisknutyZnak[1]))
            {
                stisknutyZnak = stisknutyZnak[1].ToString();
            }
            else if (key >= Key.NumPad0 && key <= Key.NumPad9)
            {
                
                stisknutyZnak = ((int)(key - Key.NumPad0)).ToString();
            }

            return povoleneZnaky.Contains(stisknutyZnak);
        }

        internal virtual void OsetriVstup(KeyEventArgs e)
        {
            // Zakáže přidat další znak po dosažení maximální délky
            if ((txtBox.Text.Length - txtBox.SelectionLength) >= maxDelka
                && e.Key != Key.Back && e.Key != Key.Delete
                && e.Key != Key.Left && e.Key != Key.Right)
            {
                e.Handled = true;
            }

            // Ošetření mazání pomocí Backspace
            if (e.Key == Key.Back)
            {
                if (txtBox.SelectionLength > 0)
                {
                    // Pokud je text vybrán, smažeme celý vybraný text
                    txtBox.SelectedText = "";
                    e.Handled = true;
                }
                else
                {
                    // Pokud není text vybrán, smažeme jeden znak před kurzorem
                    int pozice = txtBox.CaretIndex;
                    if (pozice > 0)
                    {
                        txtBox.Text = txtBox.Text.Remove(pozice - 1, 1);
                        txtBox.CaretIndex = pozice - 1;
                        e.Handled = true;
                    }
                }
            }

            // Ošetření mazání pomocí Delete
            else if (e.Key == Key.Delete)
            {
                if (txtBox.SelectionLength > 0)
                {
                    // Pokud je text vybrán, smažeme celý vybraný text
                    txtBox.SelectedText = "";
                    e.Handled = true;
                }
                else
                {
                    // Pokud není text vybrán, smažeme jeden znak za kurzorem
                    int pozice = txtBox.CaretIndex;
                    if (pozice < txtBox.Text.Length)
                    {
                        txtBox.Text = txtBox.Text.Remove(pozice, 1);
                        e.Handled = true;
                    }
                }
            }

            // Pohyb kurzoru
            if (e.Key == Key.Left)
            {
                txtBox.CaretIndex = Math.Max(0, txtBox.CaretIndex - 1);
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                txtBox.CaretIndex = Math.Min(txtBox.Text.Length, txtBox.CaretIndex + 1);
                e.Handled = true;
            }
        }

        internal virtual void OsetriVstup(TextChangedEventArgs e)
        {

        }
        protected bool JePlatnyFormat(string text)
        {
           
            return true;
        }

    }
    internal class OsetreniObecnehoVstupu : OsetreniVstupu
    {
        internal OsetreniObecnehoVstupu(TextBox textBox, int maxDelka = 20, string povoleneZnaky = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ěščřžýáíé")
            : base(textBox, maxDelka, povoleneZnaky)
        {
            if (textBox == null)
                throw new ArgumentNullException(nameof(textBox), "TextBox nesmí být null.");

            textBox.PreviewTextInput += TextBox_PreviewTextInput;
            DataObject.AddPastingHandler(textBox, OnPaste);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Text))
            {
                e.Handled = true;
                return;
            }

            foreach (char znak in e.Text)
            {
                if (!povoleneZnaky.Contains(znak))
                {
                    e.Handled = true;
                    return;
                }
            }

            int novaDelka = txtBox.Text.Length - txtBox.SelectionLength + e.Text.Length;
            if (novaDelka > maxDelka)
            {
                e.Handled = true;
            }
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string vkladanyText = (string)e.DataObject.GetData(DataFormats.Text);

                if (!vkladanyText.All(c => povoleneZnaky.Contains(c)))
                {
                    e.CancelCommand();
                    return;
                }

                int novaDelka = txtBox.Text.Length - txtBox.SelectionLength + vkladanyText.Length;
                if (novaDelka > maxDelka)
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        internal override void OsetriVstup(KeyEventArgs e)
        {
            base.OsetriVstup(e);
        }
    }
    internal class OsetreniVstupuCisel : OsetreniVstupu
    {
        internal OsetreniVstupuCisel(TextBox textBox, int maxDelka = 6, string povoleneZnaky = "0123456789")
            : base(textBox, maxDelka, povoleneZnaky)
        {
            textBox.PreviewTextInput += TextBox_PreviewTextInput;
            DataObject.AddPastingHandler(textBox, OnPaste);
        }


        // Pro blokování všeho ostatního než čísla
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Any(c => !char.IsDigit(c)))
            {
                e.Handled = true;
            }

            int novaDelka = txtBox.Text.Length - txtBox.SelectionLength + e.Text.Length;
            if (novaDelka > maxDelka)
            {
                e.Handled = true;
            }
        }

        // Pro ošetření případu kdy uživatel použije CTRL + V
        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                var text = (string)e.DataObject.GetData(DataFormats.Text);
                if (!text.All(char.IsDigit))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        internal override void OsetriVstup(KeyEventArgs e)
        {
           
            base.OsetriVstup(e);
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
