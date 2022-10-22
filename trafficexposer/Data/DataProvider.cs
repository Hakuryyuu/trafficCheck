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

        public async Task RemoveAreaAsync(Area area)
        {
            List<Area> currentData;
            if (Sysdba.SavedData.Areas != null)
            {
                currentData = Sysdba.SavedData.Areas.ToList();
            }
            else
            {
                return;
            }
            currentData.Remove(area);
            Sysdba.SavedData.Areas = currentData.ToArray();
        }

        public async Task AddAreaAsync(TimeSpan? Leave, Location startLoc, Location destinyLoc)
        {
            FileManagement Filer = new FileManagement();
            List<Area> currentData;
            if (Sysdba.SavedData.Areas != null)
            {
                currentData = Sysdba.SavedData.Areas.ToList();
            }
            else
            {
                currentData = new List<Area>();
            }
            currentData.Add(new Area { EstimatedLeave = Leave, StartLocation = startLoc, Destiny = destinyLoc });
            Sysdba.SavedData.Areas = currentData.ToArray();
            await Filer.SerializeSettings(Sysdba.SavedData);
        }
    }
}
