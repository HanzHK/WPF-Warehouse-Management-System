using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;




namespace system_sprava_skladu
{
    internal class SpravaDatabaze
    {


        internal class PripojeniDatabazeObecne
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

                clientId = Configuration["AzureAd:ClientId"] ?? throw new ArgumentNullException("ClientId is not defined in the configuration.");
                clientSecret = Configuration["AzureAd:ClientSecret"] ?? throw new ArgumentNullException("ClientSecret is not defined in the configuration.");
                tenantId = Configuration["AzureAd:TenantId"] ?? throw new ArgumentNullException("TenantId is not defined in the configuration.");
                authority = $"https://login.microsoftonline.com/{tenantId}";
                pripojeniDatabaze = Configuration["Database:ConnectionString"] ?? throw new ArgumentNullException("ConnectionString is not defined in the configuration.");

            }
            // Asynchronní připojení k databázi
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
                SqlConnection pripojeni = new(pripojeniDatabaze)
                {
                    AccessToken = accessToken
                };
                await pripojeni.OpenAsync(); // Asynchronní verze Open()
                return pripojeni;
            }
            // Synchronní připojení
            private bool OtevritSpojeni()
            {
                try
                {
                    var app = ConfidentialClientApplicationBuilder.Create(clientId)
                        .WithClientSecret(clientSecret)
                        .WithAuthority(new Uri(authority))
                        .Build();

                    var result = app.AcquireTokenForClient(new[] { "https://database.windows.net/.default" }).ExecuteAsync().Result;
                    var accessToken = result.AccessToken;
                    var builder = new SqlConnectionStringBuilder(pripojeniDatabaze)
                    {
                        ConnectTimeout = 120 
                    };

                    using (SqlConnection pripojeni = new(pripojeniDatabaze)
                    {
                        AccessToken = accessToken
                    })
                    {
                        pripojeni.Open();
                        if (pripojeni.State == System.Data.ConnectionState.Open)
                        {
                            return true; // úspěšné připojení
                        }
                        else
                        {
                            return false; // neúspěšné připojení
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Chyba při připojení");
                    return false;

                }
            }
            public bool ProbuzeniDatabaze()
            {
                return OtevritSpojeni();
            }

        }
        internal class ValidaceDatabaze
        {
            public static T ZkontrolovatNull<T>(object vysledek, T defaultHodnota)
            {
                if (vysledek != null && vysledek != DBNull.Value)
                {
                    try
                    {
                        return (T)Convert.ChangeType(vysledek, typeof(T));
                    }
                    catch
                    {
                        return defaultHodnota;
                    }
                }

                return defaultHodnota;
            }

        }
        internal class NacitaniDatzDatabaze
        {
            private static async Task<List<string>> ZiskatTypyDodavateluAsync()
            {
                List<string> typyDodavatelu = new List<string>();



                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new();

                    await using (SqlConnection pripojeni = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT Nazev FROM dbo.TypyDodavatelu";

                        await using (SqlCommand cmd = new SqlCommand(sqlDotaz, pripojeni))
                        {
                            await using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (await reader.ReadAsync())
                                {
                                    object vysledek = reader["Nazev"] ?? DBNull.Value;
                                    string nazevTypu = ValidaceDatabaze.ZkontrolovatNull(vysledek, string.Empty);

                                    if (!string.IsNullOrEmpty(nazevTypu))
                                    {
                                        typyDodavatelu.Add(nazevTypu);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při načítání typů dodavatelů: " + ex.Message);
                    Log.Error(ex, "Chyba při načítání typů dodavatelů");

                }

                return typyDodavatelu;
            }
            // Metoda pro získání seznamu zemí z databáze
            private async Task<List<string>> NactiSeznamZemiZDatabazeAsync()
            {
                List<string> seznamZemi = new();

                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new();

                    await using (SqlConnection pripojeni = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT ZemeID, ZemeNazev FROM dbo.Zeme";

                        await using (SqlCommand cmd = new SqlCommand(sqlDotaz, pripojeni))
                        {
                            await using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    object vysledek = reader["ZemeNazev"];
                                    string nazevZeme = ValidaceDatabaze.ZkontrolovatNull(vysledek, string.Empty);
                                    if (!string.IsNullOrEmpty(nazevZeme))
                                    {
                                        seznamZemi.Add(nazevZeme);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při načítání seznamu zemí: " + ex.Message);
                    Log.Error(ex, "Chyba při načítání seznamu zemí");
                }

                return seznamZemi;
            }
            // Metoda pro naplnění ComboBoxu s typy dodavatelů
            private async Task NaplnComboBoxTypyDodavateluPrivatAsync(ComboBox comboBox)

            {
                List<string> typyDodavatelu = await ZiskatTypyDodavateluAsync();

                foreach (string typDodavatele in typyDodavatelu)
                {
                    comboBox.Items.Add(typDodavatele);
                }
            }
            internal async Task NaplnComboBoxTypyDodavateluAsync(ComboBox comboBox)
            {
                await NaplnComboBoxTypyDodavateluPrivatAsync(comboBox);
            }
            // Metoda pro naplnění ComboBoxu s názvy zemí
            private async Task NaplnComboBoxZemePrivateAsync(ComboBox comboBox)
            {
                List<string> seznamZemi = await NactiSeznamZemiZDatabazeAsync();

                foreach (string zeme in seznamZemi)
                {
                    comboBox.Items.Add(zeme);
                }
            }
            internal async Task NaplnComboBoxZemeAsync(ComboBox comboBox)
            {
                await NaplnComboBoxZemePrivateAsync(comboBox);
            }
            //  Tahle část řeší načítání adresy dodavatelů do textbloku zobrazujícím adresu
            private static async Task<int> ZiskatIdAdresyDodavatele(string nazevDodavatele)
            {
                int adresaID = -1;

                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new();

                    await using (SqlConnection pripojeni = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT AdresaID FROM Dodavatele WHERE Nazev = @Nazev";

                        await using (SqlCommand cmd = new(sqlDotaz, pripojeni))
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
                    Log.Error(ex, "Chyba při získávání ID adresy dodavatele");
                }

                return adresaID;
            }
            // Metoda pro načtení adres dodavatelů
            private async Task NactiAdresuPrivate(string vybranyDodavatel, TextBlock uliceTextBlock, TextBlock cisloPopisneTextBlock, TextBlock pscTextBlock, TextBlock obecTextBlock, TextBlock zemeTextBlock)
            {
                try
                {
                    int adresaID = await ZiskatIdAdresyDodavatele(vybranyDodavatel);

                    if (adresaID != -1)
                    {
                        PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                        await using (SqlConnection pripojeni = await pripojeniDatabaze.OtevritSpojeniAsync())
                        {
                            string sqlDotaz = "SELECT Ulice, CisloPopisne, Obec, PSC, ZemeID FROM dbo.AdresyDodavatelu WHERE AdresaID = @AdresaID";
                            await using (SqlCommand cmd = new(sqlDotaz, pripojeni))
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
                    Log.Error(ex, "Chyba při načítání adresy dodavatele");
                }

            }
            internal async Task NactiAdresu(string vybranyDodavatel, TextBlock uliceTextBlock, TextBlock cisloPopisneTextBlock, TextBlock pscTextBlock, TextBlock obecTextBlock, TextBlock zemeTextBlock)
            {
                await NactiAdresuPrivate(vybranyDodavatel, uliceTextBlock, cisloPopisneTextBlock, pscTextBlock, obecTextBlock, zemeTextBlock);
            }
            // Metoda pro získání názvu země podle ID
            private static async Task<string> ZiskatNazevZemeAsync(int zemeID)
            {
                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection pripojeni = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT ZemeNazev FROM dbo.Zeme WHERE ZemeID = @ZemeID";
                        await using (SqlCommand cmd = new SqlCommand(sqlDotaz, pripojeni))
                        {
                            cmd.Parameters.AddWithValue("@ZemeID", zemeID);
                            object vysledek = await cmd.ExecuteScalarAsync() ?? DBNull.Value;

                            if (vysledek != null && vysledek != DBNull.Value)
                            {
                                return ValidaceDatabaze.ZkontrolovatNull(vysledek, string.Empty);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při získávání názvu země: " + ex.Message);
                    Log.Error(ex, "Chyba při získávání názvu země");
                }

                return string.Empty;
            }
            // Metoda pro Načtení oebecných informací o dodavateli
            internal async Task NactiObecneinformaceAsync(string vybranyDodavatel, Label nazevLabel, TextBlock icoTextBlock, TextBlock dicTextBlock)
            {
                await NactiObecneinformacePrivateAsync(vybranyDodavatel, nazevLabel, icoTextBlock, dicTextBlock);
            }
            private async Task NactiObecneinformacePrivateAsync(string vybranyDodavatel, Label nazevLabel, TextBlock icoTextBlock, TextBlock dicTextBlock)
            {
                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection pripojeni = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT Nazev, ICO, DIC FROM Dodavatele WHERE Nazev = @Nazev";
                        await using (SqlCommand cmd = new(sqlDotaz, pripojeni))
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
                    Log.Error(ex, "Chyba při načítání informací o dodavateli");
                }

            }
            //  Tahle část řeší načítání názvu dodavatele do seznamu dostupných dodavatelů
            private ObservableCollection<string> SeznamDodavatelu { get; set; } = new ObservableCollection<string>();
            internal async Task NaplnComboBoxDodavateluAsync(ComboBox comboBox)
            {
                await NaplnComboBoxDodavateluPrivateAsync(comboBox);
            }
            private async Task NaplnComboBoxDodavateluPrivateAsync(ComboBox comboBox)
            {
                try
                {
                    SeznamDodavatelu.Clear();
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection pripojeni = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT Nazev FROM dbo.Dodavatele";

                        await using (SqlCommand cmd = new(sqlDotaz, pripojeni))
                        {
                            await using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                while (reader.Read())
                                {
                                    object vysledek = reader["Nazev"] ?? DBNull.Value;
                                    string nazevDodavatele = ValidaceDatabaze.ZkontrolovatNull(vysledek, string.Empty);

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
                    Log.Error(ex, "Chyba při načítání dodavatelů");
                }
                comboBox.ItemsSource = SeznamDodavatelu;
            }

        }
        internal class NacitaniDatzDatabazeSkladovaciPozice
        {
            public class SkladovaciPoziceDTO
            {
                public int Id { get; set; }
                public required string Nazev { get; set; }
            }
            internal async Task<List<SkladovaciPoziceDTO>> NactiSkladovaciPoziceAsync()
            {
              return await NactiSkladovaciPozicePrivateAsync();
            }
            private async Task<List<SkladovaciPoziceDTO>> NactiSkladovaciPozicePrivateAsync()
            {
                PripojeniDatabazeObecne pripojeniDatabaze = new();
                List<SkladovaciPoziceDTO> skladovaciPozice = new();

                try
                {
                    using (SqlConnection pripojeni = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string query = "SELECT skladovaciPoziceID, skladovaciPoziceNazev FROM SkladovaciPozice";

                        using (SqlCommand command = new(query, pripojeni))
                        {
                            using (SqlDataReader reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    // Explicitní přidání položek do seznamu
                                    SkladovaciPoziceDTO pozice = new SkladovaciPoziceDTO
                                    {
                                        Id = reader.GetInt32(0),
                                        Nazev = reader.GetString(1)
                                    };

                                    skladovaciPozice.Add(pozice);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Chyba při načítání skladovacích pozic: {ex.Message}");
                    Log.Error(ex, "Chyba při načítání skladovacích pozic");
                }
                return skladovaciPozice;
            }
            internal async Task NaplnComboBoxSkladovaciPoziceAsync(ComboBox comboBox)
            {
                await NaplnComboBoxSkladovaciPozicePrivateAsync(comboBox);
            }
            private async Task NaplnComboBoxSkladovaciPozicePrivateAsync(ComboBox comboBox)
            {
                try
                {
                    List<SkladovaciPoziceDTO> skladovaciPoziceList = await NactiSkladovaciPoziceAsync();
                    comboBox.Items.Clear();

                    foreach (SkladovaciPoziceDTO pozice in skladovaciPoziceList)
                    {
                        comboBox.Items.Add(pozice.Nazev);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Chyba při naplňování seznamu skladovacích pozic: {ex.Message}");
                    Log.Error(ex, "Chyba při naplňování seznamu skladovacích pozic");
                }
            }
        }
        internal class VlozdoDatabazeNovyDodavatel
        {
            // Metoda pro nalezení id Země z databáze a vrácení její hodnoty
            private static async Task<int> ZiskatIdZemeAsync(string zemeNazev)
            {
                int ZemeID = -1;

                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection pripojeni = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT ZemeID FROM dbo.Zeme WHERE ZemeNazev = @ZemeNazev";

                        await using (SqlCommand cmd = new SqlCommand(sqlDotaz, pripojeni))
                        {
                            cmd.Parameters.AddWithValue("@ZemeNazev", zemeNazev);
                            object vysledek = await cmd.ExecuteScalarAsync() ?? DBNull.Value;
                            ZemeID = ValidaceDatabaze.ZkontrolovatNull(vysledek, -1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při získávání ID země: " + ex.Message);
                    Log.Error(ex, "Chyba při získávání ID země");
                }

                return ZemeID;
            }
            //Metoda pro uložení nobého dodavatele do databáze
            internal async Task UlozitDodavatele(string nazev, string ico, string dic, string popis, string typDodavatele, string ulice, string cisloPopisne, string psc, string obec, string zeme)
            {
                await UlozitDodavatelePrivate(nazev, ico, dic, popis, typDodavatele, ulice, cisloPopisne, psc, obec, zeme);
            }
            private async Task UlozitDodavatelePrivate(string nazev, string ico, string dic, string popis, string typDodavatele, string ulice, string cisloPopisne, string psc, string obec, string zeme)
            {
                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection pripojeni = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        // Získání ZemeID pro vybranou zemi
                        int zemeID = await ZiskatIdZemeAsync(zeme);

                        // Nejprve vložte adresu do tabulky AdresyDodavatelu
                        string sqlAdresaDotaz = "INSERT INTO AdresyDodavatelu (Ulice, CisloPopisne, PSC, Obec, ZemeID) VALUES (@Ulice, @CisloPopisne, @PSC, @Obec, @ZemeID); SELECT SCOPE_IDENTITY();";


                        await using (SqlCommand adresaCmd = new SqlCommand(sqlAdresaDotaz, pripojeni))
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

                            await using (SqlCommand dodavatelCmd = new SqlCommand(sqlDodavatelDotaz, pripojeni))
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
                    Log.Error(ex, "Chyba při ukládání do databáze");
                }

            }
            // Metoda pro získání id typu dodavatele (as., s.r.o., fyzická osoba atd.)
            private static async Task<int> ZiskatIdTypuDodavateleAsync(string nazevTypu)
            {
                int typDodavateleID = -1; // Defaultní hodnota v případě, že se ID nepodaří najít.

                try
                {
                    // Otevření spojení s databází.
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection pripojeni = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        string sqlDotaz = "SELECT TypDodavateleID FROM dbo.TypyDodavatelu WHERE Nazev = @Nazev";

                        // Vytvoření a konfigurace SQL příkazu.
                        await using (SqlCommand cmd = new SqlCommand(sqlDotaz, pripojeni))
                        {
                            // Přidání parametru @Nazev do SQL příkazu.
                            cmd.Parameters.AddWithValue("@Nazev", nazevTypu);

                            // Provedení SQL příkazu a získání jednoho výsledku (první sloupce prvního řádku).
                            object vysledek = await cmd.ExecuteScalarAsync() ?? DBNull.Value;
                            typDodavateleID = ValidaceDatabaze.ZkontrolovatNull(vysledek, -1);

                        }
                    }
                }
                catch (Exception ex)
                {
                    // Zpráva o chybě v případě výjimky.
                    MessageBox.Show("Chyba při získávání ID typu dodavatele: " + ex.Message);
                    Log.Error(ex, "Chyba při získávání ID typu dodavatele");
                }

                // Vrátí ID typu dodavatele.
                return typDodavateleID;
            }

        }
        internal class VlozdoDatabazeSkladovaciPozice
        {
            internal static async Task<int> UlozitSkladovaciPoziciAsync(string skladovaciPoziceNazev)
            {
              return await UlozitSkladovaciPoziciPrivateAsync(skladovaciPoziceNazev);
            }
            private static async Task<int> UlozitSkladovaciPoziciPrivateAsync(string skladovaciPoziceNazev)
            {
                int skladovaciPoziceID = -1;

                try
                {
                    PripojeniDatabazeObecne pripojeniDatabaze = new PripojeniDatabazeObecne();

                    await using (SqlConnection pripojeni = await pripojeniDatabaze.OtevritSpojeniAsync())
                    {
                        // Vložení skladovací pozice
                        string sqlSkladovaciPoziceDotaz = @"
                    INSERT INTO SkladovaciPozice (SkladovaciPoziceNazev) 
                    VALUES (@SkladovaciPoziceNazev);
                    SELECT SCOPE_IDENTITY();";

                        await using (SqlCommand skladovaciPoziceCmd = new(sqlSkladovaciPoziceDotaz, pripojeni))
                        {
                            skladovaciPoziceCmd.Parameters.AddWithValue("@SkladovaciPoziceNazev", skladovaciPoziceNazev);

                            // Získání ID nově vložené skladovací pozice
                            object vysledek = await skladovaciPoziceCmd.ExecuteScalarAsync() ?? DBNull.Value;
                            if (vysledek == null || vysledek == DBNull.Value)
                            {
                                skladovaciPoziceID = -1; // Výchozí hodnota
                            }
                            else
                            {
                                skladovaciPoziceID = ValidaceDatabaze.ZkontrolovatNull(vysledek, -1);
                            }

                        }
                    }

                    MessageBox.Show("Data byla úspěšně uložena.", "Úspěch", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při ukládání do databáze: " + ex.Message);
                    Log.Error(ex, "Chyba při ukládání do databáze");
                }

                return skladovaciPoziceID;
            }
            internal async Task<bool> KontrolaDuplicityNazvuPozice(string nazev)
            {
              return await KontrolaDuplicityNazvuPozicePrivate(nazev);
            }
            private async Task<bool> KontrolaDuplicityNazvuPozicePrivate(string nazev)
            {
                PripojeniDatabazeObecne pripojeniDatabazeObecne = new PripojeniDatabazeObecne();
                using (SqlConnection pripojeni = await pripojeniDatabazeObecne.OtevritSpojeniAsync())
                {
                    string query = "SELECT COUNT(1) FROM SkladovacíPozice WHERE NazevSkladovaciPozice = @nazev";
                                       
                    using (SqlCommand prikaz = new SqlCommand(query, pripojeni))
                    {
                        prikaz.Parameters.AddWithValue("@nazev", nazev);
                        object vysledek = await prikaz.ExecuteScalarAsync() ?? DBNull.Value;
                        int pocetZaznamu = ValidaceDatabaze.ZkontrolovatNull(vysledek, -1);
                        return pocetZaznamu > 0; // true pokud větší než 0
                    }
                }
            }
        }
    }
}



