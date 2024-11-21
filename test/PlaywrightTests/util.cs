using System.Diagnostics;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests
{
    // util class used to mainly start our localhost server. For us this means running dotnet run, and start our application.

    public static class MyEndToEndUtil
    {
        public static async Task<Process> StartServer()
        {

            // Get path needed for dotnet run
            var projectpath = @"../../../../../src/Chirp.Web";   //Ensure this matches your path locally
            // start the process and run our project
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "run --no-build", //Remove flag: --environment Production, when testing locally
                    WorkingDirectory = projectpath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            };

            // Since our project takes a while to run, we want to ensure 
            // that test suite does not begin before the server(localhost) is ready.

            var serverReady = new TaskCompletionSource<bool>();

            // reads data from our process - specifically we look for outputs from dotnet, 
            //that gives information on how far the building process has gone

            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    Console.WriteLine(args.Data);

                    if (args.Data.Contains("Now listening on") || args.Data.Contains("Application started"))
                    {
                        // when we receive information indicating that server is ready we set the Taskcompletion to true
                        serverReady.TrySetResult(true);
                    }
                };


            };
            // start process
            process.Start();
            // read outputs from process
            process.BeginOutputReadLine();

            var timeout = Task.Delay(30000); // 30 seconds timeout

            // this will either complete when 30 seconds has gone by, or when server.ready has been set to true
            // effectively ensuring that our server is ready before it is passed on to tests.
            var completedTask = await Task.WhenAny(serverReady.Task, timeout);

            if (completedTask == timeout)
            {
                throw new TimeoutException("The server did not start within the expected time.");
            }
            return process;
        }
    }
};






