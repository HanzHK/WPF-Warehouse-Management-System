using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Windows.Compatibility;

namespace system_sprava_skladu
{
    public partial class okno_generovani_barcode : Window
    {
        private string nveKod;

        public okno_generovani_barcode(string nveKod)
        {
            InitializeComponent();
            this.nveKod = nveKod;
            VygenerujBarcode();
        }

        private void VygenerujBarcode()
        {
            // Použití BarcodeWriter<Bitmap> pro generování obrázku čárového kódu
            BarcodeWriter barcodeWriter = new ZXing.Windows.Compatibility.BarcodeWriter()
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

        private static ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            using (var memory = new System.IO.MemoryStream())
            {
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
}
