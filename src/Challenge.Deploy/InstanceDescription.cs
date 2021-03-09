using System.Diagnostics;

namespace Challenge.Deploy
{
    public class InstanceDescription
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int Port { get; set; }
        public Process Process { get; set; }
    }
}
