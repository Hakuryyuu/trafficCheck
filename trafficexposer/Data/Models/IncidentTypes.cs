using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trafficexposer.Data
{
    /// <summary>
    /// Types of Traffic Events
    /// </summary>
    public struct IncidentTypes
    {
        public enum Type
        {
            UNKNOWN,
            ACCIDENT_CLEARED,
            TRAFFIC_JAM,
            ROADWORK,
            ACCIDENT,
            LONG_TERM_ROADWORK,
        }
        public enum Severity
        {
            NO_DELAY,
            SLOW_TRAFFIC,
            QUEUING_TRAFFIC,
            STATIONARY_TRAFFIC,
            CLOSED,
            UNKNOWN
        }
    }
}
