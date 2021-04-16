using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Instants;
using System.Data;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using Xunit;

namespace System.Labors.Tests
{
    public class LaborsTest
    {
        public LaborsTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [Fact]
        public void Labors_Inductive_MultiThreading_Integration_Test()
        {
            
            Laboratory lab = new Laboratory();

            Subject download = lab.Expanse(new InstantDeputy(typeof(FirstCurrency).FullName, nameof(FirstCurrency.GetFirstCurrency)), "Download");
            download.Add(new InstantDeputy(typeof(SecondCurrency).FullName, nameof(SecondCurrency.GetSecondCurrency)));
            download.Visor.CreateLaborers(8); /// e.g. 8 workers/consumers for downloading 2 different currency rates

            Subject compute = lab.Expanse(new InstantDeputy(typeof(ComputeCurrency).FullName, nameof(ComputeCurrency.Compute)), "Compute");
            compute.Add(new InstantDeputy(typeof(PresentResult).FullName, nameof(PresentResult.Present)));
            compute.Visor.CreateLaborers(4); /// e.g. 4 workers/consumers for computing and presenting results


            List<Labor> ComputeResultRequirements = new List<Labor>()
            {
                download[nameof(FirstCurrency.GetFirstCurrency)],
                download[nameof(SecondCurrency.GetSecondCurrency)]
            };

            download[nameof(FirstCurrency.GetFirstCurrency)]
                .Laborer
                    .AddEvoker(compute[nameof(ComputeCurrency.Compute)], ComputeResultRequirements);

            download[nameof(SecondCurrency.GetSecondCurrency)]
                .Laborer
                    .AddEvoker(compute[nameof(ComputeCurrency.Compute)], ComputeResultRequirements);

            compute[nameof(ComputeCurrency.Compute)]
                .Laborer
                    .AddEvoker(compute[nameof(PresentResult.Present)]);

            for (int i = 1; i < 15; i++)
            {
                download[nameof(FirstCurrency.GetFirstCurrency)].Elaborate("EUR", i);
                download[nameof(SecondCurrency.GetSecondCurrency)].Elaborate("USD", i);
            }

            Thread.Sleep(15000);

            download.Visor.Close(true);
            compute.Visor.Close(true);
        }

        [Fact]
        public void Labors_QuickLabor_Integration_Test()
        {

            QuickLabor ql0 = new QuickLabor(typeof(FirstCurrency).FullName, nameof(FirstCurrency.GetFirstCurrency), "EUR", 1);
            QuickLabor ql1 = new QuickLabor(typeof(SecondCurrency).FullName, nameof(SecondCurrency.GetSecondCurrency), "USD", 1);

            Thread.Sleep(5000);           
        }
    }

    public class FirstCurrency
    {
        public object GetFirstCurrency(string currency, int days)
        {
            Thread.Sleep(2000);

            NBPSource kurKraju = new NBPSource(days);

            try
            {
                double rate = kurKraju.LoadRate(currency);
                Debug.WriteLine("Kurs 1 : " + currency + " z przed : " + days.ToString() + " wynosi : " + rate.ToString("#.####"));

                return new object[] { currency, rate };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new InvalidLaborException(ex.ToString());
            }
        }
    }

    public class SecondCurrency
    {
        public object GetSecondCurrency(string currency, int days)
        {
            NBPSource kurKraju = new NBPSource(days);

            try
            {
                double rate = kurKraju.LoadRate(currency);
                Debug.WriteLine("Kurs 2 : " + currency + " z przed : " + days.ToString() + " wynosi : " + rate.ToString("#.####"));

                return new object[] { currency, rate };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new InvalidLaborException(ex.ToString());
            }
        }
    }

    public class ComputeCurrency
    {
        public object Compute(string currency1, double rate1, string currency2, double rate2)
        {
            try
            {
                double _rate1 = rate1;
                double _rate2 = rate2;
                double result = _rate2 / _rate1;
                Debug.WriteLine("wynik: " + result.ToString());

                return new object[] { _rate1, _rate2, result };
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new InvalidLaborException(ex.ToString());
            }
        }
    }

    public class PresentResult
    {
        public object Present(double rate1, double rate2, double result)
        {

            try
            {
                string present = "Rate USD : " + rate1.ToString() + " EUR : " + rate2.ToString() + " EUR->USD : " + result.ToString();
                Debug.WriteLine(present);
                return present;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new InvalidLaborException(ex.ToString());
            }
        }
    }

    public class NBPSource
    {
        private const string file_dir = "http://www.nbp.pl/Kursy/xml/dir.txt";
        private const string xml_url = "http://www.nbp.pl/kursy/xml/";
        private int start_int = 1;
        public string file_name;
        public DateTime rate_date;

        public NBPSource(int daysbefore)
        {
            GetFileName(daysbefore);
        }

        public Dictionary<string, double> GetCurrenciesRate(List<string> currency_names)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();

            foreach (var item in currency_names)
            {
                result.Add(item, LoadRate(item));
            }
            return result;

        }

        public double LoadRate(string currency_name)
        {

            try
            {
                string file = xml_url + file_name + ".xml";
                DataSet ds = new DataSet(); ds.ReadXml(file);
                var tabledate = ds.Tables["tabela_kursow"].Rows.Cast<DataRow>().AsEnumerable();
                var before_rate_date = (from k in tabledate select new { Data = k["data_publikacji"].ToString() }).First();
                var tabela = ds.Tables["pozycja"].Rows.Cast<DataRow>().AsEnumerable();
                var rate = (from k in tabela where k["kod_waluty"].ToString() == currency_name select new { Kurs = k["kurs_sredni"].ToString() }).First();
                rate_date = Convert.ToDateTime(before_rate_date.Data);
                return Convert.ToDouble(rate.Kurs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new InvalidLaborException(ex.ToString());
            }
        }

        private void GetFileName(int daysbefore)
        {

            try
            {
                int minusdays = daysbefore * -1;
                WebClient client = new WebClient();
                Stream strm = client.OpenRead(file_dir);
                StreamReader sr = new StreamReader(strm);
                string file_list = sr.ReadToEnd();
                string date_str = string.Empty;
                bool has_this_rate = false;
                DateTime date_of_rate = DateTime.Now.AddDays(minusdays);
                while (!has_this_rate)
                {
                    date_str = "a" + start_int.ToString().PadLeft(3, '0') + "z" + date_of_rate.ToString("yyMMdd");
                    if (file_list.Contains(date_str))
                    {
                        has_this_rate = true;
                    }

                    start_int++;

                    if (start_int > 365)
                    {
                        start_int = 1;
                        date_of_rate = date_of_rate.AddDays(-1);
                    }
                }
                file_name = date_str;
                rate_date = date_of_rate;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new InvalidLaborException(ex.ToString());
            }
        }

    }
}
