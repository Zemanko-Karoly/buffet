using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace bufe
{
    class Program
    {
        static void Main(string[] args)
        {
            var olvasAr = new StreamReader(File.OpenRead("egysegar.csv")); //egysegar.csv beolvasása
            var olvasFogyaszt = new StreamReader(File.OpenRead("fogyasztas.csv")); //fogyasztas.csv beolvasása



            List<string> termek = new List<string>(); //egységár.csv böl a termék nevek
            List<string> ar = new List<string>(); // egysegar.cs böl az árak
            List<string> vasarlo = new List<string>(); //fogyasztas.csv böl a vásárlók
            List<string> vett = new List<string>(); //fogyasztas.csv böl mikor mit vett
            List<string> darab = new List<string>(); //fogyasztas.csv böl mennyit vett


            //egysegar.csv listákba szedése
            while (!olvasAr.EndOfStream)
            {
                var sor = olvasAr.ReadLine();
                var ertek = sor.Split(';');

                termek.Add(ertek[0]);
                ar.Add(ertek[1]);
            }


            //fogyasztas.csv listába szedése
            while (!olvasFogyaszt.EndOfStream)
            {
                var sor = olvasFogyaszt.ReadLine();
                var ertek = sor.Split(';');

                vasarlo.Add(ertek[0]);
                vett.Add(ertek[1]);
                darab.Add(ertek[2]);
            }


            //beolvasók bezárása
            olvasAr.Close();
            olvasFogyaszt.Close();

            //termékek és áraik kiírása
            Console.WriteLine("Árlista");
            for (int i = 0; i < termek.Count; i++)
            {
                Console.WriteLine($" {termek[i]} ár: {Convert.ToInt32(ar[i]).ToString("c0", CultureInfo.CurrentCulture)}");
            }


            Console.WriteLine("  ");

            //Az adott napi fogyasztás kiírésa.
            Console.WriteLine("E napi fogyasztás: ");
            for (int i = 0; i < vasarlo.Count; i++)
            {
                Console.WriteLine($" {vasarlo[i]}   {vett[i]}  {darab[i]}");
            }
            

            Console.ReadKey();
        }
    }
}
