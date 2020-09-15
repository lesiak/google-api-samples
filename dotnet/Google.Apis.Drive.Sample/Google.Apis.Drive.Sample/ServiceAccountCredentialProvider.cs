using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Json;

namespace Google.Apis.Drive.Sample
{
    internal static class ServiceAccountCredentialProvider
    {
         internal static ServiceAccountCredential GetServiceAccountCredentialFromFile(
            string pathToJsonFile,
            IEnumerable<string> scopes,
            string? emailToImpersonate
            )
        {
            // Load and deserialize credential parameters from the specified JSON file.
            JsonCredentialParameters parameters;
            using (Stream json = new FileStream(pathToJsonFile, FileMode.Open, FileAccess.Read))
            {
                parameters = NewtonsoftJsonSerializer.Instance.Deserialize<JsonCredentialParameters>(json);
            }

            return ServiceAccountCredential(parameters, scopes, emailToImpersonate);
        }
         
         internal static ServiceAccountCredential GetServiceAccountCredentialFromEnv(
             string envVariableName,
             IEnumerable<string> scopes,
             string? emailToImpersonate)
         {
             var credentialsJson = Environment.GetEnvironmentVariable(envVariableName);
             if (credentialsJson == null)
             {
                 throw new ArgumentException($"Missing environment variable {envVariableName}");
             }

             return GetServiceAccountCredentialFromString(credentialsJson, scopes, emailToImpersonate);
         }
         
         internal static ServiceAccountCredential GetServiceAccountCredentialFromString(
             string credentialsJson,
             IEnumerable<string> scopes,
             string? emailToImpersonate)
         {
             // Load and deserialize credential parameters from the specified JSON file.
             var parameters = NewtonsoftJsonSerializer.Instance.Deserialize<JsonCredentialParameters>(credentialsJson);
             return ServiceAccountCredential(parameters, scopes, emailToImpersonate);
         }

         static ServiceAccountCredential ServiceAccountCredential(
            JsonCredentialParameters credentialParameters, 
            IEnumerable<string> scopes,
            string? emailToImpersonate)
        {
            // Create a credential initializer with the correct scopes.
            var initializer = new ServiceAccountCredential.Initializer(credentialParameters.ClientEmail)
            {
                Scopes = scopes
            };

            // Configure impersonation (if applicable).
            if (!string.IsNullOrEmpty(emailToImpersonate))
            {
                initializer.User = emailToImpersonate;
            }

            // Create a service account credential object using the deserialized private key.
            var credential = new ServiceAccountCredential(initializer.FromPrivateKey(credentialParameters.PrivateKey));
            return credential;
        }
    }
}