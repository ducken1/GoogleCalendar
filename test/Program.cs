using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;

class Program
{
    private static readonly string ClientSecretPath = Path.Combine(GetBaseDirectory(), "client_secret.json");
    private const string ApplicationName = "test";

    private static string GetBaseDirectory()
    {
        // Navigate up two levels from the current directory to get rid of bin\Debug or bin\Release
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(baseDirectory).FullName).FullName).FullName).FullName);
    }
    static void Main(string[] args)
    {
        Console.WriteLine(GetBaseDirectory());
        var user = "dukenotest@gmail.com";
        CreateUserEvent(user);
    }

    private static void CreateUserEvent(string userEmail)
    {
        UserCredential credential = AuthenticateUser(userEmail);

        var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        Event newEvent = new Event
        {
            Summary = "Frizer BlaBla",
            Description = "Moško striženje + prameni",
            Start = new EventDateTime { DateTime = DateTime.Now },
            End = new EventDateTime { DateTime = DateTime.Now.AddHours(1) },
        };

        Event createdEvent = service.Events.Insert(newEvent, "primary").Execute();

        Console.WriteLine($"Event created in {userEmail}'s calendar: {createdEvent.Summary}");
    }

    private static UserCredential AuthenticateUser(string userEmail)
    {
        using (var stream = new FileStream(ClientSecretPath, FileMode.Open, FileAccess.Read))
        {
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                new[] { CalendarService.Scope.Calendar },
                userEmail, 
                System.Threading.CancellationToken.None,
                new FileDataStore(GetUserCredentialsPath(userEmail), true)).Result;

            return credential;
        }
    }

    private static string GetUserCredentialsPath(string userEmail)
    {

        string baseDirectory = GetBaseDirectory();
        return Path.Combine(baseDirectory, "user_credentials", userEmail);
    }
}
