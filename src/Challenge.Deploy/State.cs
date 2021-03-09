namespace Challenge.Deploy
{
    public class State
    {
        public string Name { get; set; }
        public int Port { get; set; }
        public bool Maintenance { get; set; }
        public bool WebPagesGenerated { get; set; }
        public string Path { get; set; }
        public string Version { get; set; }
        public int ChallengesCount { get; set; }
        public int SubmissionsCount { get; set; }
        public int MembersCount { get; set; }
    }
}
