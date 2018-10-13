using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using GoogleMaps.LocationServices;
using Microsoft.AspNetCore.Mvc;
using TripAssistantResultsAPI.Models;

namespace TripAssistantResultsAPI.Controllers
{
    [Route("api/[controller]")]
    public class GeoLocationController : Controller
    {
        [HttpGet]
        // [HttpGet("{context}")]

        public List<GeoLocation> Results([FromQuery]string context)
        {


            string[] keywords = context.Split(" ");
            string city = "";
            List<GeoLocation> lst = new List<GeoLocation>();

            double latitude = 0;
            if (keywords.Length >= 1)
            {
                city = keywords[0];
                using (var client = new WebClient())
                {
                    string url = "https://maps.googleapis.com/maps/api/place/autocomplete/xml?input=" + city + "&types=geocode&language=fr&key=AIzaSyD2bL_pYSzue4JkSDQg4fYSuVT8XA_bjCQ";
                    var jsonPrediction = client.DownloadString(url);
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(url);
                    XmlNodeList elementList = xmldoc.GetElementsByTagName("description");
                    var locationService = new GoogleLocationService();


                    for (int i = 0; i < 3; i++)
                    {
                        GeoLocation address = new GeoLocation();
                        string str = elementList[i].InnerXml;
                        var Location = locationService.GetLatLongFromAddress(str);
                        address.latitude = Location.Latitude;
                        address.longitude = Location.Longitude;
                        address.add = str;


                        lst.Add(address);
                    }




                }
            }
            return lst;
          
        }

    }
}