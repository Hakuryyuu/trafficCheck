using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trafficexposer.Data
{
    public class DataProvider : IDataProvider
    {
        DeserializeJson Deserializer = new DeserializeJson(Sysdba.API_KEY);
        FileManagement Filer = new FileManagement();
        public async Task<Location[]> getLocations(string sLoc)
        {
            try
            {
                return await Deserializer.getCitiesAsync(sLoc);
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
                Incident[] oIncidents = await Deserializer.getTrafficInformationAsync(area.StartLocation, area.Destiny);
                oIncidents = oIncidents.Where(ix => ix.LocX != 0).ToArray(); // Remove unnecessary slots
                return oIncidents;
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
                List<Area> liCurrentData;
                if (Sysdba.SavedData.Areas != null)
                {
                    liCurrentData = Sysdba.SavedData.Areas.ToList();
                }
                else
                {
                    return;
                }
                liCurrentData.Remove(area);
                Sysdba.SavedData.Areas = liCurrentData.ToArray();
                await Filer.SerializeSettings(Sysdba.SavedData);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AddAreaAsync(TimeSpan? tsLeave, Location oStartLoc, Location oDestinyLoc)
        {
            try
            {
                List<Area> liCurrentData;
                if (Sysdba.SavedData.Areas != null)
                {
                    liCurrentData = Sysdba.SavedData.Areas.ToList();
                }
                else
                {
                    liCurrentData = new List<Area>();
                }
                liCurrentData.Add(new Area { EstimatedLeave = tsLeave, StartLocation = oStartLoc, Destiny = oDestinyLoc });
                Sysdba.SavedData.Areas = liCurrentData.ToArray();
                await Filer.SerializeSettings(Sysdba.SavedData);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
