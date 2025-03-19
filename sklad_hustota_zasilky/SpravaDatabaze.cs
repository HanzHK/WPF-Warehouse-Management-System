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
using Microsoft.Identity.Client;
using Microsoft.Extensions.Configuration;
using System.IO;
using static system_sprava_skladu.SpravaDatabaze;



namespace system_sprava_skladu
{
    public class SpravaDatabaze
    {


        public class PripojeniDatabazeObecne
        {
            private readonly IConfigurationRoot Configuration;
            private readonly string clientId;
            private readonly string clientSecret;
            private readonly string tenantId;
            private readonly string authority;
            private readonly string pripojeniDatabaze;

            // Konstruktor pro inicializaci jednotlivých instancí
            public PripojeniDatabazeObecne()
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("config/appsettings.private.json");

                Configuration = builder.Build();

                clientId = Configuration["AzureAd:ClientId"];
                clientSecret = Configuration["AzureAd:ClientSecret"];
                tenantId = Configuration["AzureAd:TenantId"];
                authority = $"https://login.microsoftonline.com/{tenantId}";
                pripojeniDatabaze = Configuration["Database:ConnectionString"];
            }


            // Otevře nové spojení
            
            public async Task<SqlConnection> OtevritSpojeniAsync()
            {
                var app = ConfidentialClientApplicationBuilder.Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority(new Uri(authority))
                    .Build();

                // Asynchronní získání tokenu
                var result = await app.AcquireTokenForClient(new[] { "https://database.windows.net/.default" }).ExecuteAsync();
                var accessToken = result.AccessToken;

                // Asynchronní otevření připojení
                SqlConnection connection = new SqlConnection(pripojeniDatabaze)
                {
                    AccessToken = accessToken
                };
                await connection.OpenAsync(); // Asynchronní verze Open()
                return connection;
            }

        }


        public class NacitaniDatzDatabaze
        {

