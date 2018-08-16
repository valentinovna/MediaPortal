using MediaPortal.Data.Repositories;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPortal.BL.Services;
using ThumbnailCreatorWebJob.Model;
using MediaPortal.BL.Interface;

namespace ThumbnailCreatorWebJob.Listeners
{
    public class ThumbnailListener
    {
        public CloudQueue Queue { get; private set; }

        public CloudBlobContainer BlobContainer { get; set; }
        
        private IFileSystemService _fileSystemService;

        public ThumbnailListener(IFileSystemService fileSystemService)
        {
            Queue = GetQueueReference();
            BlobContainer = GetContainerReference();
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            
            _fileSystemService = fileSystemService;

        }

        public  void Listen()
        {
            Console.WriteLine("Thumbnail listener started listen:");
            while (true)
            {
                // Get message frim queue; message will be invisible for 5 seconds
                //CloudQueueMessage message = Queue.GetMessage(new TimeSpan(0, 0, 10));
                var messages = Queue.GetMessages(32);

                Parallel.ForEach(messages, message =>
                {
                    FileIdBlobModel fileIdBlob = JsonConvert.DeserializeObject<FileIdBlobModel>(message.AsString);

                    Console.WriteLine(message.AsString);
                    try 
                    {
                    var thumbnailLink = CreateThumbnailAsync(fileIdBlob.BlobLink).Result;
                    Queue.DeleteMessage(message);

                    UpdateFileSystem(fileIdBlob.FileId, thumbnailLink);
                    }
                    catch (Exception ex)
                    {
                    }
                    
                });

            }
        }

        private void UpdateFileSystem(int id, string uri)
        {
            var cuttedUri = uri.Replace(ConfigurationManager.AppSettings.Get("azureStorageBlobLink"), "");
            _fileSystemService.UpdateThumbnail(id, cuttedUri);
        }

        private async Task<string> CreateThumbnailAsync(string blobUri)
        {
            // TODO : PUT THIS CONNECTION STRING TO APP SETTINGS
            string connectionString = ConfigurationManager.ConnectionStrings["azureConnection"].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudBlockBlob blob = new CloudBlockBlob(new Uri(blobUri), storageAccount.Credentials);
            var thumbnailImage = await ResizeBlobAsync(blob).ConfigureAwait(false);

            if (thumbnailImage.Length > 0)
            {
                var guidName = Guid.NewGuid().ToString();
                var blobName = guidName + ".png";
                //var blobName = guidName + Path.GetExtension(blob.Name);

                CloudBlockBlob blobThumbnail = BlobContainer.GetBlockBlobReference(blobName);
                blobThumbnail.DeleteIfExists();

                await blobThumbnail.UploadFromByteArrayAsync(thumbnailImage, 0, thumbnailImage.Length).ConfigureAwait(false);
                return blobThumbnail.Uri.ToString();
            }

            return null;
        }

        private async Task<byte[]> ResizeBlobAsync(CloudBlockBlob blob)
        {
            byte[] thumbnailImage = new byte[0];
            var type = Path.GetExtension(blob.Name).ToLower();
            if (type.Equals(".png") || type.Equals(".jpg") || type.Equals(".jpeg"))
            {
                using (var imageStream = await blob.OpenReadAsync())
                {
                    thumbnailImage = GetResizedImageFromStream(imageStream);
                    return thumbnailImage;
                }
            }
            else if (type.Equals(".mp4"))
            {
                using (var file = await blob.OpenReadAsync())
                {
                    using (MemoryStream thumbnailStream = GetVideoFrame(file, blob.Name))
                    {
                        thumbnailImage = GetResizedImageFromStream(thumbnailStream);
                        return thumbnailImage;
                    }
                }
            }
            else
            {
                return thumbnailImage;
            }
        }

        private byte[] GetResizedImageFromStream(Stream imageStream)
        {
            byte[] resizedImage = new byte[0];
            using (Image img = Image.FromStream(imageStream))
            {
                int h = 200;
                int w = 200;

                using (Bitmap b = new Bitmap(img, new Size(w, h)))
                {
                    using (MemoryStream thumbnailStream = new MemoryStream())
                    {
                        b.Save(thumbnailStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        resizedImage = thumbnailStream.ToArray();
                    }
                }
            }
            return resizedImage;
        }

        private MemoryStream GetVideoFrame(Stream videoStream, string filename)
        {
            var tempPathToFile = @"..\..\" + filename;
            using (FileStream output = new FileStream(tempPathToFile, FileMode.Create))
            {
                videoStream.CopyTo(output);
            }
            MemoryStream thumbnailStream = new MemoryStream();
            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            ffMpeg.GetVideoThumbnail(tempPathToFile, thumbnailStream, 0);
            if (File.Exists(tempPathToFile))
            {
                File.Delete(tempPathToFile);
            }
            return thumbnailStream;

        }

        private CloudBlobContainer GetContainerReference()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=teamresponse200;AccountKey=lk2ZcpWPmltfWQClFaesuTs01+8zSvv1yOm1UsjsHXMBc42OkFc/41jf7P3DGvlwa2EgicYPVFPKs55OPWo4/Q==";
            string ContainerName = "filesystem";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            container.CreateIfNotExists();

            return container;
        }

        private CloudQueue GetQueueReference()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=teamresponse200;AccountKey=lk2ZcpWPmltfWQClFaesuTs01+8zSvv1yOm1UsjsHXMBc42OkFc/41jf7P3DGvlwa2EgicYPVFPKs55OPWo4/Q==";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("filesthumbnailsqueue");
            queue.CreateIfNotExists();

            return queue;
        }
    }
}
