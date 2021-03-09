using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using SubmissionEvaluation.Classes.Config;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Classes.Config
{
    public static class SettingsParser
    {
        public static void InitializeSettings(string path)
        {
            if (Settings.Initialized)
            {
                return;
            }

            Settings.Application.PathToData = Path.IsPathRooted(path) ? path : Path.GetFullPath(path);

            var filename = Path.Combine(Path.GetFullPath(path), "settings.json");
            var builder = new ConfigurationBuilder().AddJsonFile(filename);
            var configuration = builder.Build();
            if (configuration == null)
            {
                throw new NullReferenceException("Configuration item is not initialized, file not found? " + filename);
            }

            /* Security Settings */
            Settings.SecurityToken = TryParseEntry(configuration, "Security", "SecurityToken",
                () => GetSecurityToken(Path.Combine(Path.GetFullPath(path), "security_token.txt")));

            /* Duplicate Check Settings */
            Settings.DuplicateCheckWindow = ParseEntry<int>(configuration, "Duplicate", "duplicate-check-window");
            /* Runtime Paths */
            Settings.Paths.Go = ParseEntry<string>(configuration, "ToolPaths", "Go");
            Settings.Paths.Java = ParseEntry<string>(configuration, "ToolPaths", "Java");
            Settings.Paths.NodeJS = ParseEntry<string>(configuration, "ToolPaths", "NodeJS");
            Settings.Paths.Perl = ParseEntry<string>(configuration, "ToolPaths", "Perl");
            Settings.Paths.Npm = ParseEntry<string>(configuration, "ToolPaths", "Npm");
            Settings.Paths.Python = ParseEntry<string>(configuration, "ToolPaths", "Python");
            Settings.Paths.Rust = ParseEntry<string>(configuration, "ToolPaths", "Rust");
            Settings.Paths.Scala = ParseEntry<string>(configuration, "ToolPaths", "Scala");
            Settings.Paths.Julia = ParseEntry<string>(configuration, "ToolPaths", "Julia");
            Settings.Paths.CppCheck = ParseEntry<string>(configuration, "ToolPaths", "CppCheck");
            Settings.Paths.DotNet = ParseEntry<string>(configuration, "ToolPaths", "DotNet");
            Settings.Paths.HaskellCompiler = ParseEntry<string>(configuration, "ToolPaths", "HaskellCompiler");
            Settings.Paths.CCompiler = ParseEntry<string>(configuration, "ToolPaths", "CCompiler");
            Settings.Paths.CppCompiler = ParseEntry<string>(configuration, "ToolPaths", "CppCompiler");
            Settings.Paths.CMake = ParseEntry<string>(configuration, "ToolPaths", "CMake");
            Settings.Paths.Maven = ParseEntry<string>(configuration, "ToolPaths", "Maven");
            Settings.Paths.ScalaCompiler = ParseEntry<string>(configuration, "ToolPaths", "ScalaCompiler");
            Settings.Paths.RustCargo = ParseEntry<string>(configuration, "ToolPaths", "RustCargo");
            Settings.Paths.TypeScriptCompiler = ParseEntry<string>(configuration, "ToolPaths", "TypeScriptCompiler");
            Settings.Paths.KotlinCompiler = ParseEntry<string>(configuration, "ToolPaths", "KotlinCompiler");

            /* Application Settings */
            Settings.Application.Delaytime = ParseEntry<int>(configuration, "Application", "Delaytime");
            Settings.Application.Inactive = ParseEntry<bool>(configuration, "Application", "Inactive");
            Settings.Application.InstancePort = ParseEntry<int>(configuration, "Application", "InstancePort");
            Settings.Application.InstanceName = ParseEntry<string>(configuration, "Application", "InstanceName");
            Settings.Application.WebApiPassphrase = ParseEntry<string>(configuration, "Application", "WebApiPassphrase");
            Settings.Application.PathToServerWwwRoot = ParseEntry<string>(configuration, "Application", "PathToServerWwwRoot");
            Settings.Application.PathToLogger = ParseEntry<string>(configuration, "Application", "PathToLogger");
            Settings.Application.SiteUrl = ParseEntry<string>(configuration, "Application", "BaseUrl");

            /* Mail Settings */
            Settings.Mail.Username = ParseEntry<string>(configuration, "Mail", "Username");
            Settings.Mail.Password = ParseEntry<string>(configuration, "Mail", "Password");
            Settings.Mail.SmtpServer = ParseEntry<string>(configuration, "Mail", "SmtpServer");
            Settings.Mail.SendMailAddress = ParseEntry<string>(configuration, "Mail", "SendMailAddress");
            Settings.Mail.HelpMailAddress = ParseEntry<string>(configuration, "Mail", "HelpMailAddress");

            /* Ldap Settings */
            Settings.Ldap.AccessPassword = ParseEntry<string>(configuration, "Ldap", "AccessPassword");
            Settings.Ldap.AccessUser = ParseEntry<string>(configuration, "Ldap", "AccessUser");
            Settings.Ldap.Domain = ParseEntry<string>(configuration, "Ldap", "Domain");
            Settings.Ldap.Ip = ParseEntry<string>(configuration, "Ldap", "Ip");
            Settings.Ldap.OrganizationalUnit = ParseEntry<string>(configuration, "Ldap", "OrganizationalUnit");
            Settings.Ldap.Port = ParseEntry<int>(configuration, "Ldap", "Port");
            Settings.Ldap.UidAttribute = ParseEntry<string>(configuration, "Ldap", "UidAttribute");

            /* Features Settings */
            Settings.Features.EnableSendMail = ParseEntry<bool>(configuration, "Features", "EnableSendMail");
            Settings.Features.EnableAutoUpdate = ParseEntry<bool>(configuration, "Features", "EnableAutoUpdate");
            Settings.Features.EnableReview = ParseEntry<bool>(configuration, "Features", "EnableReview");
            Settings.Features.EnableReviewerPromotion = ParseEntry<bool>(configuration, "Features", "EnableReviewerPromotion");
            Settings.Features.EnableRating = ParseEntry<bool>(configuration, "Features", "EnableRating");
            Settings.Features.EnableEffortEstamination = ParseEntry<bool>(configuration, "Features", "EnableEffortEstamination");
            Settings.Features.EnableAchievements = ParseEntry<bool>(configuration, "Features", "EnableAchivements");
            Settings.Features.EnableLdap = ParseEntry<bool>(configuration, "Features", "EnableLdap");
            Settings.Features.RequiresMemberActivation = ParseEntry<bool>(configuration, "Features", "RequiresMemberActivation");

            /* Permission Settings */
            var permissions = configuration.GetSection("Permissions");

            // Creator
            var parsedCreatorPermissions = permissions.GetSection("Creator").GetChildren();
            var creatorDict = parsedCreatorPermissions.ToDictionary(x => x.Key, x => x.GetChildren().ToArray().Select(y => y.Value).ToList());
            Settings.Permissions.CreatorPermissions.ViewPermissions = GetValueOrDefault(creatorDict, "ViewPermissions");
            Settings.Permissions.CreatorPermissions.CreatePermissions = GetValueOrDefault(creatorDict, "CreatePermissions");
            Settings.Permissions.CreatorPermissions.EditPermissions = GetValueOrDefault(creatorDict, "EditPermissions");

            // GroupAdmin
            var parsedGroupAdminPermissions = permissions.GetSection("GroupAdmin").GetChildren();
            var groupAdminDict = parsedGroupAdminPermissions.ToDictionary(x => x.Key, x => x.GetChildren().ToArray().Select(y => y.Value).ToList());
            Settings.Permissions.GroupAdminPermissions.ViewPermissions = GetValueOrDefault(groupAdminDict, "ViewPermissions");
            Settings.Permissions.GroupAdminPermissions.CreatePermissions = GetValueOrDefault(groupAdminDict, "CreatePermissions");
            Settings.Permissions.GroupAdminPermissions.EditPermissions = GetValueOrDefault(groupAdminDict, "EditPermissions");

            // Reviewer
            var parsedReviewerPermissions = permissions.GetSection("GroupReviewer").GetChildren();
            var reviewerDict = parsedReviewerPermissions.ToDictionary(x => x.Key, x => x.GetChildren().ToArray().Select(y => y.Value).ToList());
            Settings.Permissions.GroupReviewerPermissions.ViewPermissions = GetValueOrDefault(reviewerDict, "ViewPermissions");
            Settings.Permissions.GroupReviewerPermissions.CreatePermissions = GetValueOrDefault(reviewerDict, "CreatePermissions");
            Settings.Permissions.GroupReviewerPermissions.EditPermissions = GetValueOrDefault(reviewerDict, "EditPermissions");

            /* Registration Messages */
            Settings.Authentication.RegistrationMessage = TryParseEntry(configuration, "Authentication", "RegistrationMessage", () => string.Empty);

            /* Customization */
            var customization = configuration.GetSection("Customization");
            var parsedRatingMethods = customization.GetSection("RatingMethods").GetChildren().Cast<ConfigurationSection>();
            Settings.Customization.RatingMethods = parsedRatingMethods.ToDictionary(key =>
            {
                Enum.TryParse(key.Key, true, out RatingMethod ratingMethod);
                return ratingMethod;
            }, key =>
            {
                Enum.TryParse(key.Key, true, out RatingMethod ratingMethod);
                return new RatingMethodConfig {Type = ratingMethod, Title = key["title"], Color = key["color"]};
            });

            var parsedCategories = customization.GetSection("Categories").GetChildren().Cast<ConfigurationSection>();
            Settings.Customization.Categories = parsedCategories.ToDictionary(x => x.Key, x => x["title"]);

            var parsedAchievements = customization.GetSection("Achievements").GetChildren().Cast<ConfigurationSection>();
            Settings.Customization.Achievements =
                parsedAchievements.ToDictionary(x => x.Key, x => new AchievementConfig {Title = x["titel"], Description = x["description"]});

            var parsedResults = customization.GetSection("Results").GetChildren().Cast<ConfigurationSection>();
            Settings.Customization.Results = parsedResults.ToDictionary(x => x.Key, x => new ResultConfig {Description = x["description"], Color = x["color"]});
            Settings.Initialized = true;
        }

        private static string GetSecurityToken(string securityTokenFilePath)
        {
            void EnsureSecurityTokenFileExists(string path)
            {
                if (File.Exists(path) && File.ReadAllLines(path).Length <= 0)
                {
                    return;
                }

                var writer = File.CreateText(path);
                writer.Write(Guid.NewGuid().ToString("N"));
                writer.Close();
            }

            string ReadSecurityTokenFromFile(string path)
            {
                return File.ReadAllLines(path)[0];
            }

            EnsureSecurityTokenFileExists(securityTokenFilePath);
            return ReadSecurityTokenFromFile(securityTokenFilePath);
        }

        private static string TryParseEntry(IConfiguration configuration, string section, string key, Func<string> onEntryIsNullOrEmpty)
        {
            var settingsValue = (string) Convert.ChangeType(configuration.GetSection(section)[key], typeof(string));
            return string.IsNullOrEmpty(settingsValue) ? onEntryIsNullOrEmpty() : settingsValue;
        }

        private static T ParseEntry<T>(IConfiguration configuration, string section, string key)
        {
            var settingsValue = (T) Convert.ChangeType(configuration.GetSection(section)[key], typeof(T));
            if (settingsValue == null)
            {
                throw new NullReferenceException($"Property {section}.{key} not found in JSON file!");
            }

            return settingsValue;
        }

        private static List<string> GetValueOrDefault(IReadOnlyDictionary<string, List<string>> dict, string key)
        {
            dict.TryGetValue(key, out var holder);
            return holder ?? new List<string>();
        }
    }
}
