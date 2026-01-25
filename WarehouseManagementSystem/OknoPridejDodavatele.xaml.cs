using Serilog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace system_sprava_skladu
{
    /// <summary>
    /// Interakční logika pro OknoPridejDodavatele.xaml
    /// </summary>
    public partial class OknoPridejDodavatele : UserControl
    {
        


        private readonly OsetreniVstupuCisel osetreniIco;
        private readonly OsetreniVstupuCisel osetreniDic;
        private readonly OsetreniVstupuCisel osetreniCisloPopisne;
        private readonly OsetreniVstupuCisel osetreniPsc;
        private readonly OsetreniVstupuTextChanged osetreniPscTextChanged;
        private readonly SpravaDatabaze.NacitaniDatzDatabaze nacitani = new();

        public OknoPridejDodavatele()
        {
            InitializeComponent();

            // Instance pro validaci
            osetreniIco = new OsetreniVstupuCisel(TxtBoxIco, 8); 
            osetreniDic = new OsetreniVstupuCisel(TxtBoxDic, 8); 
            osetreniCisloPopisne = new OsetreniVstupuCisel(TxtBoxCisloPopisne, 10); 
            osetreniPsc = new OsetreniVstupuCisel(TxtBoxPsc, 6); 
            osetreniPscTextChanged = new OsetreniVstupuTextChanged(TxtBoxPsc, 7);

            InicializujOknoAsync();
                        
        }
        private async void InicializujOknoAsync()
        {
            // Naplnění typy dodavatelů
            try
            {
                await nacitani.NaplnComboBoxTypyDodavateluAsync(CboxTypyDodavatelu);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání typů dodavatelů " + ex.Message);
                Log.Error(ex, "Chyba při načítání typů dodavatelů");
            }

            try
            {
                await nacitani.NaplnComboBoxZemeAsync(CboxZeme);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání zemí " + ex.Message);
                Log.Error(ex, "Chyba při načítání zemí");
            }
        }

        private void TxtBoxIco_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            osetreniIco.OsetriVstup(e);
        }
        private void TxtBoxDic_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            osetreniDic.OsetriVstup(e);
        }
        private void TxtBoxCisloPopisne_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            osetreniCisloPopisne.OsetriVstup(e);
        }
        private void TxtBoxPsc_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            osetreniPsc.OsetriVstup(e);
        }
        private void TxtBoxPsc_TextChanged(object sender, TextChangedEventArgs e)
       {
            osetreniPscTextChanged.OsetriVstup(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, Key.Enter));
        }


        private async void PridatDodavateleDbButton_Click(object sender, RoutedEventArgs e)
        {
            // Obecné - přiřazení hodnot do proměnných
            string nazev = TxtBoxNazevDodavatele.Text;
            string ico = TxtBoxIco.Text;
            string dic = TxtBoxDic.Text;
            string popis = TxtBoxPopis.Text;
            string typDodavatele = CboxTypyDodavatelu.SelectedItem.ToString() ?? string.Empty;

            // Adresa - přiřazení hodnot do proměnných
            string ulice = TxtBoxUlice.Text;
            string cislopopisne = TxtBoxCisloPopisne.Text;
            string psc = TxtBoxPsc.Text;
            string obec = TxtBoxObec.Text;
            string zeme = CboxZeme.SelectedItem.ToString() ?? string.Empty;

            // Vytvoření instance třídy VlozdoDatabazeNovyDodavatel
            SpravaDatabaze.VlozdoDatabazeNovyDodavatel pridejDodavatele = new();

            // Volání metody pro uložení dodavatele
           await pridejDodavatele.UlozitDodavatele(nazev, ico, dic, popis, typDodavatele, ulice, cislopopisne, psc, obec, zeme);

            // Aktualizace uživatelského rozhraní - vyčištění polí
            TxtBoxNazevDodavatele.Clear();
            TxtBoxIco.Clear();
            TxtBoxDic.Clear();
            TxtBoxPopis.Clear();
            TxtBoxUlice.Clear();
            TxtBoxObec.Clear();
            TxtBoxPsc.Clear();
            TxtBoxCisloPopisne.Clear();

        }



    }

    }
