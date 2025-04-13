using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Windows.Compatibility;

namespace system_sprava_skladu
{
    public partial class OknoGenerovaniBarcode : Window
    {
        private readonly string nveKod;

        public OknoGenerovaniBarcode(string nveKod)
        {
            InitializeComponent();
            this.nveKod = nveKod;
            VygenerujBarcode();
        }

        private void VygenerujBarcode()
        {
            // Použití BarcodeWriter<Bitmap> pro generování obrázku čárového kódu
            BarcodeWriter barcodeWriter = new()
            {
                Format = BarcodeFormat.CODE_128,  // Můžeš zvolit jiný formát
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 400,  
                    Height = 300  
                }
            };

            // Generování čárového kódu
            var barcodeBitmap = barcodeWriter.Write(nveKod);
            barcodeObrazek.Source = BitmapToImageSource(barcodeBitmap);
        }

        private static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using var memory = new System.IO.MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Seek(0, System.IO.SeekOrigin.Begin);
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}
