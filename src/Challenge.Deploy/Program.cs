using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;

namespace Challenge.Deploy
{
    internal static class Program
    {
        private static string key;

        private static int Main(string[] args)
        {
            key = args[0];
            var force = args.Length > 1 && args[1] == "--force";
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            InstanceDescription newInstance = null;
            InstanceDescription currentInstance = null;
            try
            {
                Log("Getting current instance state");
                var state = GetInstanceState();

                Log("Determining new instance");
                currentInstance = GetCurrenttInstance(state);
                newInstance = GetNewInstance(currentInstance);
                Log($"{currentInstance.Name} -> {newInstance.Name}");
                Log($"{currentInstance.Path} -> {newInstance.Path}");
                if (currentInstance.Path == newInstance.Path)
                {
                    throw new ArgumentException("Same directory, aborting!");
                }

                Log("Download latest build");
                var workingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var latestBuildZip = Path.Combine(workingDir, "latest_build.zip");

                DownloadBuild(latestBuildZip, state, force);

                Log("Moving inactive instance");
                RemoveInstance(newInstance);

                Log("Deploying new instance");
                DeployRelease(latestBuildZip, newInstance);

                Log("Set maintenance mode for current instance");
                EnableMaintenanceMode(currentInstance);

                Log("Copy data files to new instance");
                CopyDataFiles(currentInstance, newInstance);

                Log("Configuring new instance");
                ConfigureInstance(currentInstance, newInstance);

                Log("Starting new instance");
                StartInstance(newInstance);

                Log("Set maintenance mode for new instance");
                EnableMaintenanceMode(newInstance);

                Log("Health check for new instance");
                CheckInstanceHealth(newInstance, state);

                try
                {
                    Log("Activate new instance");
                    ActivateInstance(newInstance);

                    Log("Disable Maintenance Mode for new instance");
                    SetMaintenanceMode(newInstance, false);

                    Log("Killing current Instance");
                    ShutdownInstance(currentInstance);

                    Log("Done");
                }
                catch (Exception e)
                {
                    Log(e);
                }

                return 0;
            }
            catch (Exception e)
            {
                Log(e);
                try
                {
                    if (currentInstance != null)
                    {
                        SetMaintenanceMode(currentInstance, false);
                    }
                }
                catch (Exception exception)
                {
                    Log(exception.Message);
                }

                try
                {
                    if (newInstance != null)
                    {
                        ShutdownInstance(newInstance);
                    }
                }
                catch (Exception exception)
                {
                    Log(exception.Message);
                }
            }

            return -1;
        }

        private static void DownloadBuild(string latestBuildZip, State state, bool force = false)
        {
            var buildNumber = "";
            var currentBuild = state.Version?.Split('-').Last();
            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.Authorization] = "Basic a2U1ZXI1OlZUTzFxWUxsc2JjaUFZa1JMS2xM";
                var currentBuildNumber =
                    wc.DownloadString(
                        "https://teamcityg3.itd-intern.de/teamcity/httpAuth/app/rest/builds/buildType:InternalProjects_ItdChallenges_Build/number");
                if (currentBuild == currentBuildNumber)
                {
                    throw new Exception($"Build {currentBuildNumber} already running");
                }

                var data = wc.DownloadData(
                    @"https://teamcityg3.itd-intern.de/teamcity/httpAuth/repository/downloadAll/InternalProjects_ItdChallenges_Build/.lastSuccessful");
                File.WriteAllBytes(latestBuildZip, data);
                if (!string.IsNullOrEmpty(wc.ResponseHeaders["Content-Disposition"]))
                {
                    var match = Regex.Match(wc.ResponseHeaders["Content-Disposition"], "_ITD_Challenges_Build_(\\d+)_artifacts\\.zip");
                    if (match.Success)
                    {
                        buildNumber = match.Groups[1].Value;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(buildNumber))
            {
                throw new Exception("Artifact buildnumber can not be determinded!");
            }

            if (currentBuild == buildNumber)
            {
                if (force)
                {
                    Log($"Build {buildNumber} already running. Forcing deployment!");
                }
                else
                {
                    throw new Exception($"Build {buildNumber} already running");
                }
            }
        }

        private static void ActivateInstance(InstanceDescription instance)
        {
            File.WriteAllText(@"c:\challenges\ExternalTools\nginx\conf\proxy.conf", $"proxy_pass       http://localhost:{instance.Port};");

            foreach (var process in Process.GetProcessesByName("nginx"))
            {
                process.Kill();
            }

            var pi = new ProcessStartInfo("nginx.exe") {WorkingDirectory = "c:\\challenges\\ExternalTools\\nginx"};
            var proc = Process.Start(pi);

            Thread.Sleep(5000);
            if (proc.HasExited)
            {
                Log("!! Nginx not running !! Help !!");
            }
        }

