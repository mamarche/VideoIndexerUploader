using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Diagnostics;
using System.Resources;
using System.Collections.Generic;
using VideoIndexerUploader.Models;

namespace VideoIndexerUploader
{
    public static class Search
    {
        [FunctionName("Search")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string apiUrl = Environment.GetEnvironmentVariable("VideoIndexerApiUrl");
            string location = Environment.GetEnvironmentVariable("VideoIndexerLocation");
            string apiKey = Environment.GetEnvironmentVariable("VideoIndexerApiKey");
            string accountId = Environment.GetEnvironmentVariable("VideoIndexerAccountId");
            string language = Environment.GetEnvironmentVariable("VideoIndexerLanguage");

            System.Net.ServicePointManager.SecurityProtocol = System.Net.ServicePointManager.SecurityProtocol | System.Net.SecurityProtocolType.Tls12;


            string keywords = req.Query["keywords"];

            var results = new List<VideoIndexData>();

            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                var accountAccessTokenRequestResult = await client.GetAsync($"{apiUrl}/auth/{location}/Accounts/{accountId}/AccessToken?allowEdit=true");
                var accountAccessToken = accountAccessTokenRequestResult.Content.ReadAsStringAsync().Result.Replace("\"", "");

                //search for videos by text
                var searchVideosRequestResult = await client.GetAsync($"{apiUrl}/{location}/Accounts/{accountId}/Videos/Search?sourceLanguage={language}&query={keywords}&accessToken={accountAccessToken}");
                var videosResult = searchVideosRequestResult.Content.ReadAsStringAsync().Result;

                var searchResult = JsonConvert.DeserializeObject<SearchResult>(videosResult);

                foreach (var videoObject in searchResult.results)
                {
                    Console.WriteLine($"{videoObject.name}");
                    var indexRequestResult = await client.GetAsync($"{apiUrl}/{location}/Accounts/{accountId}/Videos/{videoObject.id}/Index?accessToken={accountAccessToken}");

                    if (indexRequestResult.IsSuccessStatusCode)
                    {

                        var captionResult = await indexRequestResult.Content.ReadAsStringAsync();

                        var videoIndex = JsonConvert.DeserializeObject<VideoIndexData>(captionResult);
                        results.Add(videoIndex);

                    }
                }

                client.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");
            }

            return new OkObjectResult(results);
        }
    }
}
