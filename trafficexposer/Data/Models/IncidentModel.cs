using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trafficexposer.Data
{
    /// <summary>
    /// Traffic Incident Data
    /// </summary>
    public struct Incident
    {
        public string From { get; set; }
        public string To { get; set; } 
        public string AdditionalInfo { get; set; }
        public string Description { get; set; }
        public string LengthOfDelay { get; set; }
        public string SinceTime { get; set; }
        public double LocX { get; set; }
        public double LocY { get; set; }
        public IncidentTypes.Severity Severity { get; set; }
        public IncidentTypes.Type Type { get; set; }
    }
}
