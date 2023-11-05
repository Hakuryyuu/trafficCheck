/*
 *  Created by: Hakuryuu
 *  www.hakuryuu.net
 *  info@hakuryuu.net
 *  
 *  Copyright (c) 2023 Hakuryuu
 * 
 */

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
        /// <param name="sLoc"></param>
        /// <returns></returns>
        public Task<Location[]> getLocations(string sLoc);
        /// <summary>
        /// Returns all Incidents for the defined Area
        /// </summary>
        /// <param name="oArea"></param>
        /// <returns></returns>
        public Task<Incident[]> getIncidents(Area oArea);
        /// <summary>
        /// Remove an Area permanently
        /// </summary>
        /// <param name="oArea"></param>
        /// <returns></returns>
        public Task RemoveAreaAsync(Area oArea);
        /// <summary>
        /// Add and Area permanently
        /// </summary>
        /// <param name="tsLeave"></param>
        /// <param name="oStartLoc"></param>
        /// <param name="oDestinyLoc"></param>
        /// <returns></returns>
        public Task AddAreaAsync(TimeSpan? tsLeave, Location oStartLoc, Location oDestinyLoc);
    }
}
