using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SubmissionEvaluation.Compilers.Helper
{
    internal class CMakeList
    {
        private List<string> commands;

        public CMakeList(string content)
        {
            commands = ConvertToActions(content);
        }

        public void SetExecutableOutputPath(string path)
        {
            commands = commands.Where(x => !x.Contains("SET(EXECUTABLE_OUTPUT_PATH")).ToList();
            commands.Add($"SET(EXECUTABLE_OUTPUT_PATH {path})");
        }

        public string GetContent()
        {
            return string.Join(Environment.NewLine, commands);
        }

        public IEnumerable<string> GetExecutableNames()
        {
            var executableLines = commands.Where(p => p.StartsWith("add_executable"));
            return executableLines.Select(p => p.Split(' ')[0]).Select(p => p.Replace("add_executable(", ""));
        }

        public void AddSourceToExecutables(string sourceName)
        {
            var seekingAddExecutable = true;
            for (var i = 0; i < commands.Count; i++)
            {
                var command = commands[i];
                if (seekingAddExecutable && command.TrimStart().StartsWith("add_executable"))
                {
                    seekingAddExecutable = false;
                }

                if (!seekingAddExecutable)
                {
                    if (command.Contains(sourceName))
                    {
                        break;
                    }

                    var lastClosingBracket = command.LastIndexOf(')');
                    if (lastClosingBracket > 0)
                    {
                        var newLine = command.Substring(0, lastClosingBracket);
                        commands[i] = $"{newLine} {Enquote(sourceName)})";
                        break;
                    }
                }
            }
        }

        public static CMakeList GetDefaultCMakeListForC(string[] sources)
        {
            var content = "cmake_minimum_required (VERSION 3.12)" + Environment.NewLine;
            content += "project (submission C)" + Environment.NewLine;
            content += "set(CMAKE_C_STANDARD 11)" + Environment.NewLine;

            content += Environment.NewLine + "# Build release version and optimizes for performance" + Environment.NewLine;
            content += "if (NOT CMAKE_BUILD_TYPE)" + Environment.NewLine;
            content += "set(CMAKE_BUILD_TYPE Release)" + Environment.NewLine;
            content += "endif()" + Environment.NewLine;
            content += "set(CMAKE_C_FLAGS \"-Wall -Wextra\")" + Environment.NewLine;
            content += "set(CMAKE_C_FLAGS_RELEASE \"-O3\")" + Environment.NewLine;

            content += Environment.NewLine + "# Use Latin1 encoding as default for source file and execution" + Environment.NewLine;
            content += "set(CMAKE_C_FLAGS \"${CMAKE_C_FLAGS} -finput-charset=Latin1 -fexec-charset=Latin1\")" + Environment.NewLine;

            content += Environment.NewLine + "# Configures exe to build from source" + Environment.NewLine;
            content += $"add_executable(submission {string.Join(" ", sources.Select(Enquote))}){Environment.NewLine}";

            content += Environment.NewLine + "# Adds use of math library" + Environment.NewLine;
            content += "target_link_libraries(submission m)" + Environment.NewLine;
            return new CMakeList(content);
        }

        public static CMakeList GetDefaultCMakeListForCpp(string[] sources)
        {
            var content = "cmake_minimum_required (VERSION 3.12)" + Environment.NewLine;
            content += "project (submission)" + Environment.NewLine;
            content += "set(CXX_STANDARD 17)" + Environment.NewLine;
            //content += "add_compile_options(-std=c++17)" + Environment.NewLine;

            content += Environment.NewLine + "# Build release version and optimizes for performance" + Environment.NewLine;
            content += "if (NOT CMAKE_BUILD_TYPE)" + Environment.NewLine;
            content += "set(CMAKE_BUILD_TYPE Release)" + Environment.NewLine;
            content += "endif()" + Environment.NewLine;
            content += "set(CMAKE_CXX_FLAGS \"-Wall -Wextra\")" + Environment.NewLine;
            content += "set(CMAKE_CXX_FLAGS_RELEASE \"-O3\")" + Environment.NewLine;

            content += Environment.NewLine + "# Use Latin1 encoding as default for source file and execution" + Environment.NewLine;
            content += "set(CMAKE_CXX_FLAGS_RELEASE \"${CMAKE_C_FLAGS} -finput-charset=Latin1 -fexec-charset=Latin1\")" + Environment.NewLine;

            content += Environment.NewLine + "# Configures exe to build from source" + Environment.NewLine;
            content += $"add_executable(submission {string.Join(" ", sources.Select(Enquote))}){Environment.NewLine}";

            content += Environment.NewLine + "# Adds use of math library" + Environment.NewLine;
            content += "target_link_libraries(submission m)" + Environment.NewLine;
            return new CMakeList(content);
        }

        private static string Enquote(string s)
        {
            if (s.All(x => x != ' '))
            {
                return s;
            }

            var result = new StringBuilder();
            var lastChr = '\0';
            foreach (var chr in s)
            {
                if (chr == ' ' && lastChr != '\\')
                {
                    result.Append('\\');
                }

                result.Append(chr);
                lastChr = chr;
            }

            return result.ToString();
        }

        public string GetEncoding()
        {
            var match = Regex.Match(GetContent(), "-fexec-charset\\s*=\\s*(?<encoding>\\w+)");
            if (match.Success)
            {
                if (match.Groups["encoding"].Value == "Latin1")
                {
                    return "1252";
                }
            }

            return "UTF8";
        }

        private List<string> ConvertToActions(string text)
        {
            text = FixNewlines(text);
            return new List<string>(text.Split(new[] {Environment.NewLine}, StringSplitOptions.None).Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim()));
        }

        private string FixNewlines(string text)
        {
            return text.Replace("\r", "\n").Replace("\n\n", "\n").Replace("\n", Environment.NewLine);
        }
    }
}
