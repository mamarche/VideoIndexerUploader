# VideoIndexerUploader
This demo project is aimed to demonstrate how to automatically upload videos from a Blob Storage to a Azure Video Indexer Service leveraging Azure Durable Functions.

In order to try the sample, you need:
- An Azure Subscription
- A Video Indexer Service
- A Storage Account with 2 containers

Fill the data into the "local.settings.json" file with your account names and keys and run the project locally.

When the functions are running, upload a video into the "video" contaniner. The function should be automatically triggered and the video will be uploaded to the Video Indexer Service. 

The durable function still polling the Video indexer Service checking the indexing state, until the state is "Processed".
Once the video is processed, the function will get all the index data and save it as a file into the "data" container.
