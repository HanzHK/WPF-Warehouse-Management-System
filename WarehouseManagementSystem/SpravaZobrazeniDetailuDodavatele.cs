using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace system_sprava_skladu
{
    internal class SpravaZobrazeniDetailuDodavatele
    {
        private static async Task OtevriDetailDodavatelePrivate(ContentControl cilovyContentControl, int dodavatelID)
        {
            OknoDetailDodavatele detailControl = new();
            await detailControl.NastavDodavateleAsync(dodavatelID); 
            cilovyContentControl.Content = detailControl;
        }
        public static async Task OtevriDetailDodavatele(ContentControl cilovyContentControl, int dodavatelID)
        {
            await OtevriDetailDodavatelePrivate(cilovyContentControl, dodavatelID);
        }
    }
}
