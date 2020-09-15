using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using File = Google.Apis.Drive.v3.Data.File;

namespace Google.Apis.Drive.Sample
{
    internal static class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        private static readonly string[] Scopes = {DriveService.Scope.Drive};
        private const string ApplicationName = "Drive API .NET Quickstart";

        public static void Main(string[] args)
        {
            var credential = ServiceAccountCredentialProvider.GetServiceAccountCredentialFromEnv(
                "DRIVE_CREDENTIALS",
                Scopes,
                null);

            // Create Drive API service.
            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var targetFolderId = Environment.GetEnvironmentVariable("TARGET_FOLDER_ID");
            if (targetFolderId == null)
            {
                throw new ArgumentException($"Missing environment variable TARGET_FOLDER_ID");
            }
            var targetName = "photo.jpg";
            var sourcePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)}/testPhoto.jpg";
            var contentType = "image/jpeg";

            CreateFile(driveService, targetFolderId, targetName, sourcePath, contentType);
            ListFiles(driveService);
            //DeleteFile(driveService, fileId);
            
        }

        private static void CreateFile(DriveService driveService,
            string targetFolderId,
            string targetName,
            string sourcePath,
            string contentType)
        {
            File fileMetadata = new File
            {
                Name = targetName,
                Parents = new List<string> {targetFolderId}
            };
            string? uploadedFileId = null;
            using FileStream fs = System.IO.File.OpenRead(sourcePath);
            FilesResource.CreateMediaUpload createRequest = driveService.Files.Create(fileMetadata, fs, contentType);
            createRequest.Fields = "id, parents";
            createRequest.ProgressChanged += progress =>
                Console.WriteLine($"Upload status: {progress.Status} Bytes sent: {progress.BytesSent}");
            createRequest.ResponseReceived += file =>
            {
                uploadedFileId = file.Id;
                Console.WriteLine($"Created: {file.Id} parents: {file.Parents}");
            };
            var uploadProgress = createRequest.Upload();
            Console.WriteLine($"Final status: {uploadProgress.Status} {uploadedFileId}");
        }


        private static void ListFiles(DriveService driveService)
        {
            // Define parameters of request.
            FilesResource.ListRequest listRequest = driveService.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name, webViewLink)";


            // List files.
            IList<File> files = listRequest.Execute().Files;
            PrintFiles(files);
        }

        private static void PrintFiles(IList<File> files)
        {
            Console.WriteLine("Files:");
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Console.WriteLine($"{file.Name} ({file.Id}): {file.WebViewLink}");
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
        }

        private static void DeleteFile(DriveService driveService, string fileId)
        {
            driveService.Files.Delete(fileId).Execute();
        }
    }
}