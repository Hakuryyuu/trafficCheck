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
        FileManagement Filer = new FileManagement();
        public async Task<Location[]> getLocations(string Loc)
        {
            try
            {
                return await DS.getCitiesAsync(Loc);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<Incident[]> getIncidents(Area area)
        {
            try
            {
                Incident[] incidents = await DS.getTrafficInformationAsync(area.StartLocation, area.Destiny);
                incidents = incidents.Where(ix => ix.LocX != 0).ToArray(); // Remove unnecessary slots
                return incidents;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveAreaAsync(Area area)
        {
            try
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
                await Filer.SerializeSettings(Sysdba.SavedData);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AddAreaAsync(TimeSpan? Leave, Location startLoc, Location destinyLoc)
        {
            try
            {
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
            catch (Exception)
            {

                throw;
            }
        }
    }
}
