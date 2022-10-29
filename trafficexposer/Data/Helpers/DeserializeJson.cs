using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace trafficexposer.Data
{
    public class DeserializeJson
    {
        private readonly string API_KEY;
        public DeserializeJson(string sAPI_Key)
        {
            API_KEY = sAPI_Key;
        }

        /// <summary>
        /// Returns data on Array of all found Cities from given Name
        /// </summary>
        /// <param name="sName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Location[]> getCitiesAsync(string sName)
        {
            Location[] oLocs;
            string sData = await getJsonFromURL($"https://api.tomtom.com/search/2/geocode/{sName}.json?key={API_KEY}");

            if (!String.IsNullOrEmpty(sData))
            {
                JObject joResponse = JObject.Parse(sData);
                JArray oResults = (JArray)joResponse["results"];

                 oLocs = new Location[oResults.Count];

                for (int i = 0; i < oResults.Count; i++)
                {
                    JObject addr = (JObject)oResults[i]["address"];
                    JObject pos = (JObject)oResults[i]["position"];
                    oLocs[i].Name = Convert.ToString(((JValue)addr.SelectToken("freeformAddress")).Value); // Location Name
                    oLocs[i].Longitude = Convert.ToString(((JValue)pos.SelectToken("lon")).Value); //Location cords
                    oLocs[i].Latitude = Convert.ToString(((JValue)pos.SelectToken("lat")).Value);

                }
            }
            else
            {
                throw new Exception("No Data found");
            }
            return oLocs;
        }

        /// <summary>
        /// Returns an Array of incidents in the area between two given locations
        /// </summary>
        /// <param name="oLoc1"></param>
        /// <param name="oLoc2"></param>
        /// <returns></returns>
        public async Task<Incident[]> getTrafficInformationAsync(Location oLoc1, Location oLoc2)
        {
            Incident[] oIncidents = null;
            if (!String.IsNullOrEmpty(oLoc1.Longitude) && // Check if the Data is valid for Exception prevention
                !String.IsNullOrEmpty(oLoc1.Latitude) &&
                !String.IsNullOrEmpty(oLoc2.Latitude) &&
                !String.IsNullOrEmpty(oLoc2.Longitude))
            {


                Format(ref oLoc1); // Format Numbers to US Format
                Format(ref oLoc2);
                string data = await getJsonFromURL($"https://api.tomtom.com/traffic/services/4/incidentDetails/s3/{oLoc1.Latitude},{oLoc1.Longitude},{oLoc2.Latitude},{oLoc2.Longitude}/10/-1/json?key={API_KEY}&projection=EPSG4326");

                if (!String.IsNullOrEmpty(data))
                {
                    JObject joResponse = JObject.Parse(data);
                    JObject joTm = (JObject)joResponse["tm"];
                    JArray joPoi = (JArray)joTm["poi"];

                    oIncidents = new Incident[joPoi.Count];
                   // oIncidents = oIncidents.Where(ix => ix. != 0).ToArray();

                    for (int i = 0; i < joPoi.Count; i++)
                    {
                        if (!((JValue)joPoi[i].SelectToken("id")).Value.ToString().Contains("CLUSTER") &&
                            !String.IsNullOrEmpty(((JValue)joPoi[i].SelectToken("f")).Value.ToString()) &&
                            !String.IsNullOrEmpty(((JValue)joPoi[i].SelectToken("t")).Value.ToString()) && //Basic filtering of invalid/empty data which are returned by the API
                            !String.IsNullOrEmpty(((JValue)joPoi[i].SelectToken("f")).Value.ToString()))
                        {
                            GetType(ref oIncidents, ref joPoi, ref i);
                            TimeSpan AgeOfAccident = DateTime.Parse(DateTime.Now.ToString()).Subtract(DateTime.Parse(Convert.ToString(((JValue)joPoi[i].SelectToken("sd")).Value)));

                                JObject pos = (JObject)joPoi[i]["p"];
                                oIncidents[i].LocX = Convert.ToDouble(((JValue)pos.SelectToken("x")).Value); // Location of the Incident
                                oIncidents[i].LocY = Convert.ToDouble(((JValue)pos.SelectToken("y")).Value);

                                //Incidents[i].AdditionalInfo = Convert.ToString(((JValue)inc[i].SelectToken("c")).Value); // Info like "one line closed"

                                oIncidents[i].Description = Convert.ToString(((JValue)joPoi[i].SelectToken("d")).Value); // desc ex. "road closed"
                                oIncidents[i].SinceTime = Convert.ToString(((JValue)joPoi[i].SelectToken("sd")).Value); // since when its closed
                                oIncidents[i].From = Convert.ToString(((JValue)joPoi[i].SelectToken("f")).Value); // from city
                                oIncidents[i].To = Convert.ToString(((JValue)joPoi[i].SelectToken("t")).Value); // to city
                                oIncidents[i].LengthOfDelay = Convert.ToString(((JValue)joPoi[i].SelectToken("l")).Value); //Lentgh of the Delay in KM
                                Format(ref oIncidents, ref i);

                                GetSeverity(ref oIncidents, ref joPoi, ref i);
                        }
                    }

                }
                else
                {
                    throw new Exception("Data provided by API is null");
                }
            }
            return oIncidents;
        }

        /// <summary>
        /// Formats Numbers to US format for the TomTom API
        /// </summary>
        /// <param name="oLoc"></param>
        private void Format(ref Location oLoc)
        {
            oLoc.Longitude = oLoc.Longitude.Replace(',', '.');
            oLoc.Latitude = oLoc.Latitude.Replace(',', '.');
        }
        /// <summary>
        /// Formats KM for better readability. Ex: from 1276 (<- Returned by API) to 1.2km
        /// </summary>
        private void Format(ref Incident[] oIncidents, ref int iIndex)
        {
            if (oIncidents[iIndex].LengthOfDelay.Length > 1) //Check if theres formatting needed
            {
                string sLength = oIncidents[iIndex].LengthOfDelay;
                char[] cLentghFormatted = new char[sLength.Length + 1]; //+1 for the additional ","

                for (int x = 0; x < sLength.Length; x++) // Set each previous digit in the new string
                {
                    // Set the new positions
                        cLentghFormatted[0] = sLength[0]; // Pos 0 & 1 are always the same so can be hardcoded
                        cLentghFormatted[1] = ',';
                        cLentghFormatted[x + 1] = sLength[x]; 
                    
                }
                sLength = new string(cLentghFormatted);
                double dKM = Convert.ToDouble(sLength); // Convert to double for Rounding
                dKM = Math.Round(dKM, 1); // Round to one digit
                oIncidents[iIndex].LengthOfDelay = dKM.ToString() + "km"; // Set the new formatted value
            }
        }
        private void GetSeverity(ref Incident[] oIncidents, ref JArray oInc, ref int iIndex)
        {
            // Severity
            switch (Convert.ToInt32(((JValue)oInc[iIndex].SelectToken("ty")).Value))
                //switch (value)
            {
                case 0:
                    oIncidents[iIndex].Severity = IncidentTypes.Severity.NO_DELAY;
                    break;
                case 1:
                    oIncidents[iIndex].Severity = IncidentTypes.Severity.SLOW_TRAFFIC;
                    break;
                case 2:
                    oIncidents[iIndex].Severity = IncidentTypes.Severity.QUEUING_TRAFFIC;
                    break;
                case 3:
                    oIncidents[iIndex].Severity = IncidentTypes.Severity.STATIONARY_TRAFFIC;
                    break;
                case 4:
                    oIncidents[iIndex].Severity = IncidentTypes.Severity.CLOSED;
                    break;
                default:
                    oIncidents[iIndex].Severity = IncidentTypes.Severity.UNKNOWN;
                    break;
            }
        }

        private void GetType(ref Incident[] oIncidents, ref JArray oInc, ref int iIndex)
        {
            // Type of incident
            switch (Convert.ToInt32(((JValue)oInc[iIndex].SelectToken("ic")).Value))
            {
                case 3:
                    oIncidents[iIndex].Type = IncidentTypes.Type.ACCIDENT_CLEARED;
                    break;
                case 6:
                    oIncidents[iIndex].Type = IncidentTypes.Type.TRAFFIC_JAM;
                    break;
                case 7:
                    oIncidents[iIndex].Type = IncidentTypes.Type.ROADWORK;
                    break;
                case 8:
                    oIncidents[iIndex].Type = IncidentTypes.Type.ACCIDENT;
                    break;
                case 9:
                    oIncidents[iIndex].Type = IncidentTypes.Type.LONG_TERM_ROADWORK;
                    break;
                default:
                    oIncidents[iIndex].Type = IncidentTypes.Type.UNKNOWN;
                    break;
            }
        }


        /// <summary>
        /// Get Json as String from given URL
        /// </summary>
        /// <param name="sUrl"></param>
        /// <returns></returns>
        private async Task<string> getJsonFromURL(string sUrl)
        {
            HttpClient client = new HttpClient();
            string sResult = String.Empty;
            var _response = client.GetAsync(sUrl).Result;

            if (_response.IsSuccessStatusCode == true)
            {
                sResult = await _response.Content.ReadAsStringAsync();
            }
            client.Dispose();
            return sResult;
        }
    }
}