            public async Task<List<string>> ZiskatTypyDodavateluAsync()
            {
                List<string> typyDodavatelu = new List<string>();



                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection connection = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT Nazev FROM dbo.TypyDodavatelu";

                        await using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                        {
                            await using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (await reader.ReadAsync())
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

            // Metoda pro získání seznamu zemí z databáze
            public async Task<List<string>> NactiSeznamZemiZDatabazeAsync()
            {
                List<string> seznamZemi = new List<string>();

                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection connection = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT ZemeID, ZemeNazev FROM dbo.Zeme";

                        await using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                        {
                            await using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
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

                return seznamZemi;
            }


            // Metoda pro naplnění ComboBoxu s typy dodavatelů
            public async Task NaplnComboBoxTypyDodavateluAsync(ComboBox comboBox) 

            {
                List<string> typyDodavatelu = await ZiskatTypyDodavateluAsync();

                foreach (string typDodavatele in typyDodavatelu)
                {
                    comboBox.Items.Add(typDodavatele);
                }
            }

            // Metoda pro naplnění ComboBoxu s názvy zemí
            public async Task NaplnComboBoxZemeAsync(ComboBox comboBox)
            {
                List<string> seznamZemi = await NactiSeznamZemiZDatabazeAsync();

                foreach (string zeme in seznamZemi)
                {
                    comboBox.Items.Add(zeme);
                }
            }

            //  Tahle část řeší načítání adresy dodavatelů do textbloku zobrazujícím adresu
            public async Task <int> ZiskatIdAdresyDodavatele(string nazevDodavatele)
            {
                int adresaID = -1;

                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection connection = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT AdresaID FROM Dodavatele WHERE Nazev = @Nazev";

                        await using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                        {
                            cmd.Parameters.AddWithValue("@Nazev", nazevDodavatele);
                            var result = await cmd.ExecuteScalarAsync();

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


            public async Task NactiAdresu(string vybranyDodavatel, TextBlock uliceTextBlock, TextBlock cisloPopisneTextBlock, TextBlock pscTextBlock, TextBlock obecTextBlock, TextBlock zemeTextBlock)
            {
                try
                {
                    int adresaID = await ZiskatIdAdresyDodavatele(vybranyDodavatel);

                    if (adresaID != -1)
                    {
                        PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                        await using (SqlConnection connection = await pripojeniDatabaze.OtevritSpojeniAsync())
                        {
                            string sqlDotaz = "SELECT Ulice, CisloPopisne, Obec, PSC, ZemeID FROM dbo.AdresyDodavatelu WHERE AdresaID = @AdresaID";
                            await using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                            {
                                cmd.Parameters.AddWithValue("@AdresaID", adresaID);
                                await using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
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
                                            string zemeNazev = await ZiskatNazevZemeAsync(zemeID);
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

            }

            // Metoda pro získání názvu země podle ID
            private async Task <string> ZiskatNazevZemeAsync(int zemeID)
            {
                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection connection = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT ZemeNazev FROM dbo.Zeme WHERE ZemeID = @ZemeID";
                        await using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                        {
                            cmd.Parameters.AddWithValue("@ZemeID", zemeID);
                            object result = await cmd.ExecuteScalarAsync();

                            if (result != null && result != DBNull.Value)
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


            public async Task NactiObecneinformaceAsync(string vybranyDodavatel, Label nazevLabel, TextBlock icoTextBlock, TextBlock dicTextBlock)
            {
                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection connection = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT Nazev, ICO, DIC FROM Dodavatele WHERE Nazev = @Nazev";
                        await using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                        {
                            cmd.Parameters.AddWithValue("@Nazev", vybranyDodavatel);
                            await using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
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

            }



            //  Tahle část řeší načítání názvu dodavatele do seznamu dostupných dodavatelů

            public ObservableCollection<string> SeznamDodavatelu { get; set; } = new ObservableCollection<string>();
            public async Task NaplnComboBoxDodavateluAsync(ComboBox comboBox)
            {
                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection connection = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT Nazev FROM dbo.Dodavatele";

                        await using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                        {
                            await using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
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
                comboBox.ItemsSource = SeznamDodavatelu;
            }


        }
        //
        //  Část řešící vkládání dat do databáze - Přídání dodavatele
        //

        public class VlozdoDatabazeNovyDodavatel
        {

            // Metoda pro nalezení id Země z databáze a vrácení její hodnoty
            public async Task <int> ZiskatIdZemeAsync(string zemeNazev)
            {
                int id = -1;

                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection connection = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT ZemeID FROM dbo.Zeme WHERE ZemeNazev = @ZemeNazev";

                        await using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                        {
                            cmd.Parameters.AddWithValue("@ZemeNazev", zemeNazev);
                            object result = await cmd.ExecuteScalarAsync();

                            if (result != null && result != DBNull.Value)
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

            //Metoda pro uložení nobého dodavatele do databáze
            public async Task UlozitDodavatele(string nazev, string ico, string dic, string popis, string typDodavatele, string ulice, string cisloPopisne, string psc, string obec, string zeme)
            {
                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection connection = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        // Získání ZemeID pro vybranou zemi
                        int zemeID = await ZiskatIdZemeAsync(zeme);

                        // Nejprve vložte adresu do tabulky AdresyDodavatelu
                        string sqlAdresaDotaz = "INSERT INTO AdresyDodavatelu (Ulice, CisloPopisne, PSC, Obec, ZemeID) VALUES (@Ulice, @CisloPopisne, @PSC, @Obec, @ZemeID); SELECT SCOPE_IDENTITY();";


                        await using (SqlCommand adresaCmd = new SqlCommand(sqlAdresaDotaz, connection))
                        {
                            adresaCmd.Parameters.AddWithValue("@Ulice", ulice);
                            adresaCmd.Parameters.AddWithValue("@CisloPopisne", cisloPopisne);
                            adresaCmd.Parameters.AddWithValue("@PSC", psc);
                            adresaCmd.Parameters.AddWithValue("@Obec", obec);
                            adresaCmd.Parameters.AddWithValue("@ZemeID", zemeID);


                            // Získání ID nově vložené adresy
                            int adresaID = Convert.ToInt32(await adresaCmd.ExecuteScalarAsync());

                            int typDodavateleID = await ZiskatIdTypuDodavateleAsync(typDodavatele);
                            string sqlDodavatelDotaz = "INSERT INTO Dodavatele (Nazev, ICO, DIC, Popis, TypDodavateleID, AdresaID) VALUES (@Nazev, @ICO, @DIC, @Popis, @TypDodavateleID, @AdresaID)";

                            await using (SqlCommand dodavatelCmd = new SqlCommand(sqlDodavatelDotaz, connection))
                            {
                                dodavatelCmd.Parameters.AddWithValue("@Nazev", nazev);
                                dodavatelCmd.Parameters.AddWithValue("@ICO", ico);
                                dodavatelCmd.Parameters.AddWithValue("@DIC", dic);
                                dodavatelCmd.Parameters.AddWithValue("@Popis", popis);
                                dodavatelCmd.Parameters.AddWithValue("@TypDodavateleID", typDodavateleID);
                                dodavatelCmd.Parameters.AddWithValue("@AdresaID", adresaID);
                                await dodavatelCmd.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    // Po úspěšném uložení zobrazí vyskakovací okno
                    MessageBox.Show("Data byla úspěšně uložena.", "Úspěch", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při ukládání do databáze: " + ex.Message);
                }

            }
            // Metoda pro získání id typu dodavatele (as., s.r.o., fyzická osoba atd.)
            public async Task <int> ZiskatIdTypuDodavateleAsync(string nazevTypu)
            {
                int id = -1; // Defaultní hodnota v případě, že se ID nepodaří najít.

                try
                {
                    // Otevření spojení s databází.
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection connection = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT TypDodavateleID FROM dbo.TypyDodavatelu WHERE Nazev = @Nazev";

                        // Vytvoření a konfigurace SQL příkazu.
                        await using (SqlCommand cmd = new SqlCommand(sqlDotaz, connection))
                        {
                            // Přidání parametru @Nazev do SQL příkazu.
                            cmd.Parameters.AddWithValue("@Nazev", nazevTypu);

                            // Provedení SQL příkazu a získání jednoho výsledku (první sloupce prvního řádku).
                            object result = await cmd.ExecuteScalarAsync();

                            // Pokud byl nalezen výsledek, přiřaďá jej k proměnné id.
                            if (result != null && result != DBNull.Value)
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

                // Vrátí ID typu dodavatele.
                return id;
            }

        }
    }
}


