using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MyApp
{
    internal class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFile(
            string fileName,
            uint desiredAccess,
            uint shareMode,
            IntPtr securityAttributes,
            uint creationDisposition,
            uint flagsAndAttributes,
            IntPtr templateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ReadFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        const uint GENERIC_READ = 0x80000000;
        const uint OPEN_EXISTING = 3;
        const uint FILE_SHARE_READ = 0x00000001;

        static void Main(string[] args)
        {
            //DataSetimizin dosya yolunu buraya giriyoruz.
            string dosyaYolu = @"C:\Users\DELL\Documents\GitHub\DataTableConverter\DataConverter\DataSet.txt";

            IntPtr dosyaHandle = CreateFile(dosyaYolu, GENERIC_READ, FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

            if (dosyaHandle == IntPtr.Zero)
            {
                Console.WriteLine("Dosya açılamadı.");
                return;
            }

            //Dosya boyutunu sınırlamayı burdaki byte değişkeninden ayarlıyoruz.
            byte[] buffer = new byte[10240];
            uint bytesOkundu;
            StringBuilder toplamIcerik = new StringBuilder();
            //DataSetimizin satir ve sütunlarını oluşturuyoruz.
            int satirSayisi = 0;
            int sutunSayisi = 0;


            //Burda ise ReadFile kullanarak byte byte verimizi okuyup okunanVeri stringine aktarıyoruz.
            do
            {
                if (ReadFile(dosyaHandle, buffer, (uint)buffer.Length, out bytesOkundu, IntPtr.Zero))
                {
                    string okunanVeri = Encoding.UTF8.GetString(buffer, 0, (int)bytesOkundu);
                    toplamIcerik.Append(okunanVeri);
                }
                else
                {
                    //Eğer bir hata sebebi ile dosya okunamıyorsa hata kodumuzu veriyor.
                    Console.WriteLine("Dosya okunamadı.");
                    break;
                }
            } while (bytesOkundu > 0);

            //Tüm içeriği satır satır ayırıyoruz.
            string[] satirlar = toplamIcerik.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //Satır sayisini satirlar dizisinde kaç tane dizi olduğuna bakarak atamasını yapıyoruz.
            satirSayisi = satirlar.Length;

            //Sütun sayısını kontrol et
            foreach (string satir in satirlar)
            {
                if (!string.IsNullOrWhiteSpace(satir))
                {
                    //Her satirdaki "," leri ayırıp sutun dizimize stringleri aktarıyoruz o sırada da sutun sayımızı öğreniyoruz.
                    string[] sutunlar = satir.Split(',');
                    sutunSayisi = Math.Max(sutunSayisi, sutunlar.Length);
                }
            }

            //DataSetimizi oluşturuz.
            string[,] DataSet = new string[satirSayisi, sutunSayisi];

            //DataSet'imizi dolduruyoruz.
            for (int i = 0; i < satirSayisi; i++)
            {
                string[] sutunlar = satirlar[i].Split(',');
                for (int j = 0; j < sutunlar.Length; j++)
                {
                    DataSet[i, j] = sutunlar[j].Trim(); //Trim fonksiyonu ile boşlukları temizliyoruz.
                }
            }

            Console.WriteLine("DataSet İçeriği: \n");
            for (int i = 0; i < satirSayisi; i++)
            {
                for (int j = 0; j < sutunSayisi; j++)
                {
                    Console.Write(DataSet[i, j] + "\t"); //Sütunların arasına tab ile ayırıyoruz.
                }
                Console.WriteLine(); //Satır sonunda yeni satıra geçmek için bi boşluk.
            }

            //Toplam satir ve sutun sayimizi yazdırıyoruz.
            Console.WriteLine($"Toplam Satır Sayısı: {satirSayisi}");
            Console.WriteLine($"En Fazla Sütun Sayısı: {sutunSayisi}");

            //Dosyayı kapatma fonksiyonunu kullanarak dosyayı kapatıyoruz.
            CloseHandle(dosyaHandle);
        }
    }
}
