using Microsoft.Playwright;
using System.Diagnostics;

namespace WanderlustJournal.Tests;

[TestClass]
public class PlaywrightFixture
{
    private static Process? _dotnetProcess;
    private static bool _serverStarted = false;
    
    // BaseUrl is static and can be accessed by tests if needed
    public static string BaseUrl => "http://localhost:5026";

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext _)
    {
        StartWebServer();
        
        // Install Playwright browsers if needed
        var exitCode = Microsoft.Playwright.Program.Main(new[] { "install" });
        if (exitCode != 0)
        {
            throw new Exception($"Playwright exited with code {exitCode}");
        }
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        StopWebServer();
    }

    private static void StartWebServer()
    {
        if (_serverStarted)
        {
            return;
        }

        var projectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "WanderlustJournal.5"));
        
        _dotnetProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run",
                WorkingDirectory = projectPath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        _dotnetProcess.OutputDataReceived += (sender, args) => 
        {
            if (args.Data != null && args.Data.Contains("Application started"))
            {
                _serverStarted = true;
            }
            Console.WriteLine($"[ASP.NET] {args.Data}");
        };
        
        _dotnetProcess.ErrorDataReceived += (sender, args) => 
        {
            if (args.Data != null)
            {
                Console.WriteLine($"[ASP.NET ERROR] {args.Data}");
            }
        };

        _dotnetProcess.Start();
        _dotnetProcess.BeginOutputReadLine();
        _dotnetProcess.BeginErrorReadLine();

        // Wait for the server to start
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));
        var serverStartedTask = Task.Run(() =>
        {
            while (!_serverStarted)
            {
                Task.Delay(100).Wait();
            }
        });

        Task.WhenAny(timeoutTask, serverStartedTask).GetAwaiter().GetResult();

        if (!_serverStarted)
        {
            throw new Exception("Failed to start the web server within the timeout period.");
        }

        // Additional delay to ensure the server is fully ready
        Task.Delay(TimeSpan.FromSeconds(2)).Wait();
    }

    private static void StopWebServer()
    {
        if (_dotnetProcess != null && !_dotnetProcess.HasExited)
        {
            try
            {
                _dotnetProcess.Kill();
                _dotnetProcess.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping web server: {ex.Message}");
            }
        }
    }
}