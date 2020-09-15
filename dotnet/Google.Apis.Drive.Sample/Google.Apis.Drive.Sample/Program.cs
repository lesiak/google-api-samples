using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;

namespace Google.Apis.Drive.Sample
{
    internal static class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        private static readonly string[] Scopes = {DriveService.Scope.DriveReadonly};
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

            ListFiles(driveService);

            Console.Read();
        }
        
        

        private static void ListFiles(DriveService service)
        {
            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;
            Console.WriteLine("Files:");
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Console.WriteLine("{0} ({1})", file.Name, file.Id);
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
        }
        
    }
}