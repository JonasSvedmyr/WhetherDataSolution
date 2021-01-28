using System;
using System.Collections.Generic;
using System.Text;

namespace WhetherData
{
    public partial class Log
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string Location { get; set; }
        public double Tempreture { get; set; }
        public double Humidity { get; set; }
        public double MoldRisk { get; set; }

        public int DoorOpen { get; set; }

        public Log()
        {

        }
        public Log(DateTime time, string location, double tempreture,double humidity)
        {
            Time = time;
            Location = location;
            Tempreture = tempreture;
            Humidity = humidity;
        }
    }
}
