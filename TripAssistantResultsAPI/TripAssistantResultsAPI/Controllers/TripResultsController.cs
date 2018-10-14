using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using GoogleMaps.LocationServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TripAssistantResultsAPI.Controllers
{
    //[Produces("application/json")]

    [Route("api/[controller]")]
    public class TripResultsController : Controller
    {
        [HttpGet]

        public List<JObject> Results([FromQuery]string context)
        {

            List<double> locations = new List<double>();
            List<JObject> obj = new List<JObject>();
            string[] keywords = context.Split(" ");
            string city = "";

            if (keywords.Length >= 1)
            {
                city = keywords[0];
                string url = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=" + city + "&key=AIzaSyD2bL_pYSzue4JkSDQg4fYSuVT8XA_bjCQ";

                locations = GetGeoLocation(url);


                string lat = locations[0].ToString();
                string lng = locations[1].ToString();



                return Post(lat, lng, keywords, city);
            }
                return obj;
        }
        public List<double> GetGeoLocation(string url)
        {
            int count = 0;
            List<double> locations = new List<double>();
            JObject value,location,geometry;
            double latitude = 0;
            double longitude = 0;
           
            using (var client = new WebClient())
            {
                var jsonPrediction = client.DownloadString(url);
                var data = (JObject)JsonConvert.DeserializeObject(jsonPrediction);
                var results = data["results"].Value<JArray>();

                foreach (JObject res in results)
                {

                    count++;
                    geometry = res["geometry"].Value<JObject>();
                    location = geometry["location"].Value<JObject>();
                    latitude += location["lat"].Value<Double>();
                    longitude += location["lng"].Value<Double>();
                }
              }
            locations.Add(latitude);
            locations.Add(longitude);
            locations.Add(count);
            return locations;
        }

        

        public List<JObject> Post(string lat, string lng, string[] keywords,string city)
        {
            List<JObject> totaldata = new List<JObject>();

            List<double> locations = new List<double>();
            JObject value;
            int count = 0;
            JObject location;
            JObject geometry;
            double latitude = 0;

            double longitude = 0;

            JObject data=new JObject();

            string url = "";
            using (var client = new WebClient())
            {
                if (keywords.Length == 1)
                {
                    url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + lat + "," + lng + "&radius=100000&keyword=attractions&key=AIzaSyD2bL_pYSzue4JkSDQg4fYSuVT8XA_bjCQ";
                   var  jsonPrediction = client.DownloadString(url);
                    data = (JObject)JsonConvert.DeserializeObject(jsonPrediction);
                    totaldata.Add(data);
                }

                else if (keywords.Length > 1)
                {
                    if (keywords[1] == "1")
                    {
                        url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + lat + "," + lng + "&radius=100000&keyword=attractions&key=AIzaSyD2bL_pYSzue4JkSDQg4fYSuVT8XA_bjCQ";
                        var jsonPrediction = client.DownloadString(url);
                        data = (JObject)JsonConvert.DeserializeObject(jsonPrediction);
                        totaldata.Add(data);

                    }
                    else
                    {
                        url = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=%22%20+%20things+to+do+in+" + city + "&key=AIzaSyD2bL_pYSzue4JkSDQg4fYSuVT8XA_bjCQ";
                        // url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=18.5204,73.8567&radius=300000&keyword=point%20of%20interest&key=AIzaSyD2bL_pYSzue4JkSDQg4fYSuVT8XA_bjCQ";
                        var jsonPrediction = client.DownloadString(url);
                        data = (JObject)JsonConvert.DeserializeObject(jsonPrediction);
                        totaldata.Add(data);
                        locations = GetGeoLocation(url);
                        latitude = locations[0] / locations[2];
                        longitude = locations[1] / locations[2];



                        data= GetHotels(latitude, longitude);
                        totaldata.Add(data);

                    }
                }
               
            }
            return totaldata;
        }

        public JObject GetHotels(double latitude, double longitude)
        {

             string url = "";
           // List<JObject> data = new List<JObject>();
            JObject data = new JObject();
            using (var client = new WebClient())
            {
                url = "  https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + latitude.ToString() + "," + longitude.ToString() + "&radius=100000&type=hotels&keyword=hotels&key=AIzaSyD2bL_pYSzue4JkSDQg4fYSuVT8XA_bjCQ";


                //url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + latitude.ToString() + "," + longitude.ToString() + "&radius=100000&keyword=attractions&key=AIzaSyD2bL_pYSzue4JkSDQg4fYSuVT8XA_bjCQ";
                var jsonPrediction = client.DownloadString(url);

                 data = (JObject)JsonConvert.DeserializeObject(jsonPrediction);

            }
            return data;
        }
    }
}


