using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace sklad_hustota_zasilky
{
    public class SpravaDatabaze
    {

        #region Obecné
        public class PripojeniDatabazeObecne
        {
            private static string pripojeniDatabaze = "Server=DESKTOP-PHD2MVI;Database=Warehouseapp;User Id=AdminWH;Password=hovno02;";
            public static SqlConnection Connection { get; private set; }

            public static SqlConnection OtevritSpojeni()
            {
                Connection = new SqlConnection(pripojeniDatabaze);
                Connection.Open();
                return Connection;
            }

            public static void ZavritSpojeni()
            {
                if (Connection != null && Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }

            }
        }
        #endregion

        private okno_pridej_zasilku OknoPridejZasilku;

        public List<string> ZiskatTypyDodavatelu()
        {
            List<string> typyDodavatelu = new List<string>();



            try
            {
                using (SqlConnection connection = PripojeniDatabazeObecne.OtevritSpojeni())
                {
                    string sqlDotaz = "SELECT Nazev FROM dbo.TypyDodavatelu";

                    using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                typyDodavatelu.Add(reader["Nazev"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání typů dodavatelů: " + ex.Message);
            }
            finally
            {
                PripojeniDatabazeObecne.ZavritSpojeni();
            }

            return typyDodavatelu;
        }

        // Metoda pro získání seznamu zemí z databáze
        public List<string> NactiSeznamZemiZDatabaze()
        {
            List<string> seznamZemi = new List<string>();

            try
            {
                using (SqlConnection connection = PripojeniDatabazeObecne.OtevritSpojeni())
                {
                    string sqlDotaz = "SELECT ZemeID, ZemeNazev FROM dbo.Zeme";

                    using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                seznamZemi.Add(reader["ZemeNazev"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání seznamu zemí: " + ex.Message);
            }
            finally
            {
                PripojeniDatabazeObecne.ZavritSpojeni();
            }

            return seznamZemi;
        }


        // Metoda pro naplnění ComboBoxu s typy dodavatelů
        public void NaplnComboBoxTypyDodavatelu(ComboBox comboBox)
        {
            List<string> typyDodavatelu = ZiskatTypyDodavatelu();

            foreach (string typDodavatele in typyDodavatelu)
            {
                comboBox.Items.Add(typDodavatele);
            }
        }

        // Metoda pro naplnění ComboBoxu s názvy zemí
        public void NaplnComboBoxZeme(ComboBox comboBox)
        {
            List<string> seznamZemi = NactiSeznamZemiZDatabaze();

            foreach (string zeme in seznamZemi)
            {
                comboBox.Items.Add(zeme);
            }
        }
        //
        // Část řěšící načítání dat z databáze
        //
        public class NacitaniDatzDatabaze
        {
            //  Tahle část řeší načítání adresy dodavatelů do textbloku zobrazujícím adresu
            public int ZiskatIdAdresyDodavatele(string nazevDodavatele)
            {
                int adresaID = -1;

                try
                {
                    using (SqlConnection connection = PripojeniDatabazeObecne.OtevritSpojeni())
                    {
                        string sqlDotaz = "SELECT AdresaID FROM Dodavatele WHERE Nazev = @Nazev";

                        using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                        {
                            cmd.Parameters.AddWithValue("@Nazev", nazevDodavatele);
                            var result = cmd.ExecuteScalar();

                            if (result != null && result != DBNull.Value)
                            {
                                adresaID = Convert.ToInt32(result);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při získávání ID adresy dodavatele: " + ex.Message);
                }

                return adresaID;
            }


            public void NactiAdresu(string vybranyDodavatel, TextBlock uliceTextBlock, TextBlock cisloPopisneTextBlock, TextBlock pscTextBlock, TextBlock obecTextBlock, TextBlock zemeTextBlock)
            {
                try
                {
                    int adresaID = ZiskatIdAdresyDodavatele(vybranyDodavatel);

                    if (adresaID != -1)
                    {
                        using (SqlConnection connection = PripojeniDatabazeObecne.OtevritSpojeni())
                        {
                            string sqlDotaz = "SELECT Ulice, CisloPopisne, Obec, PSC, ZemeID FROM dbo.AdresyDodavatelu WHERE AdresaID = @AdresaID";
                            using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                            {
                                cmd.Parameters.AddWithValue("@AdresaID", adresaID);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        uliceTextBlock.Text = reader["Ulice"].ToString();
                                        cisloPopisneTextBlock.Text = reader["CisloPopisne"].ToString();
                                        obecTextBlock.Text = reader["Obec"].ToString();
                                        pscTextBlock.Text = reader["PSC"].ToString();

                                        if (reader["ZemeID"] != DBNull.Value)
                                        {
                                            int zemeID = Convert.ToInt32(reader["ZemeID"]);
                                            string zemeNazev = ZiskatNazevZeme(zemeID);
                                            zemeTextBlock.Text = zemeNazev;
                                        }
                                        else
                                        {
                                            zemeTextBlock.Text = "N/A"; // nebo jiný výchozí text pro případ DBNull.Value
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při načítání adresy dodavatele: " + ex.Message);
                }
                finally
                {
                    PripojeniDatabazeObecne.ZavritSpojeni();
                }
            }

            // Metoda pro získání názvu země podle ID
            private string ZiskatNazevZeme(int zemeID)
            {
                try
                {
                    using (SqlConnection connection = PripojeniDatabazeObecne.OtevritSpojeni())
                    {
                        string sqlDotaz = "SELECT ZemeNazev FROM dbo.Zeme WHERE ZemeID = @ZemeID";
                        using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                        {
                            cmd.Parameters.AddWithValue("@ZemeID", zemeID);
                            object result = cmd.ExecuteScalar();

                            if (result != null)
                            {
                                return result.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při získávání názvu země: " + ex.Message);
                }

                return string.Empty;
            }


            public void NactiObecneinformace(string vybranyDodavatel, Label nazevLabel, TextBlock icoTextBlock, TextBlock dicTextBlock)
            {
                try
                {
                    using (SqlConnection connection = PripojeniDatabazeObecne.OtevritSpojeni())
                    {
                        string sqlDotaz = "SELECT Nazev, ICO, DIC FROM Dodavatele WHERE Nazev = @Nazev";
                        using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                        {
                            cmd.Parameters.AddWithValue("@Nazev", vybranyDodavatel);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    nazevLabel.Content = reader["Nazev"].ToString();
                                    icoTextBlock.Text = reader["ICO"].ToString();
                                    dicTextBlock.Text = reader["DIC"].ToString();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při načítání informací o dodavateli: " + ex.Message);
                }
                finally
                {
                    PripojeniDatabazeObecne.ZavritSpojeni();
                }
            }


            //
            //  Tahle část řeší načítání názvu dodavatele do seznamu dostupných dodavatelů
            //
            public ObservableCollection<string> SeznamDodavatelu { get; set; } = new ObservableCollection<string>();
            public void NaplnComboBoxDodavatelu(ComboBox comboBox)
            {
                try
                {
                    using (SqlConnection connection = PripojeniDatabazeObecne.OtevritSpojeni())
                    {
                        string sqlDotaz = "SELECT Nazev FROM dbo.Dodavatele";

                        using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string nazevDodavatele = reader["Nazev"].ToString();

                                    // Kontrola, zda se dodavatel již nachází v kolekci
                                    if (!SeznamDodavatelu.Contains(nazevDodavatele))
                                    {
                                        SeznamDodavatelu.Add(nazevDodavatele);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při načítání dodavatelů: " + ex.Message);
                }
                finally
                {
                    PripojeniDatabazeObecne.ZavritSpojeni();
                }
                comboBox.ItemsSource = SeznamDodavatelu;
            }


        }

        //
        //  Část řešící vkládání dat do databáze - Přídání dodavatele
        //

        public class PridejDodavateleSql
        {
            public int ZiskatIdZeme(string zemeNazev)
            {
                int id = -1;

                try
                {
                    using (SqlConnection connection = PripojeniDatabazeObecne.OtevritSpojeni())
                    {
                        string sqlDotaz = "SELECT ZemeID FROM dbo.Zeme WHERE ZemeNazev = @ZemeNazev";

                        using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                        {
                            cmd.Parameters.AddWithValue("@ZemeNazev", zemeNazev);
                            object result = cmd.ExecuteScalar();

                            if (result != null)
                            {
                                id = (int)result;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při získávání ID země: " + ex.Message);
                }

                return id;
            }


            public void UlozitDodavatele(string nazev, string ico, string dic, string popis, string typDodavatele, string ulice, string cisloPopisne, string psc, string obec, string zeme)
            {
                try
                {
                    using (SqlConnection connection = PripojeniDatabazeObecne.OtevritSpojeni())
                    {
                        // Získání ZemeID pro vybranou zemi
                        int zemeID = ZiskatIdZeme(zeme);

                        // Nejprve vložte adresu do tabulky AdresyDodavatelu
                        string sqlAdresaDotaz = "INSERT INTO AdresyDodavatelu (Ulice, CisloPopisne, PSC, Obec, ZemeID) VALUES (@Ulice, @CisloPopisne, @PSC, @Obec, @ZemeID); SELECT SCOPE_IDENTITY();";


                        using (SqlCommand adresaCmd = new SqlCommand(sqlAdresaDotaz, connection))
                        {
                            adresaCmd.Parameters.AddWithValue("@Ulice", ulice);
                            adresaCmd.Parameters.AddWithValue("@CisloPopisne", cisloPopisne);
                            adresaCmd.Parameters.AddWithValue("@PSC", psc);
                            adresaCmd.Parameters.AddWithValue("@Obec", obec);
                            adresaCmd.Parameters.AddWithValue("@ZemeID", ZiskatIdZeme(zeme));


                            // Získání ID nově vložené adresy
                            int adresaID = Convert.ToInt32(adresaCmd.ExecuteScalar());

                            // Následně vložte dodavatele a přiřaďte mu adresu
                            string sqlDodavatelDotaz = "INSERT INTO Dodavatele (Nazev, ICO, DIC, Popis, TypDodavateleID, AdresaID) VALUES (@Nazev, @ICO, @DIC, @Popis, @TypDodavateleID, @AdresaID)";

                            using (SqlCommand dodavatelCmd = new SqlCommand(sqlDodavatelDotaz, connection))
                            {
                                dodavatelCmd.Parameters.AddWithValue("@Nazev", nazev);
                                dodavatelCmd.Parameters.AddWithValue("@ICO", ico);
                                dodavatelCmd.Parameters.AddWithValue("@DIC", dic);
                                dodavatelCmd.Parameters.AddWithValue("@Popis", popis);
                                dodavatelCmd.Parameters.AddWithValue("@TypDodavateleID", ZiskatIdTypuDodavatele(typDodavatele));
                                dodavatelCmd.Parameters.AddWithValue("@AdresaID", adresaID);
                                dodavatelCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    // Po úspěšném uložení zobrazte vyskakovací okno
                    MessageBox.Show("Data byla úspěšně uložena.", "Úspěch", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při ukládání do databáze: " + ex.Message);
                }
                finally
                {
                    PripojeniDatabazeObecne.ZavritSpojeni();
                }
            }

            public int ZiskatIdTypuDodavatele(string nazevTypu)
            {
                int id = -1; // Defaultní hodnota v případě, že se ID nepodaří najít.

                try
                {
                    // Otevření spojení s databází.
                    using (SqlConnection connection = PripojeniDatabazeObecne.OtevritSpojeni())
                    {
                        string sqlDotaz = "SELECT TypDodavateleID FROM dbo.TypyDodavatelu WHERE Nazev = @Nazev";

                        // Vytvoření a konfigurace SQL příkazu.
                        using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                        {
                            // Přidání parametru @Nazev do SQL příkazu.
                            cmd.Parameters.AddWithValue("@Nazev", nazevTypu);

                            // Provedení SQL příkazu a získání jednoho výsledku (první sloupce prvního řádku).
                            object result = cmd.ExecuteScalar();

                            // Pokud byl nalezen výsledek, přiřaďte jej k proměnné id.
                            if (result != null)
                            {
                                id = (int)result;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Zpráva o chybě v případě výjimky.
                    MessageBox.Show("Chyba při získávání ID typu dodavatele: " + ex.Message);
                }

                // Vrátit ID typu dodavatele.
                return id;
            }

        }
    }

}
