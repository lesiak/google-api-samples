﻿using Google.Apis.Drive.v3;
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

            using FileStream fs = System.IO.File.OpenRead(sourcePath);
            FilesResource.CreateMediaUpload creteRequest = driveService.Files.Create(fileMetadata, fs, contentType);
            creteRequest.Fields = "id, parents";
            creteRequest.ProgressChanged += progress =>
                Console.WriteLine($"Upload status: {progress.Status} Bytes sent: {progress.BytesSent}");
            var uploadProgress = creteRequest.Upload();
            Console.WriteLine(uploadProgress.Status);
        }


        private static void ListFiles(DriveService driveService)
        {
            // Define parameters of request.
            FilesResource.ListRequest listRequest = driveService.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name, webViewLink)";


            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;
            Console.WriteLine("Files:");
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Console.WriteLine("{0} ({1}): {2}", file.Name, file.Id, file.WebViewLink);
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