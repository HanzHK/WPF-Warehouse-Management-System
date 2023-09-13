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

        // Nově přidaná metoda pro naplnění ComboBoxu s typy dodavatelů
        public void NaplnComboBoxTypyDodavatelu(ComboBox comboBox)
        {
            List<string> typyDodavatelu = ZiskatTypyDodavatelu();

            foreach (string typDodavatele in typyDodavatelu)
            {
                comboBox.Items.Add(typDodavatele);
            }
        }
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

        public class PridejDodavateleSql
        {
            public void UlozitDodavatele(string nazev, string ico, string dic, string popis, string typDodavatele)
            {
                try
                {
                    using (SqlConnection connection = PripojeniDatabazeObecne.OtevritSpojeni())
                    {
                        string sqlDotaz = "INSERT INTO Dodavatele (Nazev, ICO, DIC, Popis, TypDodavateleID) VALUES (@Nazev, @ICO, @DIC, @Popis, @TypDodavateleID)";

                        using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                        {
                            cmd.Parameters.AddWithValue("@Nazev", nazev);
                            cmd.Parameters.AddWithValue("@ICO", ico);
                            cmd.Parameters.AddWithValue("@DIC", dic);
                            cmd.Parameters.AddWithValue("@Popis", popis);
                            cmd.Parameters.AddWithValue("@TypDodavateleID", ZiskatIdTypuDodavatele(typDodavatele));

                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Dodavatel byl úspěšně uložen do databáze.");
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
