using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trafficexposer.Data
{
    public interface IDataProvider
    {
        /// <summary>
        /// Returns an Array of possible Matches
        /// </summary>
        /// <param name="Loc"></param>
        /// <returns></returns>
        public Task<Location[]> getLocations(string Loc);
        /// <summary>
        /// Returns all Incidents for the defined Area
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public Task<Incident[]> getIncidents(Area area);
    }
}
