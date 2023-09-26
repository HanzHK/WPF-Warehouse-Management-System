using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
                    string sqlDotaz = "SELECT Nazev FROM dbo.Zeme";

                    using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                seznamZemi.Add(reader["Nazev"].ToString());
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
                                    comboBox.Items.Add(reader["Nazev"].ToString());
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
            }

           
        }

        //
        //  Část řešící vkládání dat do databáze - Přídání dodavatele
        //

        public class PridejDodavateleSql
        {

            public void UlozitDodavatele(string nazev, string ico, string dic, string popis, string typDodavatele, string ulice, string cisloPopisne, string psc, string obec, string zeme)
            {
                try
                {
                    using (SqlConnection connection = PripojeniDatabazeObecne.OtevritSpojeni())
                    {
                        // Nejprve vložte adresu do tabulky AdresyDodavatelu
                        string sqlAdresaDotaz = "INSERT INTO AdresyDodavatelu (Ulice, CisloPopisne, PSC, Obec, Zeme) VALUES (@Ulice, @CisloPopisne, @PSC, @Obec, @Zeme); SELECT SCOPE_IDENTITY();";

                        using (SqlCommand adresaCmd = new SqlCommand(sqlAdresaDotaz, connection))
                        {
                            adresaCmd.Parameters.AddWithValue("@Ulice", ulice);
                            adresaCmd.Parameters.AddWithValue("@CisloPopisne", cisloPopisne);
                            adresaCmd.Parameters.AddWithValue("@PSC", psc);
                            adresaCmd.Parameters.AddWithValue("@Obec", obec);
                            adresaCmd.Parameters.AddWithValue("@Zeme", zeme);

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
