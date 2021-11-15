using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SC.DevChallenge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PricesController : ControllerBase
    {
        static PricesController()
        {
            try
            {
            bool firstline = true;
            foreach(string line in System.IO.File.ReadAllLines("data.csv"))
            {
                if (firstline)
                {
                    firstline = false;
                    continue;
                }

                string[] parts = line.Split(',', 5);
                string p1 = parts[0];
                string p2 = parts[1];
                string p3 = parts[2];
                DateTime p4 = DateTime.Parse(parts[3], new CultureInfo("de-DE"));
                decimal p5 = decimal.Parse(parts[4]);
                data.Add(new DataEntry(p1, p2, p3, p4, p5));
            }
            }catch(Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        static List<DataEntry> data = new();


        [HttpGet("average")]
        public string Average(
            string portfolio,
            string owner,
            string instrument,
            string date)
        {
            DateTime dt = new(0);
            if(date != null)
                dt = DateTime.Parse(date, new CultureInfo("de-DE"));

            decimal sum = 0;
            int count = 0;
            DateTime datelb = AlignDate(dt);  // нижня межа
            DateTime dateub = datelb.AddSeconds(10000);  // верхня межа

            foreach(DataEntry entry in data)
            {
                bool matches = true;
                if(portfolio != null)
                        matches = matches && string.Compare(portfolio, entry.portfolio, true) == 0;
                if(owner != null)
                        matches = matches && string.Compare(owner, entry.owner, true) == 0;
                if(instrument != null)
                        matches = matches && string.Compare(instrument, entry.instrument, true) == 0;
                if(dt.Ticks != 0)
                        matches = matches && (datelb <= entry.datetime && entry.datetime < dateub);
                if(matches)
                {
                    sum += entry.price;
                    count++;
                }
            }
            if(count == 0) return "";
            decimal avg = sum / count;
            return "{\"date\": \"" +
                datelb.ToString("dd/MM/yyyy HH:mm:ss") +
                "\",\"price\": \"" +
                avg.ToString("0.##", new CultureInfo("de-DE")) +
                "\"}";
        }

        DateTime AlignDate(DateTime date)
        {
            const long timeunit = 10_000_000L * 10_000L;

            // Вирівняти дату й час до 10000 секунд.
            long zeropoint = new DateTime(2018,1,1,0,0,0).Ticks;
            long dateticks = date.Ticks - zeropoint;
            dateticks -= dateticks % timeunit;
            DateTime result = new DateTime(dateticks + zeropoint);
            Debug.WriteLine(result);
            return result;
        }
    }

    class DataEntry
    {
        public string portfolio { get;}
        public string owner { get;}
        public string instrument { get;}
        public DateTime datetime { get;}
        public decimal price { get;}

        public DataEntry(
            string portfolio,
            string owner,
            string instrument,
            DateTime datetime,
            decimal price)
        {
            this.portfolio = portfolio;
            this.owner = owner;
            this.instrument = instrument;
            this.datetime = datetime;
            this.price = price;
        }
    }
}
