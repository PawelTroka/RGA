using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Xml;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;

namespace RGA.Helpers.RouteGeneration
{
    public class MapQuestAPI
    {
        private const string key = "Fmjtd%7Cluurn16anq%2C85%3Do5-9wtsga";

        private string distanceMatrixUrl = "http://www.mapquestapi.com/directions/v2/routematrix?key=" + key + "&avoids=Toll road&locale=pl";

        private string optimizeRouteUrl = "http://www.mapquestapi.com/directions/v2/optimizedroute?key=" + key + "&avoids=Toll road&locale=pl" + "&outFormat=" + "xml";

        public MapQuestAPI()
        {
        }


        public double[,] getDistanceMatrix(List<string> addresses, RouteOptimizationType type)
        {
            var request = (HttpWebRequest)WebRequest.Create(distanceMatrixUrl);
            request.ContentType = "text/json";
            request.Method = "POST";
            var arr = addresses.ToArray();
            for (int i = 0; i < arr.Length; i++)
                arr[i] = RemoveDiacritics(arr[i]);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    locations = arr,
                    options = new {allToAll=true}
                });

                streamWriter.Write(json);
            }

            var response = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                double[,] ret = new double[,] {};

                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(result);

                if(type==RouteOptimizationType.Distance)
                    ret = JsonConvert.DeserializeObject<double[,]>(dictionary["distance"]);
                else if(type==RouteOptimizationType.Time)
                    ret = JsonConvert.DeserializeObject<double[,]>(dictionary["time"]);

                return ret;
            }
        }



        public int[] getOptimalRoute(List<string> addresses)
        {
            var request = (HttpWebRequest)WebRequest.Create(optimizeRouteUrl);
            request.ContentType = "text/json";
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                var arr = addresses.ToArray();

                for (int i = 0; i < arr.Length; i++)
                    arr[i] = RemoveDiacritics(arr[i]);

                string json = new JavaScriptSerializer().Serialize(new
                {
                    locations = arr
                });

                streamWriter.Write(json);
            }

            var response = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(result); // suppose that myXmlString contains "<Names>...</Names>"

                var sequence = xml.GetElementsByTagName("locationSequence")[0].InnerText;

                var elementsStr = sequence.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);

                var optimalSequence = new List<int>();
                elementsStr.ForEach(s => optimalSequence.Add(int.Parse(s)));

                return optimalSequence.ToArray();
            }

            


        }



        public static string RemoveDiacritics(string stIn)
        {
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }
    }
}