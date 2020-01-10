using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Drawing;

namespace Switch
{
    class AvatarGen
    {
        public static string GenerateHash(string soul)
        {
            int unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            string name = unixTime.ToString() + soul;
            var message = Encoding.ASCII.GetBytes(name + Convert.ToString(DateTime.UtcNow)); // уникальная строка
            SHA256Managed hashString = new SHA256Managed();
            string hex = "";

            var hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x); // получение уникального хэша пользователя
                Console.Write(x);
            }

            return hex;
        }

        public static Bitmap GenerateAvatar(string hex)
        {
            var hashValue = StringToByteArray(hex);
            Bitmap bmp = new Bitmap(260, 260);
            Brush Brush = new SolidBrush(ColorTranslator.FromHtml($"#{hex.Substring(0, 6)}")); // устанавливаем цвет

            List<bool> FillOrNot = new List<bool>();
            int n = 3;
            bool check = true;

            while (check)
            {
                for (int i = 0; i <= 7; i++)
                {
                    if (FillOrNot.Count < 15)
                    {
                        FillOrNot.Add((hashValue[n] & (1 << i)) != 0);
                    }
                    else
                    {
                        check = false;
                        break;
                    }
                }
                n++;
            }

            int coordX = 0;
            int coordY = 0;
            int rowNumber = 1;

            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.FillRectangle(Brushes.White, new Rectangle(0, 0, 260, 260));
                for (int i = 0; i < 15; i++)
                {
                    if ((i % 5) != 0 || i == 0)
                    {
                        if (FillOrNot[i])
                        {
                            gr.FillRectangle(Brush, new Rectangle(coordX, coordY + ((i % 5) * 52), 52, 52));
                            gr.FillRectangle(Brush, new Rectangle((260 - (52 * rowNumber)), coordY + ((i % 5) * 52), 52, 52));
                        }
                    }
                    else
                    {
                        coordX += 52;
                        rowNumber++;
                        coordY = 0;

                        if (FillOrNot[i])
                        {
                            gr.FillRectangle(Brush, new Rectangle(coordX, coordY + ((i % 5) * 52), 52, 52));
                            gr.FillRectangle(Brush, new Rectangle((260 - (52 * rowNumber)), coordY + ((i % 5) * 52), 52, 52));
                        }
                    }
                }
            }
            return bmp;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private static Random random = new Random((int)DateTime.Now.Ticks);
        public static string RandomString(int length)
        {
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var builder = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                var c = pool[random.Next(0, pool.Length)];
                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}
