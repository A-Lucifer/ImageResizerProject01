using Azure.Storage.Blobs;
using System;
using System.Threading.Tasks;
using System.Web;
using Azure.Storage.Blobs.Specialized;

namespace AzureProject01
{
    public class ImageService
    {
        //tempImageName variable to store imagename 
        private static string tempImageName;
        //Connection string of azure storage account
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=projectstorageaccount02;AccountKey=TdqIeYVgfakszQyLAtSU2IiihroZ9BSlVK3Pd8Zg5QcR3b5OzgruxVW7HQwWY2uiNqBK6+inGKIpLJLIYoQv/Q==;EndpointSuffix=core.windows.net";
        
        //Method to upload image in container
        public async Task<string> UploadImageAsync(HttpPostedFileBase imageToUpload)
        {
            string imageFullPath = null;
            if (imageToUpload == null || imageToUpload.ContentLength == 0)
            {
                return null;
            }
            try
            {
                //Instance of BlobServiceClient to connect with storage account using connection string
                BlobServiceClient blobService = new BlobServiceClient(connectionString);

                //Condition to check if input image exist in container or not
                //If exist then it will return the url of resized image 
                //else image will get stored in container "original-image"
                if (blobService.GetBlobContainerClient("thumbnail-image").GetBlockBlobClient(imageToUpload.FileName.ToString()).Exists())
                {
                    return blobService.GetBlobContainerClient("thumbnail-image").GetBlockBlobClient(imageToUpload.FileName).Uri.AbsoluteUri.ToString();
                }
                else
                {
                    BlobContainerClient container = blobService.GetBlobContainerClient("original-image");
                    BlockBlobClient blockBlob = container.GetBlockBlobClient(imageToUpload.FileName.ToString());
                    await blockBlob.UploadAsync(imageToUpload.InputStream);
                    imageFullPath = blockBlob.Uri.AbsoluteUri.ToString();

                    string imageName = imageToUpload.FileName;
                    tempImageName = imageName;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            //return url of original image uploaded in container
            return imageFullPath;   
        }

        //Method to fetch image from "thumbnail-image" container
        public string FetchImage()
        {
            string imageFullPath = null;
            try
            {
                //First it will make connection with "thumbnail-image" container of storage account
                //BlobContainerClient instance will get refernce of blob from the name of image we provide i.e. tempImageName
                //This method will return url of resized image from container "thumbnail-image"
                BlobContainerClient container = new BlobContainerClient(connectionString, "thumbnail-image");
                BlockBlobClient blockBlob = container.GetBlockBlobClient(tempImageName.ToString());
                imageFullPath = blockBlob.Uri.AbsoluteUri.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return imageFullPath;
        }
    }
}