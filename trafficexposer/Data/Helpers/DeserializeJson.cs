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
        private string API_KEY;
        public DeserializeJson(string APIKEY)
        {
            API_KEY = APIKEY;
        }

        /// <summary>
        /// Returns data on Array of all found Cities from given Name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Location[]> getCitiesAsync(string Name)
        {
            Location[] Locs;
            string data = await getJsonFromURL($"https://api.tomtom.com/search/2/geocode/{Name}.json?key={API_KEY}");

            if (!String.IsNullOrEmpty(data))
            {
                JObject joResponse = JObject.Parse(data);
                JArray results = (JArray)joResponse["results"];

                 Locs = new Location[results.Count];

                for (int i = 0; i < results.Count; i++)
                {
                    JObject addr = (JObject)results[i]["address"];
                    JObject pos = (JObject)results[i]["position"];
                    Locs[i].Name = Convert.ToString(((JValue)addr.SelectToken("freeformAddress")).Value); // Location Name
                    Locs[i].Longitude = Convert.ToString(((JValue)pos.SelectToken("lon")).Value); //Location cords
                    Locs[i].Latitude = Convert.ToString(((JValue)pos.SelectToken("lat")).Value);

                }
            }
            else
            {
                throw new Exception("No Data found");
            }
            return Locs;
        }

        /// <summary>
        /// Returns an Array of incidents in the area between two given locations
        /// </summary>
        /// <param name="Loc1"></param>
        /// <param name="Loc2"></param>
        /// <returns></returns>
        public async Task<Incident[]> getTrafficInformationAsync(Location Loc1, Location Loc2)
        {
            Incident[] Incidents = null;
            if (!String.IsNullOrEmpty(Loc1.Longitude) && // Check if the Data is valid for Exception prevention
                !String.IsNullOrEmpty(Loc1.Latitude) &&
                !String.IsNullOrEmpty(Loc2.Latitude) &&
                !String.IsNullOrEmpty(Loc2.Longitude))
            {


                Format(ref Loc1, ref Loc2); // Format Numbers to US Format
                string data = await getJsonFromURL($"https://api.tomtom.com/traffic/services/4/incidentDetails/s3/{Loc1.Latitude},{Loc1.Longitude},{Loc2.Latitude},{Loc2.Longitude}/10/-1/json?key={API_KEY}&projection=EPSG4326");

                if (!String.IsNullOrEmpty(data))
                {
                    JObject joResponse = JObject.Parse(data);
                    JObject tm = (JObject)joResponse["tm"];
                    JArray inc = (JArray)tm["poi"];

                    Incidents = new Incident[inc.Count];

                    for (int i = 0; i < inc.Count; i++)
                    {
                        if (!((JValue)inc[i].SelectToken("id")).Value.ToString().Contains("CLUSTER") &&
                            !String.IsNullOrEmpty(((JValue)inc[i].SelectToken("f")).Value.ToString()) &&
                            !String.IsNullOrEmpty(((JValue)inc[i].SelectToken("t")).Value.ToString())) //Basic filtering of invalid/empty data which are returned by the APU&& !String.IsNullOrEmpty(((JValue)inc[i].SelectToken("f")).Value.ToString())I
                        {
                            await GetType(Incidents, inc, i);
                            TimeSpan AgeOfAccident = DateTime.Parse(DateTime.Now.ToString()).Subtract(DateTime.Parse(Convert.ToString(((JValue)inc[i].SelectToken("sd")).Value)));
                            if (Incidents[i].Type != IncidentTypes.Type.ACCIDENT && AgeOfAccident.Days > 1) // Remove old accidents returned by the API
                            {
                                JObject pos = (JObject)inc[i]["p"];
                                Incidents[i].LocX = Convert.ToDouble(((JValue)pos.SelectToken("x")).Value); // Location of the Incident
                                Incidents[i].LocY = Convert.ToDouble(((JValue)pos.SelectToken("y")).Value);

                                //Incidents[i].AdditionalInfo = Convert.ToString(((JValue)inc[i].SelectToken("c")).Value); // Info like "one line closed"

                                Incidents[i].Description = Convert.ToString(((JValue)inc[i].SelectToken("d")).Value); // desc ex. "road closed"
                                Incidents[i].SinceTime = Convert.ToString(((JValue)inc[i].SelectToken("sd")).Value); // since when its closed
                                Incidents[i].From = Convert.ToString(((JValue)inc[i].SelectToken("f")).Value); // from city
                                Incidents[i].To = Convert.ToString(((JValue)inc[i].SelectToken("t")).Value); // to city
                                Incidents[i].LengthOfDelay = Convert.ToString(((JValue)inc[i].SelectToken("l")).Value); //Lentgh of the Delay in KM


                                await GetSeverity(Incidents, inc, i);
                            }
                        }
                    }

                }
            }
            return Incidents;
        }

        private static void Format(ref Location Loc1, ref Location Loc2)
        {
            // Format Numbers to US Format
            Loc1.Longitude = Loc1.Longitude.Replace(',', '.');
            Loc2.Longitude = Loc2.Longitude.Replace(',', '.');

            Loc1.Latitude = Loc1.Latitude.Replace(',', '.');
            Loc2.Latitude = Loc2.Latitude.Replace(',', '.');
        }

        private static async Task GetSeverity(Incident[] Incidents, JArray inc, int i)
        {
            // Severity
            switch (Convert.ToInt32(((JValue)inc[i].SelectToken("ty")).Value))
            {
                case 0:
                    Incidents[i].Severity = IncidentTypes.Severity.NO_DELAY;
                    break;
                case 1:
                    Incidents[i].Severity = IncidentTypes.Severity.SLOW_TRAFFIC;
                    break;
                case 2:
                    Incidents[i].Severity = IncidentTypes.Severity.QUEUING_TRAFFIC;
                    break;
                case 3:
                    Incidents[i].Severity = IncidentTypes.Severity.STATIONARY_TRAFFIC;
                    break;
                case 4:
                    Incidents[i].Severity = IncidentTypes.Severity.CLOSED;
                    break;
                default:
                    Incidents[i].Severity = IncidentTypes.Severity.UNKNOWN;
                    break;
            }
        }

        private static async Task GetType(Incident[] Incidents, JArray inc, int i)
        {
            // Type of incident
            switch (Convert.ToInt32(((JValue)inc[i].SelectToken("ic")).Value))
            {
                case 3:
                    Incidents[i].Type = IncidentTypes.Type.ACCIDENT_CLEARED;
                    break;
                case 6:
                    Incidents[i].Type = IncidentTypes.Type.TRAFFIC_JAM;
                    break;
                case 7:
                    Incidents[i].Type = IncidentTypes.Type.ROADWORK;
                    break;
                case 8:
                    Incidents[i].Type = IncidentTypes.Type.ACCIDENT;
                    break;
                case 9:
                    Incidents[i].Type = IncidentTypes.Type.LONG_TERM_ROADWORK;
                    break;
                default:
                    Incidents[i].Type = IncidentTypes.Type.UNKNOWN;
                    break;
            }
        }


        /// <summary>
        /// Get Json as String from given URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<string> getJsonFromURL(string url)
        {
            HttpClient client = new HttpClient();
            string res = String.Empty;
            var response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode == true)
            {
                res = await response.Content.ReadAsStringAsync();
            }
            client.Dispose();
            return res;
        }
    }
}
