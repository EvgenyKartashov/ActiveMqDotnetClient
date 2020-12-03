namespace ActiveMqClient.Options
{
    public class Queue1Config
    {
        public static string ConfigSection = "Queue1Config";

        public string Host { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string IncomingQueue { get; set; }
    }
}
