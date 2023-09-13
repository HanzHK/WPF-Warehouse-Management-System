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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
