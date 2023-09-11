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
        private string pripojeniDatabaze = "Server=DESKTOP-PHD2MVI;Database=Warehouseapp;User Id=AdminWH;Password=hovno02;";

        public SqlConnection OtevritSpojeni()
        {
            SqlConnection connection = new SqlConnection(pripojeniDatabaze);
            connection.Open();
            return connection;
        }

        public void ZavritSpojeni(SqlConnection connection)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public List<string> ZiskatTypyDodavatelu()
        {
            List<string> typyDodavatelu = new List<string>();

            try
            {
                using (SqlConnection connection = OtevritSpojeni())
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
    }

}
