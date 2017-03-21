using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace This_sol_on_Mars
{
    public class Curiosity
    {
        private const string BASE_URL = "http://marsweather.ingenology.com/v1/";

        public static bool Closing = false;

        public static Task<Report> GetLatestData()
        {
            return Task.Factory.StartNew(() =>
            {
                var client = new RestClient(BASE_URL);

                var request = new RestRequest("latest/", Method.GET);

                request.AddParameter("format", "json");
                Report report = new Report();

                IRestResponse<RootObject> response = client.Execute<RootObject>(request);
                if (response.Data != null)
                {
                    report = response.Data.report;
                }
                else
                {
                    throw new Exception(response.StatusCode.ToString());
                }

                return report;
            });
        }

        public static Task<List<Report>> GetArchiveData()
        {
            return Task.Factory.StartNew(() =>
           {
                var client = new RestClient(BASE_URL);

                var request = new RestRequest("archive/", Method.GET);

                request.AddParameter("format", "json");
                int page = 1;
                request.AddParameter("page", page);

                List<Report> reports = new List<Report>();

                IRestResponse<RootObject> response = null;

                do
                {
                    response = client.Execute<RootObject>(request);
                    reports.AddRange(response.Data.results);
                    request.Parameters.Single(x => x.Name == "page").Value = ++page;   

                } while (response.Data.next != null && !Closing);

                return reports;
           });
        }

        public static IEnumerable<RootObject> GetArchiveDataY()
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest("archive/", Method.GET);

            request.AddParameter("format", "json");
            int page = 1;
            request.AddParameter("page", page);

            RootObject reports = new RootObject();

            IRestResponse<RootObject> response = null;

            do
            {
                response = client.Execute<RootObject>(request);
                reports = response.Data;
                request.Parameters.Single(x => x.Name == "page").Value = ++page;
                yield return reports;

            } while (response.Data.next != null && !Closing);
        }

        public static Task<List<Report>> GetArchiveDataAsync(int page = 1)
        {
            return Task.Factory.StartNew(() =>
            {
                List<Report> reports = new List<Report>();

                var client = new RestClient(BASE_URL);

                var request = new RestRequest("archive/", Method.GET);

                request.AddParameter("format", "json");
                request.AddParameter("page", page.ToString());

                var asyncHandler = client.ExecuteAsync<RootObject>(request, r =>
                {
                    if (r.ResponseStatus == ResponseStatus.Completed && r.StatusCode == HttpStatusCode.OK)
                    {
                        reports.AddRange(r.Data.results);
                    }
                });
                
                return reports;
            });
        }
    }
}
