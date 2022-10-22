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
        /// <summary>
        /// Remove an Area permanently
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public Task RemoveAreaAsync(Area area);
        /// <summary>
        /// Add and Area permanently
        /// </summary>
        /// <param name="Leave"></param>
        /// <param name="startLoc"></param>
        /// <param name="destinyLoc"></param>
        /// <returns></returns>
        public Task AddAreaAsync(TimeSpan? Leave, Location startLoc, Location destinyLoc);
    }
}
