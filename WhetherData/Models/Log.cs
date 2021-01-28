using System;
using System.Collections.Generic;
using System.Text;

namespace WhetherData
{
    public partial class Log
    {
        public DateTime Time { get; set; }
        public string Location { get; set; }
        public double Tempreture { get; set; }
        public double Humidity { get; set; }
        public double MoldRisk { get; set; }
    }
}
