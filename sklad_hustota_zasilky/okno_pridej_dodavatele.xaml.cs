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
using static system_sprava_skladu.OsetreniVstupu;
using MahApps.Metro.Controls;


namespace system_sprava_skladu
{
    /// <summary>
    /// Interakční logika pro okno_pridej_dodavatele.xaml
    /// </summary>
    public partial class okno_pridej_dodavatele : UserControl
    {
        


        private OsetreniVstupuCisel osetreniIco;
        private OsetreniVstupuCisel osetreniDic;
        private OsetreniVstupuCisel osetreniCisloPopisne;
        private OsetreniVstupuCisel osetreniPsc;
        private OsetreniVstupuTextChanged osetreniPscTextChanged;

        SpravaDatabaze.NacitaniDatzDatabaze nacitani = new SpravaDatabaze.NacitaniDatzDatabaze();

        public okno_pridej_dodavatele()
        {
            InitializeComponent();

            // Instance pro validaci
            osetreniIco = new OsetreniVstupuCisel(txtBoxIco, 8); 
            osetreniDic = new OsetreniVstupuCisel(txtBoxDic, 8); 
            osetreniCisloPopisne = new OsetreniVstupuCisel(txtBoxCisloPopisne, 10); 
            osetreniPsc = new OsetreniVstupuCisel(txtBoxPsc, 6); 
            osetreniPscTextChanged = new OsetreniVstupuTextChanged(txtBoxPsc, 7);

            InicializujOknoAsync();
                        
        }
        private async void InicializujOknoAsync()
        {
            // Naplnění typy dodavatelů
            try
            {
                await nacitani.NaplnComboBoxTypyDodavateluAsync(cBoxTypyDodavatelu);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání typů dodavatelů " + ex.Message);
            }

            try
            {
                await nacitani.NaplnComboBoxZemeAsync(cBoxZeme);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání zemí " + ex.Message);
            }
        }

        private void txtBoxIco_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            osetreniIco.OsetriVstup(e);
        }
        private void txtBoxDic_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            osetreniDic.OsetriVstup(e);
        }
        private void txtBoxCisloPopisne_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            osetreniCisloPopisne.OsetriVstup(e);
        }
        private void txtBoxPsc_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            osetreniPsc.OsetriVstup(e);
        }
        private void txtBoxPsc_TextChanged(object sender, TextChangedEventArgs e)
       {
            osetreniPscTextChanged.OsetriVstup(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, Key.Enter));
        }


        private async void pridatDodavateleDbButton_Click(object sender, RoutedEventArgs e)
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

            // Vytvoření instance třídy VlozdoDatabazeNovyDodavatel
            SpravaDatabaze.VlozdoDatabazeNovyDodavatel pridejDodavatele = new SpravaDatabaze.VlozdoDatabazeNovyDodavatel();

            // Volání metody pro uložení dodavatele
           await pridejDodavatele.UlozitDodavatele(nazev, ico, dic, popis, typDodavatele, ulice, cislopopisne, psc, obec, zeme);

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



    }

    }
