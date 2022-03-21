using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
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
                throw new NullReferenceException($"Configuration item is not initialized, file not found? {filename}");
            }

            /* Security Settings */
            Settings.SecurityToken = TryParseEntry(configuration, "Security", nameof(Settings.SecurityToken),
                () => GetSecurityToken(Path.Combine(Path.GetFullPath(path), "security_token.txt")));

            /* Duplicate Check Settings */
            Settings.DuplicateCheckWindow = ParseEntry<int>(configuration, "Duplicate", nameof(Settings.DuplicateCheckWindow));
            /* Runtime Paths */
            Settings.ToolPaths.Go = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.Go));
            Settings.ToolPaths.Dart = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.Dart));
            Settings.ToolPaths.Java = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.Java));
            Settings.ToolPaths.Node = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.Node));
            Settings.ToolPaths.Perl = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.Perl));
            Settings.ToolPaths.Npm = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.Npm));
            Settings.ToolPaths.Python = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.Python));
            Settings.ToolPaths.Tu = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.Tu));
            Settings.ToolPaths.Rust = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.Rust));
            Settings.ToolPaths.Scala = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.Scala));
            Settings.ToolPaths.Julia = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.Julia));
            Settings.ToolPaths.CppCheck = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.CppCheck));
            Settings.ToolPaths.DotNet = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.DotNet));
            Settings.ToolPaths.HaskellCompiler = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.HaskellCompiler));
            Settings.ToolPaths.CCompiler = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.CCompiler));
            Settings.ToolPaths.CppCompiler = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.CppCompiler));
            Settings.ToolPaths.CMake = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.CMake));
            Settings.ToolPaths.Maven = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.Maven));
            Settings.ToolPaths.ScalaCompiler = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.ScalaCompiler));
            Settings.ToolPaths.RustCargo = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.RustCargo));
            Settings.ToolPaths.TypeScriptCompiler = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.TypeScriptCompiler));
            Settings.ToolPaths.KotlinCompiler = ParseEntry<string>(configuration, nameof(Settings.ToolPaths), nameof(Settings.ToolPaths.KotlinCompiler));

            /* Application Settings */
            Settings.Application.DelayTime = ParseEntry<int>(configuration, nameof(Settings.Application), nameof(Settings.Application.DelayTime));
            Settings.Application.Inactive = ParseEntry<bool>(configuration, nameof(Settings.Application), nameof(Settings.Application.Inactive));
            Settings.Application.InstancePort = ParseEntry<int>(configuration, nameof(Settings.Application), nameof(Settings.Application.InstancePort));
            Settings.Application.InstanceName = ParseEntry<string>(configuration, nameof(Settings.Application), nameof(Settings.Application.InstanceName));
            Settings.Application.WebApiPassphrase = ParseEntry<string>(configuration, nameof(Settings.Application), nameof(Settings.Application.WebApiPassphrase));
            Settings.Application.PathToServerWwwRoot = ParseEntry<string>(configuration, nameof(Settings.Application), nameof(Settings.Application.PathToServerWwwRoot));
            Settings.Application.SiteUrl = ParseEntry<string>(configuration, nameof(Settings.Application), nameof(Settings.Application.SiteUrl));
            Settings.Application.DeleteAfterMoreThenOneYearInactivity = ParseEntry<bool>(configuration, nameof(Settings.Application), nameof(Settings.Application.DeleteAfterMoreThenOneYearInactivity));

            /* Mail Settings */
            Settings.Mail.Username = ParseEntry<string>(configuration, nameof(Settings.Mail), nameof(Settings.Mail.Username));
            Settings.Mail.Password = ParseEntry<string>(configuration, nameof(Settings.Mail), nameof(Settings.Mail.Password));
            Settings.Mail.SmtpServer = ParseEntry<string>(configuration, nameof(Settings.Mail), nameof(Settings.Mail.SmtpServer));
            Settings.Mail.SendMailAddress = ParseEntry<string>(configuration, nameof(Settings.Mail), nameof(Settings.Mail.SendMailAddress));
            Settings.Mail.HelpMailAddress = ParseEntry<string>(configuration, nameof(Settings.Mail), nameof(Settings.Mail.HelpMailAddress));
            Settings.Mail.Port = ParseEntry<int>(configuration, nameof(Settings.Mail), nameof(Settings.Mail.Port));

            /* Ldap Settings */
            Settings.Ldap.AccessPassword = ParseEntry<string>(configuration, nameof(Settings.Ldap), nameof(Settings.Ldap.AccessPassword));
            Settings.Ldap.AccessUser = ParseEntry<string>(configuration, nameof(Settings.Ldap), nameof(Settings.Ldap.AccessUser));
            Settings.Ldap.Domain = ParseEntry<string>(configuration, nameof(Settings.Ldap), nameof(Settings.Ldap.Domain));
            Settings.Ldap.Ip = ParseEntry<string>(configuration, nameof(Settings.Ldap), nameof(Settings.Ldap.Ip));
            Settings.Ldap.OrganizationalUnit = ParseEntry<string>(configuration, nameof(Settings.Ldap), nameof(Settings.Ldap.OrganizationalUnit));
            Settings.Ldap.Port = ParseEntry<int>(configuration, nameof(Settings.Ldap), nameof(Settings.Ldap.Port));
            Settings.Ldap.UidAttribute = ParseEntry<string>(configuration, nameof(Settings.Ldap), nameof(Settings.Ldap.UidAttribute));

            /* Features Settings */
            Settings.Features.EnableSendMail = ParseEntry<bool>(configuration, nameof(Settings.Features), nameof(Settings.Features.EnableSendMail));
            Settings.Features.EnableAutoUpdate = ParseEntry<bool>(configuration, nameof(Settings.Features), nameof(Settings.Features.EnableAutoUpdate));
            Settings.Features.EnableReview = ParseEntry<bool>(configuration, nameof(Settings.Features), nameof(Settings.Features.EnableReview));
            Settings.Features.EnableReviewerPromotion = ParseEntry<bool>(configuration, nameof(Settings.Features), nameof(Settings.Features.EnableReviewerPromotion));
            Settings.Features.EnableRating = ParseEntry<bool>(configuration, nameof(Settings.Features), nameof(Settings.Features.EnableRating));
            Settings.Features.EnableEffortEstimation = ParseEntry<bool>(configuration, nameof(Settings.Features), nameof(Settings.Features.EnableEffortEstimation));
            Settings.Features.EnableAchievements = ParseEntry<bool>(configuration, nameof(Settings.Features), nameof(Settings.Features.EnableAchievements));
            Settings.Features.EnableLdap = ParseEntry<bool>(configuration, nameof(Settings.Features), nameof(Settings.Features.EnableLdap));
            Settings.Features.RequiresMemberActivation = ParseEntry<bool>(configuration, nameof(Settings.Features), nameof(Settings.Features.RequiresMemberActivation));

            /* Permission Settings */
            var permissions = configuration.GetSection(nameof(Settings.Permissions));

            // Creator
            var parsedCreatorPermissions = permissions.GetSection(nameof(Settings.Permissions.Creator)).GetChildren();
            var creatorDict = parsedCreatorPermissions.ToDictionary(x => x.Key, x => x.GetChildren().Select(y => y.Value).ToList());
            Settings.Permissions.Creator.ViewPermissions = GetValueOrDefault(creatorDict, nameof(Settings.Permissions.Creator.ViewPermissions));
            Settings.Permissions.Creator.CreatePermissions = GetValueOrDefault(creatorDict, nameof(Settings.Permissions.Creator.CreatePermissions));
            Settings.Permissions.Creator.EditPermissions = GetValueOrDefault(creatorDict, nameof(Settings.Permissions.Creator.EditPermissions));

            // GroupAdmin
            var parsedGroupAdminPermissions = permissions.GetSection(nameof(Settings.Permissions.GroupAdmin)).GetChildren();
            var groupAdminDict = parsedGroupAdminPermissions.ToDictionary(x => x.Key, x => x.GetChildren().Select(y => y.Value).ToList());
            Settings.Permissions.GroupAdmin.ViewPermissions = GetValueOrDefault(groupAdminDict, nameof(Settings.Permissions.GroupAdmin.ViewPermissions));
            Settings.Permissions.GroupAdmin.CreatePermissions = GetValueOrDefault(groupAdminDict, nameof(Settings.Permissions.GroupAdmin.CreatePermissions));
            Settings.Permissions.GroupAdmin.EditPermissions = GetValueOrDefault(groupAdminDict, nameof(Settings.Permissions.GroupAdmin.EditPermissions));

            // Reviewer
            var parsedReviewerPermissions = permissions.GetSection(nameof(Settings.Permissions.GroupReviewer)).GetChildren();
            var reviewerDict = parsedReviewerPermissions.ToDictionary(x => x.Key, x => x.GetChildren().Select(y => y.Value).ToList());
            Settings.Permissions.GroupReviewer.ViewPermissions = GetValueOrDefault(reviewerDict, nameof(Settings.Permissions.GroupReviewer.ViewPermissions));
            Settings.Permissions.GroupReviewer.CreatePermissions = GetValueOrDefault(reviewerDict, nameof(Settings.Permissions.GroupReviewer.CreatePermissions));
            Settings.Permissions.GroupReviewer.EditPermissions = GetValueOrDefault(reviewerDict, nameof(Settings.Permissions.GroupReviewer.EditPermissions));

            /* Registration Messages */
            Settings.Authentication.RegistrationMessage = TryParseEntry(configuration, nameof(Settings.Authentication), nameof(Settings.Authentication.RegistrationMessage), () => string.Empty);

            /* Customization */
            var customization = configuration.GetSection(nameof(Settings.Customization));
            var parsedRatingMethods = customization.GetSection(nameof(Settings.Customization.RatingMethods)).GetChildren().Cast<ConfigurationSection>();
            Settings.Customization.RatingMethods = parsedRatingMethods.ToDictionary(key =>
            {
                Enum.TryParse(key.Key, true, out RatingMethod ratingMethod);
                return ratingMethod;
            }, key =>
            {
                Enum.TryParse(key.Key, true, out RatingMethod ratingMethod);
                return new RatingMethodConfig {Type = ratingMethod, Title = key[nameof(RatingMethodConfig.Title)], Color = key[nameof(RatingMethodConfig.Color)]};
            });

            var parsedCategories = customization.GetSection(nameof(Settings.Customization.Categories)).GetChildren().Cast<ConfigurationSection>();
            Settings.Customization.Categories = parsedCategories.ToDictionary(x => x.Key, x => x["Title"]);

            var parsedAchievements = customization.GetSection(nameof(Settings.Customization.Achievements)).GetChildren().Cast<ConfigurationSection>();
            Settings.Customization.Achievements =
                parsedAchievements.ToDictionary(x => x.Key, x => new AchievementConfig {Title = x[nameof(AchievementConfig.Title)], Description = x[nameof(AchievementConfig.Description)]});

            var parsedResults = customization.GetSection(nameof(Settings.Customization.Results)).GetChildren().Cast<ConfigurationSection>();
            Settings.Customization.Results = parsedResults.ToDictionary(x => x.Key, x => new ResultConfig {Description = x[nameof(ResultConfig.Description)], Color = x[nameof(ResultConfig.Color)]});

            // Creator
            var parsedIgnoreFiles = configuration.GetSection(nameof(Settings.Customization)).GetSection(nameof(Settings.Customization.IgnoreFiles)).GetChildren();
            var parsedIgnoreFilesDict = parsedIgnoreFiles.ToDictionary(x => x.Key, x => x.GetChildren().Select(y => y.Value).ToList());
            Settings.Customization.IgnoreFiles = GetValueOrDefault(parsedIgnoreFilesDict, "Filenames");

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
