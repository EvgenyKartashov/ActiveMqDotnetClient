namespace ActiveMqClient.Options
{
    public class Queue2Config
    {
        public static string ConfigSection = "Queue2Config";

        public string Host { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string OutgoingQueue { get; set; }
        public Credentials Credentials { get; set; }
    }
}
