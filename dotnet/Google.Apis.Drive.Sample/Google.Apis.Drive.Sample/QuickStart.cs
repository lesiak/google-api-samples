using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DriveQuickstart
{
    static class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "Drive API .NET Quickstart";

        static async Task<int> Main(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client-secret-oauth2.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            //ListFiles(service);
            await foreach (var file in GetFilesAsync(service))
            {
                Console.WriteLine("{0} ({1})", file.Name, file.Id);
            }

            return 0;
        }

        private static void ListFiles(DriveService service)
        {
            string? pageToken = null;
            do
            {
                FilesResource.ListRequest listRequest = service.Files.List();
                listRequest.PageSize = 100;
                listRequest.Fields = "nextPageToken, files(id, name)";
                listRequest.Q = "name contains 'Programista'";
                listRequest.PageToken = pageToken;
                listRequest.IncludeTeamDriveItems = false;
                listRequest.IncludeItemsFromAllDrives = false;

                // List files.
                FileList fileList = listRequest.Execute();
                IList<Google.Apis.Drive.v3.Data.File> files = fileList.Files;
                var nextToken = fileList.NextPageToken;
                PrintFiles(files);

                pageToken = nextToken;
            } while (pageToken != null);
        }
        
        private static async IAsyncEnumerable<Google.Apis.Drive.v3.Data.File> GetFilesAsync(DriveService service)
        {
            string? pageToken = null;
            do
            {
                FilesResource.ListRequest listRequest = service.Files.List();
                listRequest.PageSize = 100;
                listRequest.Fields = "nextPageToken, files(id, name)";
                listRequest.Q = "name contains 'Programista'";
                listRequest.PageToken = pageToken;
                listRequest.IncludeTeamDriveItems = false;
                listRequest.IncludeItemsFromAllDrives = false;

                // List files.
                FileList fileList = await listRequest.ExecuteAsync();
                IList<Google.Apis.Drive.v3.Data.File> files = fileList.Files;
                var nextToken = fileList.NextPageToken;
                foreach (var f in files)
                    yield return f;

                pageToken = nextToken;
            } while (pageToken != null);
        }

        static void PrintFiles(IList<Google.Apis.Drive.v3.Data.File>? files)
        {
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