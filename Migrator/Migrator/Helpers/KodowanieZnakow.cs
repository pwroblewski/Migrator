using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Migrator.Helpers
{
    public static class KodowanieZnakow
    {
        public static string PolskieZnaki(string str, Modul modul)
        {
            try
            {
                Encoding cp852 = Encoding.GetEncoding("CP852");
                Encoding win1250 = Encoding.GetEncoding("Windows-1250");

                byte[] src;

                if(modul == Modul.SRTR)
                    src = cp852.GetBytes(str);
                else 
                    src = win1250.GetBytes(str);

                for (int i = 0; i < src.Length; i++)
                {
                    if (src[i] == 134) { src[i] = 185; }
                    if (src[i] == 141) { src[i] = 230; }
                    if (src[i] == 149) { src[i] = 198; }
                    if (src[i] == 144) { src[i] = 202; }
                    if (src[i] == 145) { src[i] = 234; }
                    if (src[i] == 146) { src[i] = 179; }
                    if (src[i] == 164) { src[i] = 241; }
                    if (src[i] == 162) { src[i] = 243; }
                    if (src[i] == 152) { src[i] = 140; }
                    if (src[i] == 166) { src[i] = 159; }
                    if (src[i] == 161) { src[i] = 175; }
                    if (src[i] == 167) { src[i] = 191; }
                    if (src[i] == 165) { src[i] = 209; }
                    if (src[i] == 163) { src[i] = 211; }
                    if (src[i] == 143) { src[i] = 165; }
                    if (src[i] == 156) { src[i] = 163; }
                    if (src[i] == 158) { src[i] = 156; }
                    if (src[i] == 160) { src[i] = 143; }
                }

                str = win1250.GetString(src);
            }
            catch(Exception ex)
            {
                string message = string.Format("Błąd konwersji na polskie znaki - {0}", ex.Message);
                MessageBox.Show(message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return str;
        }

        public static string PolskieZnakiEWPB(string str)
        {
            try
            {
                Encoding cp852 = Encoding.GetEncoding("CP852");
                Encoding win1250 = Encoding.GetEncoding("Windows-1250");

                byte[] src = cp852.GetBytes(str);
                byte[] convert = Encoding.Convert(cp852, win1250, src);

                str = cp852.GetString(convert);
            }
            catch (Exception ex)
            {
                string message = string.Format("Błąd konwersji na polskie znaki - {0}", ex.Message);
                MessageBox.Show(message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return str;
        }
    }
}
