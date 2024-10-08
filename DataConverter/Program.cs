﻿using System;
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
        public static string[,] YaziToRakam(string[,] Dizi)
        {
            int rows = Dizi.GetLength(0);
            int cols = Dizi.GetLength(1);
            string[,] LastModel = new string[rows, cols];
            int mevcutNumara = 1;

            // Her sütun için döngü
            for (int i = 0; i < cols-1; i++)
            {
                Dictionary<string, int> degerMap = new Dictionary<string, int>();
                // Her satır için döngü
                for (int z = 0; z < rows; z++)
                {
                    // Boş veya null olup olmadığını kontrol ediyoruz.
                    if (Dizi[z, i] != null && !string.IsNullOrWhiteSpace(Dizi[z, i]))
                    {

                        if (Dizi[z, i] == "?")
                        {
                            LastModel[z, i] = "?";
                        }
                        // Eğer sayısal bir değer değilse
                        else if (!float.TryParse(Dizi[z, i], out float value))
                        {
                            string currentValue = Dizi[z, i];

                            // Eğer değer daha önce görülmediyse, yeni bir numara atıyoruz.
                            if (!degerMap.ContainsKey(currentValue))
                            {
                                degerMap[currentValue] = mevcutNumara;
                                mevcutNumara++;
                            }

                            // O değerin numarasını SwitchSet'e yazıyoruz.
                            LastModel[z, i] = degerMap[currentValue].ToString();
                        }
                        else
                        {
                            LastModel[z, i] = Dizi[z, i];
                        }
                    }
                    else
                    {
                        // Eğer null ya da boşsa, varsayılan bir değer atayabiliriz (örneğin "0").
                        LastModel[z, i] = "0";
                    }
                }
                foreach (var s in degerMap)
                {
                    Console.WriteLine(s);
                }
                Console.WriteLine("-------------------------------------------------------");
                degerMap = new Dictionary<string, int>();
            }
            for(int z = 0; z < rows; z++)
            {
                LastModel[z, cols-1] = Dizi[z, cols-1];
            }
            return LastModel;
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
            Console.WriteLine("Classlarımız Şunlardır :\n");
            foreach (var entry in result) 
            { 
                Console.WriteLine(entry.Key);
            }

            Console.WriteLine("--------------------------------------\n");

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
        public static void DizileriOlustur(Dictionary<string, List<string[]>> dictionary)
        {
            // Her sınıf (anahtar) için bir dizi oluşturuyoruz
            foreach (var entry in dictionary)
            {
                string className = entry.Key; // Sınıf ismi (anahtar)
                List<string[]> data = entry.Value; // Sınıf verileri (değer)

                // Sınıf verileri ile dizi boyutlarını hesaplayalım
                int rows = data.Count;
                int cols = data[0].Length;

                // Yeni dizi oluşturalım
                string[,] result = new string[rows, cols];

                // Verileri diziye aktaralım
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = data[i][j];
                    }
                }

                // Diziyi ekrana yazdıralım
                Console.WriteLine($"Sınıf: {className}");
                Console.WriteLine("\nSınıfımızın Dönüşüm Değerleri : \n");
                result = YaziToRakam(result);
                Console.WriteLine("\nSoru işaretleri yerine gelen değerler :\n");
                result = OrtalamaYaz(result);
                Console.WriteLine("\nSınıfımızın Son Hali : \n");
                YazdirDizi(result);
                Console.WriteLine("--------------------------------------\n");
            }
        }
        public static void YazdirDizi(string[,] dizi)
        {
            int rows = dizi.GetLength(0);
            int cols = dizi.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(dizi[i, j] + "\t");
                }
                Console.WriteLine();
            }
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
            //Veri setimizi ekrana yazdırıyoruz.
            Console.WriteLine("Girdiğiniz Veri Setiniz :\n");
            YazdirDizi(DataSet);
            Console.WriteLine("---------------------------------");

            //Veri Setinin Rakama çevrilmiş halinin ekrana yazdırılması
            //SwitchSet = YaziToRakam(DataSet);
            //Console.WriteLine("Rakam Halleri\n");
            //YazdirDizi(SwitchSet);
            //Console.WriteLine("---------------------------------");

            Dictionary<string, List<string[]>> classes = new Dictionary<string, List<string[]>>();
            classes = OrganizeByLastColumn(DataSet);

            DizileriOlustur(classes);

            //Toplam satir ve sutun sayimizi yazdırıyoruz.
            Console.WriteLine($"Toplam Satır Sayısı: {satirSayisi}");
            Console.WriteLine($"En Fazla Sütun Sayısı: {sutunSayisi}");

            //Dosyayı kapatma fonksiyonunu kullanarak dosyayı kapatıyoruz.
            CloseHandle(dosyaHandle);
        }
    }
}