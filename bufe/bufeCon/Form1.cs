using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace bufeCon
{
    public partial class Form1 : Form
    {

        List<string> termek = new List<string>(); //egységár.csv name of product
        List<string> ar = new List<string>(); // egysegar.cs prices
        List<string> vasarlo = new List<string>(); //fogyasztas.csv buyers
        List<string> vett = new List<string>(); //fogyasztas.csv whatdid he bought
        List<string> darab = new List<string>(); //fogyasztas.csv  how many bought
        List<string> vasarList = new List<string>();//vásárlók list without blanks
        List<int> napszakIndex = new List<int>();//indexes for buyers and part of day

        //workaround for a bug false at start
        bool antiBug = false;

        public Form1()
        {
            InitializeComponent();

            //buttons at start
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;


        }


        //beolvasó button 
        private void button1_Click(object sender, EventArgs e)
        {
            var olvasAr = new StreamReader(File.OpenRead("egysegar.csv")); //read egysegar.csv 
            var olvasFogyaszt = new StreamReader(File.OpenRead("fogyasztas.csv")); //read fogyasztas.csv

            //egysegar.csv into lists
            while (!olvasAr.EndOfStream)
            {
                var sor = olvasAr.ReadLine();
                var ertek = sor.Split(';');

                termek.Add(ertek[0]);
                ar.Add(ertek[1]);
            }


            //fogyasztas.csv into lists
            while (!olvasFogyaszt.EndOfStream)
            {
                var sor = olvasFogyaszt.ReadLine();
                var ertek = sor.Split(';');

                vasarlo.Add(ertek[0]);
                vett.Add(ertek[1]);
                darab.Add(ertek[2]);
            }

            vasarList = vasarlo.Where(teve => !string.IsNullOrWhiteSpace(teve)).Distinct().ToList();//save vasarlo without blank



            //close readers
            olvasAr.Close();
            olvasFogyaszt.Close();

            //beolvas button disabled
            button1.Enabled = false;

            //enable buttons
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;






        }

        //vásárlók száma button 
        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.Add($"A mai napon vásárlók száma: {vasarList.Count}");
        }

        //kávék button
        private void button3_Click(object sender, EventArgs e)
        {
            int szamlal = 0;//counter for coffe buyers
            int kaveSzam = 0; //counter for coffees


            //going through on buyers
            for (int i = 0; i < vett.Count; i++)
            {
                if (vett[i]=="Kávé")//if he bought coffee...
                {
                    szamlal++;//add 1 to buyers who bought coffe and...

                    kaveSzam = kaveSzam + Convert.ToInt32(darab[i]);//save number ot coffees he bought
                }

            }
            listBox1.Items.Clear();
            listBox1.Items.Add($"{szamlal} darab vásárló vitt el összesen {kaveSzam} adag kávét.");
        }

        //bevétel button 
        private void button4_Click(object sender, EventArgs e)
        {
            int bevetel = 0;

            //at blank spaces based on index check item bought and check for item price for counting
            for (int i = 0; i < vasarlo.Count; i++)
            {
                if (vasarlo[i]=="")
                {
                    for (int j = 0; j < termek.Count; j++)
                    {
                        if (vett[i]==termek[j])
                        {
                            bevetel = bevetel +(Convert.ToInt32(darab[i]) * Convert.ToInt32(ar[j]));
                        }
                    }
                }
            }

            listBox1.Items.Clear();

            //save part of day (and with this buyers) index for later use


            for (int i = 0; i < vett.Count; i++)
            {
                if (vett[i]=="Reggel" && !antiBug || vett[i] == "Délelőtt" && !antiBug || vett[i] == "Délután" && !antiBug)
                {
                    napszakIndex.Add(i);
                }
            }

            //torn this on so napszakindex list is not growing any more
            antiBug = true;
            try
            {
                int mostvesz = 0;//which buyer are we checking
                for (int i = 0; i < napszakIndex.Count; i++)
                {


                    for (int j = napszakIndex[i]; j < napszakIndex[i + 1]; j++)//fisrt and secong for just goes through the whole vett list
                    {
                        int termekar = 0;//prices
                        List<int> koltott = new List<int>();//money spent by 1 costumer

                        if (vett[j] == "Reggel" || vett[j] == "Délelőtt" || vett[j] == "Délután")//if part of day found...
                        {

                            mostvesz++;
                            for (int n = j+1; n < napszakIndex[i+1]; n++)//.... that index is ignored and we only check for items
                            {


                                for (int k = 0; k < termek.Count; k++)//looking for the prices
                                {

                                    if (vett[n] == termek[k])
                                    {
                                        koltott.Add( termekar + (Convert.ToInt32(ar[k]) * Convert.ToInt32(darab[n])));
                                    }

                                }

                            }
                            File.AppendAllText("E:\\vasarlok.csv", $" Vásárló{mostvesz};{koltott.Sum(x => Convert.ToInt32(x)).ToString("c0", CultureInfo.CurrentCulture)}", Encoding.UTF8);

                            listBox1.Items.Add($"Vásárló{mostvesz}      elköltött  {koltott.Sum(x => Convert.ToInt32(x)).ToString("c0", CultureInfo.CurrentCulture)} -ot");

                        }

                    }

                }
            }
            catch (Exception)// the last buyer and item
            {
                int termekar = 0;//for price
                for (int i = napszakIndex.Last()+1; i < vett.Count; i++)
                {
                    for (int j = 0; j < termek.Count; j++)
                    {
                        if (vett[i]==termek[j])
                        {
                            termekar = termekar + (Convert.ToInt32(darab[i]) * Convert.ToInt32(ar[j]));
                        }
                    }
                }
                listBox1.Items.Add($"Vásárló {vasarList.Count()}     elköltött  {termekar.ToString("c0", CultureInfo.CurrentCulture)} -ot");

            }

            listBox1.Items.Add($"A mai napi bevétel : {bevetel.ToString("c2", CultureInfo.CurrentCulture)}");

            File.AppendAllText("E:\\vasarlok.csv", $"Napi bevétel:;{bevetel.ToString("c2", CultureInfo.CurrentCulture)}", Encoding.UTF8);



        }

        //napszakok button
        private void button5_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            int reggel = 0, delelott = 0, delutan = 0;//number of buyers at part of day

            for (int i = 0; i < vett.Count; i++)
            {
                if (vett[i] == "Reggel")
                {
                    reggel++;
                }
                else if (vett[i]=="Délelőtt")
                {
                    delelott++;
                }
                else if(vett[i]=="Délután")
                {
                    delutan++;
                }
            }

            listBox1.Items.Add($"Reggel {reggel}, délelőtt {delelott}, délután pedig {delutan} vásárló volt.");
        }
    }
}
