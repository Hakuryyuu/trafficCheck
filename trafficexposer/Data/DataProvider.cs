using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trafficexposer.Data
{
    public class DataProvider : IDataProvider
    {
        DeserializeJson DS = new DeserializeJson(Sysdba.API_KEY);
        public async Task<Location[]> getLocations(string Loc)
        {
            return await DS.getCitiesAsync(Loc);
        }

        public async Task<Incident[]> getIncidents(Area area)
        {
          return await DS.getTrafficInformationAsync(area.StartLocation, area.Destiny);
        }
    }
}
