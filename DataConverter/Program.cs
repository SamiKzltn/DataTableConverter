using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Serialization.Metadata;

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

        public static string[,] OrtalamaYaz(string[,] Dizi)
        {
            int rows = Dizi.GetLength(0);
            int cols = Dizi.GetLength(1);

            for (int col = 0; col < cols; col++)
            {
                float toplam = 0;
                int sayac = 0;
                List<int> eksikdegerler = new List<int>();

                for (int row = 0; row < rows; row++)
                {
                    if (Dizi[row, col] == "?")
                    {
                        eksikdegerler.Add(row);
                    }
                    else if (float.TryParse(Dizi[row, col], NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                    {
                        toplam += value;
                        sayac++;
                    }
                }
                if (eksikdegerler.Count > 0 && sayac > 0)
                {
                    float ortalama = toplam / sayac;

                    foreach (var eksiksatir in eksikdegerler)
                    {
                        Dizi[eksiksatir, col] = ortalama.ToString("F1", CultureInfo.InvariantCulture);
                        Console.WriteLine(eksiksatir + "," + col + " = " + ortalama.ToString("F1", CultureInfo.InvariantCulture));
                    }
                }
            }
            return Dizi;
        }

        public static void ShowOnConsole(string[,] dizi, int satir_sayisi, int sutun_sayisi)
        {
            for (int i = 0; i < satir_sayisi; i++)
            {
                for (int j = 0; j < sutun_sayisi; j++)
                {
                    Console.Write(dizi[i, j] + "\t"); //Sütunların arasına tab ile ayırıyoruz.
                }
                Console.WriteLine(); //Satır sonunda yeni satıra geçmek için bi boşluk.
            }
        }

        public static string[,] YaziToRakam(string[,] Dizi, string[,] LastModel)
        {
            int rows = Dizi.GetLength(0);
            int cols = Dizi.GetLength(1);
            string[,] SwitchSet = new string[rows, cols];

            for (int i = 0; i < cols; i++)
            {
                Dictionary<string, int> degerMap = new Dictionary<string, int>();
                int mevcutNumara = 1;

                for (int z = 0; z < rows; z++)
                {
                    //Boş veya null olup olmadığını kontrol ediyoruz.
                    if (Dizi[z, i] != null && !string.IsNullOrWhiteSpace(Dizi[z, i]))
                    {
                        if (!(float.TryParse(Dizi[z, i], out float value)))
                        {
                            string currentValue = Dizi[z, i];
                            SwitchSet[z, i] = currentValue;

                            // Eğer değer daha önce görülmediyse, yeni bir numara atıyoruz.
                            if (!degerMap.ContainsKey(currentValue))
                            {
                                degerMap[currentValue] = mevcutNumara;
                                mevcutNumara++;
                            }

                            //O değerin numarasını SwitchSet'e yazıyoruz.
                            SwitchSet[z, i] = degerMap[currentValue].ToString();
                        }
                        else
                        {
                            LastModel[z, i] = Dizi[z, i];
                        }
                    }
                    else
                    {
                        // Eğer null ya da boşsa, bir default değer ya da boş bir string atayabiliriz
                        SwitchSet[z, i] = "0";
                    }
                    degerMap = new Dictionary<string, int>();
                }
            }
            return SwitchSet;
        }

        public static string[,] DiziOlustur(int cols, int rows)
        {
            string[,] dizi = new string[rows, cols];
            return dizi;
        }

        public static Dictionary<string, int> ClassOrganization(string[,] Dizi)
        {
            int rows = Dizi.GetLength(0);
            int cols = Dizi.GetLength(1);
            int deger2 = 0;
            int mevcutnumara = 1;

            Dictionary<string, int> diziSayisi = new Dictionary<string, int>();

            for (int i = 0; i < rows; i++)
            {
                if (!string.IsNullOrWhiteSpace(Dizi[i, cols - 1]))
                {
                    deger2++;
                    string gecici = Dizi[i, cols - 1];
                    if (!diziSayisi.ContainsKey(gecici))
                    {
                        diziSayisi[gecici] = mevcutnumara;
                        mevcutnumara++;
                    }
                }
            }
            foreach (var item in diziSayisi)
            {
                Console.WriteLine(item.Key + " " + item.Value);
            }
            return diziSayisi;
        }
        public static Dictionary<string, List<string[]>> OrganizeByLastColumn(string[,] Dizi)
        {
            int rows = Dizi.GetLength(0);
            int cols = Dizi.GetLength(1);

            // Son sütundaki benzersiz string'ler için bir sözlük (Dictionary) oluşturuyoruz
            Dictionary<string, List<string[]>> result = new Dictionary<string, List<string[]>>();

            for (int i = 0; i < rows; i++)
            {
                // Son sütundaki string'i alıyoruz
                string key = Dizi[i, cols - 1];

                // Eğer sözlükte bu string (key) yoksa, o string için yeni bir liste oluşturuyoruz
                if (!result.ContainsKey(key))
                {
                    result[key] = new List<string[]>();
                }

                // O satırı listeye ekliyoruz
                string[] row = new string[cols];
                for (int j = 0; j < cols; j++)
                {
                    row[j] = Dizi[i, j];
                }
                result[key].Add(row);
            }

            // Çıktı üretme kısmı: Her sınıf ve ona ait satırları yazdırıyoruz
            foreach (var entry in result)
            {
                Console.WriteLine($"Class: {entry.Key}");
                foreach (var row in entry.Value)
                {
                    Console.WriteLine(string.Join(", ", row));
                }
                Console.WriteLine(); // Her sınıf arasına boş satır koyar
            }

            return result;
        }



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
            byte[] buffer = new byte[20480];
            uint bytesOkundu;
            StringBuilder toplamIcerik = new StringBuilder();
            //DataSetimizin satir ve sütunlarını oluşturuyoruz.
            int satirSayisi = 0;
            int sutunSayisi = 0;
            string[] sutunlar = new string[sutunSayisi];
            List<float> floatlist = new List<float>();

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

            bool trick = false;

            //Tüm içeriği satır satır ayırıyoruz.
            string[] satirlar = toplamIcerik.ToString().Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            //Satır sayisini satirlar dizisinde kaç tane dizi olduğuna bakarak atamasını yapıyoruz.
            satirSayisi = satirlar.Length;

            //Sütun sayısını kontrol et
            foreach (string satir in satirlar)
            {
                if (!string.IsNullOrWhiteSpace(satir))
                {
                    if (satir.Contains(","))
                    {
                        //Her satirdaki "," leri ayırıp sutun dizimize stringleri aktarıyoruz o sırada da sutun sayımızı öğreniyoruz.
                        sutunlar = satir.Split(',');
                        trick = true;
                    }
                    else if (satir.Contains(";"))
                    {
                        sutunlar = satir.Split(';');
                    }
                    sutunSayisi = Math.Max(sutunSayisi, sutunlar.Length);
                }
            }

            //DataSetimizi oluşturuz.
            string[,] DataSet = new string[satirSayisi, sutunSayisi];
            string[,] SwitchSet = new string[satirSayisi, sutunSayisi];
            string[,] LastModel = new string[satirSayisi, sutunSayisi];
            string[,] yazilar = new string[200000, 2];

            //DataSet'imizi dolduruyoruz.
            for (int i = 0; i < satirSayisi; i++)
            {
                if (trick == true)
                {
                    sutunlar = satirlar[i].Split(',');
                    for (int j = 0; j < sutunlar.Length; j++)
                    {
                        DataSet[i, j] = sutunlar[j].Trim(); //Trim fonksiyonu ile boşlukları temizliyoruz.
                    }
                }
                else if (trick == false)
                {
                    sutunlar = satirlar[i].Split(';');
                    for (int j = 0; j < sutunlar.Length; j++)
                    {
                        DataSet[i, j] = sutunlar[j].Trim(); //Trim fonksiyonu ile boşlukları temizliyoruz.
                    }
                }
            }
            //Değerleri ve onların karşılık gelen numaralarını tutmak için bir dictionary oluşturuyoruz.
            Dictionary<string, int> degerMap = new Dictionary<string, int>();
            int mevcutNumara = 1;
            int deger = 0;

            //Fonksiyonu kullandığımız yer burasıdır.
            OrtalamaYaz(DataSet);
            Console.WriteLine("---------------------------Lazım");
            //YaziToRakam(DataSet,LastModel);
            Console.WriteLine("-----------------------------");
            ShowOnConsole(DataSet, satirSayisi, sutunSayisi);
            ShowOnConsole(SwitchSet, satirSayisi, sutunSayisi);
            Console.WriteLine("-----------------------------");

            for (int v = 0; v < satirSayisi; v++)
            {
                for (int z = 0; z < sutunSayisi; z++)
                {
                    //Boş veya null olup olmadığını kontrol ediyoruz.
                    if (DataSet[v, z] != null && !string.IsNullOrWhiteSpace(DataSet[v, z]))
                    {
                        if (!(float.TryParse(DataSet[v, z], out float value)))
                        {
                            deger++;
                            string currentValue = DataSet[v, z];
                            SwitchSet[v, z] = currentValue;

                            // Eğer değer daha önce görülmediyse, yeni bir numara atıyoruz.
                            if (!degerMap.ContainsKey(currentValue))
                            {
                                degerMap[currentValue] = mevcutNumara;
                                mevcutNumara++;
                            }

                            //O değerin numarasını SwitchSet'e yazıyoruz.
                            SwitchSet[v, z] = degerMap[currentValue].ToString();
                        }
                        else
                        {
                            LastModel[v, z] = DataSet[v, z];
                        }
                    }
                    else
                    {
                        // Eğer null ya da boşsa, bir default değer ya da boş bir string atayabiliriz
                        SwitchSet[v, z] = "0"; // Veya istediğiniz bir default değer
                    }
                }
            }
            yazilar = new string[deger, 2];

            for (int ss = 0; ss < sutunSayisi; ss++)
            {
                for (int zz = 0; zz < satirSayisi; zz++)
                {
                    if (string.IsNullOrWhiteSpace(LastModel[zz, ss]))
                    {
                        LastModel[zz, ss] = SwitchSet[zz, ss];
                    }
                    else { }
                }
            }
            float[] floatArray = floatlist.ToArray();

            Console.WriteLine("DataSet İçeriği: \n");
            ShowOnConsole(DataSet, satirSayisi, sutunSayisi);

            // DataSetimizdeki değerlerin atandığı sayilar ekrana yazdırılıyor.
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("DataSet'in Stringlerinin Aldığı Değerler : \n");
            foreach (var s in degerMap)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("-------------------------------------------------------");

            Console.WriteLine("DataSet İçeriği: \n");
            ShowOnConsole(LastModel, satirSayisi, sutunSayisi);

            //Toplam satir ve sutun sayimizi yazdırıyoruz.
            Console.WriteLine($"Toplam Satır Sayısı: {satirSayisi}");
            Console.WriteLine($"En Fazla Sütun Sayısı: {sutunSayisi}");
            Console.WriteLine(yazilar.Length);
            Console.WriteLine(deger);

            ClassOrganization(DataSet);

            OrganizeByLastColumn(DataSet);

            //Dosyayı kapatma fonksiyonunu kullanarak dosyayı kapatıyoruz.
            CloseHandle(dosyaHandle);
        }
    }
}