        private static void ShutdownInstance(InstanceDescription instance)
        {
            var currentSettings =
                JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(File.ReadAllText(Path.Combine(instance.Path, "WebHost", "settings.json")));
            currentSettings["Application"]["Inactive"] = true;
            File.WriteAllText(Path.Combine(instance.Path, "WebHost", "settings.json"),
                JsonConvert.SerializeObject(currentSettings, new JsonSerializerSettings {Formatting = Formatting.Indented}));

            using var client = new HttpClient();
            var shutdownTask = client.PostAsync($"http://localhost:{instance.Port}/api/maintenance/shutdown/" + key, new StringContent(""));
            shutdownTask.Wait(60000);
            if (!shutdownTask.Result.IsSuccessStatusCode)
            {
                Log("Shutdown fehlgeschlagen. " + shutdownTask.Result.ReasonPhrase);

                if (instance.Process != null)
                {
                    Log("Killing process");
                    try
                    {
                        instance.Process.Kill();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static void CheckInstanceHealth(InstanceDescription instance, State expectedState)
        {
            State newState = null;
            for (var i = 0; i < 100; i++)
            {
                Thread.Sleep(20000);
                try
                {
                    newState = GetInstanceState("http://localhost:" + instance.Port);
                    if (newState.WebPagesGenerated)
                    {
                        break;
                    }
                }
                catch
                {
                }
            }

            if (newState?.Name != instance.Name)
            {
                throw new Exception("Wrong instance name, aborting!");
            }

            if (newState.Port != instance.Port)
            {
                throw new Exception("Wrong instance port, aborting!");
            }

            if (newState.ChallengesCount != expectedState.ChallengesCount)
            {
                throw new Exception($"Wrong challenges count {expectedState.ChallengesCount} != {newState.ChallengesCount}");
            }

            if (newState.MembersCount != expectedState.MembersCount)
            {
                throw new Exception($"Wrong members count {expectedState.MembersCount} != {newState.MembersCount}");
            }

            if (newState.SubmissionsCount != expectedState.SubmissionsCount)
            {
                throw new Exception($"Wrong submissions count {expectedState.SubmissionsCount} != {newState.SubmissionsCount}");
            }
        }

        private static void StartInstance(InstanceDescription instance)
        {
            var migrateProcess = Process.Start(Path.Combine(instance.Path, "WebHost", "ASPStandardJekyllVerwaltung.exe"), "migrate");
            if (migrateProcess?.WaitForExit(TimeSpan.FromHours(1).Milliseconds) == false)
            {
                migrateProcess.Kill();
                throw new Exception("Migration hangs, aborting!");
            }

            if (migrateProcess?.ExitCode != 0)
            {
                throw new Exception("Migration failed!");
            }

            var process = Process.Start(Path.Combine(instance.Path, "WebHost", "ASPStandardJekyllVerwaltung.exe"));
            instance.Process = process;
            Thread.Sleep(5000);
            if (process?.HasExited != false)
            {
                throw new Exception("Instance " + instance.Name + " not running, aborting");
            }
        }

        private static void ConfigureInstance(InstanceDescription currentInstance, InstanceDescription newInstance)
        {
            var currentDir = currentInstance.Path;
            var newDir = newInstance.Path;
            File.Copy(Path.Combine(currentDir, "WebHost", "appsettings.json"), Path.Combine(newDir, "WebHost", "appsettings.json"), true);
            File.Copy(Path.Combine(currentDir, "RawWebsite", "_config.yml"), Path.Combine(newDir, "RawWebsite", "_config.yml"), true);
            var settings = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(File.ReadAllText(Path.Combine(currentDir, "WebHost", "settings.json")));
            settings["Application"]["InstanceName"] = newInstance.Name;
            settings["Application"]["InstancePort"] = newInstance.Port;
            settings["Application"]["PathToChallengeDir"] = Path.Combine(newDir, "RawWebsite");
            settings["Application"]["PathToServerWwwRoot"] = Path.Combine(newDir, "WebHost", "wwwroot");
            settings["Application"]["PathToLogger"] = $"C:\\challenges\\Log_{newInstance.Name}.txt";
            settings["Application"]["Inactive"] = false;
            File.WriteAllText(Path.Combine(newDir, "WebHost", "settings.json"),
                JsonConvert.SerializeObject(settings, new JsonSerializerSettings {Formatting = Formatting.Indented}));
        }

        private static void CopyDataFiles(InstanceDescription currentInstance, InstanceDescription newInstance)
        {
            var dirsToCopy = new[]
            {
                new CopyOperation {Name = "_challenges"},
                new CopyOperation
                {
                    Name = "_data",
                    IncludeFiles = new[]
                    {
                        "activity\\.yml$", "awards\\.yml$", "achievements\\.yml$", "members\\.yml$", "tournamentMatches_(.)+\\.yml$",
                        "globalRanking\\.yml$", "semesterRanking\\.yml$"
                    }
                },
                new CopyOperation {Name = "_games"}, new CopyOperation {Name = "bundles"}, new CopyOperation {Name = "documents"},
                new CopyOperation
                {
                    Name = "_submissions",
                    IncludeFiles = new[] {"result\\.yml$", "result\\.old$", "Review\\.zip$", "Source\\.zip$", "review\\.pdf$", "failed_report\\.html$"},
                    ExcludeDirs = new[] {"built", "extracted", "src"}
                },
                new CopyOperation {Name = "_tournaments"},
                new CopyOperation {Name = "_tournamentEntries", IncludeFiles = new[] {"author\\.yml$", "Source\\.zip$"}}
            };

            foreach (var op in dirsToCopy)
            {
                CopyDirectory(Path.Combine(currentInstance.Path, "RawWebsite", op.Name), Path.Combine(newInstance.Path, "RawWebsite", op.Name), op.IncludeFiles,
                    op.ExcludeDirs);
            }
        }

        private static void EnableMaintenanceMode(InstanceDescription instance)
        {
            State state = null;
            SetMaintenanceMode(instance, true);
            for (var i = 0; i < 60; i++)
            {
                state = GetInstanceState();
                if (state.Maintenance)
                {
                    break;
                }

                Thread.Sleep(10000);
            }

            if (state?.Maintenance != true)
            {
                throw new Exception("Instance not in maintenance mode, aborting!");
            }
        }

        private static void DeployRelease(string pathToRelease, InstanceDescription instance)
        {
            using var stream = File.OpenRead(pathToRelease);
            using var archive = new ZipArchive(stream);
            archive.ExtractToDirectory(instance.Path);
            if (!Directory.Exists(instance.Path))
            {
                throw new Exception("Entpacken der Daten fehlgeschlagen. Ungültiges Zip?");
            }
        }

        private static void RemoveInstance(InstanceDescription instance)
        {
            if (Directory.Exists(instance.Path))
            {
                Directory.Move(instance.Path, $"{instance.Path}_{DateTime.Now.ToFileTime()}");
            }
        }

        private static InstanceDescription GetNewInstance(InstanceDescription currentInstance)
        {
            var newName = string.Compare(currentInstance.Name, "Green", true, CultureInfo.InvariantCulture) == 0 ? "Blue" : "Green";
            return new InstanceDescription
            {
                Name = newName,
                Path = currentInstance.Path.Replace($"\\{currentInstance.Name}", $"\\{newName}"),
                Port = newName == "Green" ? currentInstance.Port - 1 : currentInstance.Port + 1
            };
        }

        private static InstanceDescription GetCurrenttInstance(State state)
        {
            return new InstanceDescription {Name = state.Name, Path = state.Path, Port = state.Port};
        }

        private static void SetMaintenanceMode(InstanceDescription instance, bool enable)
        {
            var lastPhrase = "";
            using var client = new HttpClient();
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    var requestUri = $"http://localhost:{instance.Port}/api/maintenance/mode/{key}";
                    var maintenanceTask = client.PutAsync(requestUri, new StringContent(enable.ToString().ToLower(), Encoding.UTF8, "application/json"));
                    if (maintenanceTask.Result.IsSuccessStatusCode)
                    {
                        return;
                    }

                    lastPhrase = maintenanceTask.Result.ReasonPhrase;
                }
                catch
                {
                    Thread.Sleep(3000);
                }

                Log("Retry setting Maintenance Mode =" + enable);
            }

            throw new Exception("Maintenance mode can not be set, aborting! " + lastPhrase);
        }

        private static State GetInstanceState(string host = "https://localhost")
        {
            using var client = new HttpClient();
            var stateTask = client.GetStringAsync($"{host}/api/maintenance/state/{key}");
            var state = JsonConvert.DeserializeObject<State>(stateTask.Result);
            return state;
        }

        public static void CopyDirectory(string src, string dest, string[] opInclude, string[] opExcludeDirs)
        {
            void Copy(DirectoryInfo source, DirectoryInfo target)
            {
                if (opExcludeDirs?.Length > 0 && opExcludeDirs.Any(x => Regex.IsMatch(target.Name, x, RegexOptions.IgnoreCase)))
                {
                    return;
                }

                var filesToCopy = source.GetFiles();
                if (opInclude?.Length > 0)
                {
                    filesToCopy = source.EnumerateFiles().Where(x => opInclude.Any(inc => Regex.IsMatch(x.FullName, inc, RegexOptions.IgnoreCase))).ToArray();
                }

                if (filesToCopy.Length > 0)
                {
                    Directory.CreateDirectory(target.FullName);
                    foreach (var fi in filesToCopy)
                    {
                        fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                    }
                }

                foreach (var diSourceSubDir in source.EnumerateDirectories())
                {
                    Copy(diSourceSubDir, new DirectoryInfo(Path.Combine(target.FullName, diSourceSubDir.Name)));
                }
            }

            Copy(new DirectoryInfo(src), new DirectoryInfo(dest));
        }

        private static void Log(object msg)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: {msg}");
        }
    }
}
