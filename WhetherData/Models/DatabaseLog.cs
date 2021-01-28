using System;
using System.Collections.Generic;
using System.Text;

namespace WhetherData
{
    public class DatabaseLog
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string Location { get; set; }
        public double Tempreture { get; set; }
        public double Humidity { get; set; }

        public DatabaseLog(DateTime time, string location, double tempreture,double humidity)
        {
            Time = time;
            Location = location;
            Tempreture = tempreture;
            Humidity = humidity;
        }
    }
}
