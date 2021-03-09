using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Providers;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TestParameters parameters = new TestParameters
            {
                Hint = "Comment",
                ExpectedOutput =
                
                    new OutputDefinition
                    {
                        Content = "Hallo",
                        Settings = new CompareSettings
                        {
                            CompareMode = CompareModeType.Exact,
                            IgnoreCase = true,
                            IgnoreUmlauts = true,
                            Trim = TrimMode.End,
                            UnifyFloatingNumbers = true,
                            Whitespaces = WhitespacesMode.RemoveEvenNewlines
                        },
                        Alternatives = new List<string>
                        {
                            "Alternative1",
                            "Alternative2"
                        }
                    }
                ,
                ExpectetOutputFile = new OutputFileDefinition
                {
                    Content = "Hallo",
                    Name = "Foobar.txt",
                    Settings = new CompareSettings
                    {
                        CompareMode = CompareModeType.Exact,
                        IgnoreCase = true,
                        IgnoreUmlauts = true,
                        Trim = TrimMode.End,
                        UnifyFloatingNumbers = true,
                        Whitespaces = WhitespacesMode.RemoveEvenNewlines
                    }
                },
                Input = new InputDefinition {Content = "4711"},
                InputFile = new FileDefinition()
                {
                    Content = "abcd",
                    Name = "input.txt"
                },
                Parameters = new List<string> { "A", "B" },
                Timeout = 1234
            };

            var yamlProvider = new YamlProvider(null);
            yamlProvider.Serialize("test.yml", parameters);
            return;

        }
    }
}
