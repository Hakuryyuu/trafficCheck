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
            catch (Exception x)
            {
                await App.Current.MainPage.DisplayAlert("Error", x.Message, "OK");
            }
            return null;
        }

        public async Task<Incident[]> getIncidents(Area area)
        {
            try
            {
                Incident[] oIncidents = await Deserializer.getTrafficInformationAsync(area.StartLocation, area.Destiny); // Obtain Data
                List<Incident> liTemp = oIncidents.ToList(); // Conversion to list for LINQ Expressions
                liTemp.RemoveAll(ix => ix.LocX == 0); // Remove unnecessary slots
                TimeSpan tsMaxAccidentAge = TimeSpan.FromHours(12); // Usually an accident should be cleared 12h
                liTemp.RemoveAll(ix => ix.Type == IncidentTypes.Type.ACCIDENT && DateTime.Parse(DateTime.Now.ToString()).Subtract(DateTime.Parse(ix.SinceTime)) > tsMaxAccidentAge); // Remove old Accidents

                oIncidents = liTemp.ToArray(); // Convert back to Array

                return oIncidents; // Retrun filtered result
            }
            catch (Exception x)
            {
                await App.Current.MainPage.DisplayAlert("Error", x.Message, "OK");
            }
            return null;
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
            catch (Exception x)
            {
                await App.Current.MainPage.DisplayAlert("Error", x.Message, "OK");
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
            catch (Exception x)
            {
                await App.Current.MainPage.DisplayAlert("Error", x.Message, "OK");
            }
        }
    }
}
