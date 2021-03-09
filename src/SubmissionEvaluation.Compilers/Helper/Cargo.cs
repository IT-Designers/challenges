using Nett;

namespace SubmissionEvaluation.Compilers.Helper
{
    internal class Cargo
    {
        private string value;

        public Cargo(string value)
        {
            this.value = value;
        }

        public void SetPackageName(string packageName)
        {
            var parsed = Toml.ReadString(value);
            parsed.Get<TomlTable>("package").Update("name", packageName);
            value = parsed.ToString();
        }

        public string GetText()
        {
            return value;
        }

        public static Cargo GetDefaultCargo()
        {
            var defaultCargo = @"
[package]
name = ""default_submission""
version = ""0.1.0""
authors = [""Maximilian Schall <Maximilian.Schall@mail.de>""]
edition = ""2018""

[dependencies]
";
            return new Cargo(defaultCargo);
        }
    }
}
