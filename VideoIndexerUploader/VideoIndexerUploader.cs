using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using VideoIndexerUploader.Helpers;
using VideoIndexerUploader.Models;

namespace VideoIndexerUploader
{
    public static class VideoIndexerUploader
    {
        [FunctionName("VideoIndexerCheckState")]
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            //video id
            string videoId = context.GetInput<string>();

            int pollingInterval = Convert.ToInt32(Environment.GetEnvironmentVariable("VideoIndexerCheckStatusPollingInterval"));
            DateTime expiryTime = DateTime.Now.AddMinutes(Convert.ToInt32(Environment.GetEnvironmentVariable("VideoIndexerChechStatusExpiryTime")));

            while (context.CurrentUtcDateTime < expiryTime)
            {
                var videoIndexData = await context.CallActivityAsync<VideoIndexData>("VideoIndexerUploader_GetIndexingStatus", videoId);
                if (videoIndexData == null || videoIndexData.state == "Processed")
                {
                    // Perform an action when a condition is met.
                    await context.CallActivityAsync("VideoIndexerUploader_SaveVideoIndexData", videoIndexData);
                    break;
                }

                // Orchestration sleeps until this time.
                var nextCheck = context.CurrentUtcDateTime.AddSeconds(pollingInterval);
                await context.CreateTimer(nextCheck, CancellationToken.None);
            }

            // Perform more work here, or let the orchestration end.

            return "Indexing for search completed";
        }

        [FunctionName("VideoIndexerUploader_GetIndexingStatus")]
        public static async Task<VideoIndexData> GetIndexingStatusAsync([ActivityTrigger] string videoId, ILogger log)
        {
            string apiUrl = Environment.GetEnvironmentVariable("VideoIndexerApiUrl");
            string location = Environment.GetEnvironmentVariable("VideoIndexerLocation");
            string apiKey = Environment.GetEnvironmentVariable("VideoIndexerApiKey");
            string accountId = Environment.GetEnvironmentVariable("VideoIndexerAccountId");

            VideoIndexData videoIndexData;

            System.Net.ServicePointManager.SecurityProtocol = System.Net.ServicePointManager.SecurityProtocol | System.Net.SecurityProtocolType.Tls12;
            string processingState = null;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                var accountAccessTokenRequestResult = await client.GetAsync($"{apiUrl}/auth/{location}/Accounts/{accountId}/AccessToken?allowEdit=true");
                var accountAccessToken = accountAccessTokenRequestResult.Content.ReadAsStringAsync().Result.Replace("\"", "");

                var videoGetIndexRequestResult = await client.GetAsync($"{apiUrl}/{location}/Accounts/{accountId}/Videos/{videoId}/Index?accessToken={accountAccessToken}&language=English");
                string videoGetIndexResult = await videoGetIndexRequestResult.Content.ReadAsStringAsync();

                videoIndexData = JsonConvert.DeserializeObject<VideoIndexData>(videoGetIndexResult);

                processingState = videoIndexData.state;

                Console.WriteLine("");
                Console.WriteLine("State:");
                Console.WriteLine(processingState);
            }

            return videoIndexData;
        }

        [FunctionName("VideoIndexerUploader_SaveVideoIndexData")]
        public static async void SaveVideoIndexData([ActivityTrigger] VideoIndexData videoIndexData, ILogger log)
        {
            string conStr = System.Environment.GetEnvironmentVariable($"ConnectionStrings:AzureWebJobsVideoStorage", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = System.Environment.GetEnvironmentVariable($"CUSTOMCONNSTR_AzureWebJobsVideoStorage", EnvironmentVariableTarget.Process);

            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(conStr);

            //Create a unique name for the container
            string containerName = Environment.GetEnvironmentVariable("VideoIndexDataContainerName");

            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = containerClient.GetBlobClient(videoIndexData.id);

            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(videoIndexData));
            MemoryStream uploadFileStream = new MemoryStream(byteArray);
            await blobClient.UploadAsync(uploadFileStream);
            uploadFileStream.Close();

        }

        [FunctionName("VideoIndexerUploader_BlobTriggerStart")]
        public static async void BlobTriggerStart(
            [BlobTrigger("holomaintenance-video/{name}", Connection = "VideoStorage")]Stream myBlob, string name, Uri Uri,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            string apiUrl = Environment.GetEnvironmentVariable("VideoIndexerApiUrl");
            string location = Environment.GetEnvironmentVariable("VideoIndexerLocation");
            string apiKey = Environment.GetEnvironmentVariable("VideoIndexerApiKey");
            string accountId = Environment.GetEnvironmentVariable("VideoIndexerAccountId");

            System.Net.ServicePointManager.SecurityProtocol = System.Net.ServicePointManager.SecurityProtocol | System.Net.SecurityProtocolType.Tls12;
            string videoId = "";

            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                var accountAccessTokenRequestResult = await client.GetAsync($"{apiUrl}/auth/{location}/Accounts/{accountId}/AccessToken?allowEdit=true");
                var accountAccessToken = accountAccessTokenRequestResult.Content.ReadAsStringAsync().Result.Replace("\"", "");

                //Generate SAS TOKEN for the BLOB
                string containerUrl = $"https://{Environment.GetEnvironmentVariable("VideoBlobAccountName")}.blob.core.windows.net/{Environment.GetEnvironmentVariable("VideoBlobContainerName")}";
                var blobContainer = new CloudBlobContainer(new Uri(containerUrl),
                    new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(Environment.GetEnvironmentVariable("VideoBlobAccountName"), Environment.GetEnvironmentVariable("VideoBlobKey")));

                var videoUrlWithSasToken = BlobHelpers.GetBlobSasUri(blobContainer, name);

                // upload a video from URL
                Console.WriteLine("Uploading...");
                var videoUrl = HttpUtility.UrlEncode(videoUrlWithSasToken); // video URL

                var uploadRequestResult = await client.PostAsync($"{apiUrl}/{location}/Accounts/{accountId}/Videos?accessToken={accountAccessToken}&name={name}&description=uploaded_by_HoloMaintenance&privacy=private&partition=some_partition&videoUrl={videoUrl}", null);
                var uploadResult = await uploadRequestResult.Content.ReadAsStringAsync();


                // get the video id from the upload result
                videoId = JsonConvert.DeserializeObject<dynamic>(uploadResult)["id"];
                Console.WriteLine("Uploaded");
                Console.WriteLine("Video ID: " + videoId);
            }

            //call orchestrator with the video id
            string instanceId = null;
            if (!string.IsNullOrEmpty(videoId))
            {
                instanceId = await starter.StartNewAsync("VideoIndexerCheckState", videoId);
                log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            }
            
        }

        
    }
}