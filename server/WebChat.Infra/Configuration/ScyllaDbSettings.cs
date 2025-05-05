namespace WebChat.Infra.Configuration
{
    public class ScyllaDbSettings
    {
        public string[] ContactPoints { get; set; }
        public int Port { get; set; }
        public string DataCenter { get; set; }
        public string Keyspace { get; set; }
        public string MessagesTable { get; set; }
        public string RoomsTable { get; set; }
        public string UsersTable { get; set; }
    }
}
