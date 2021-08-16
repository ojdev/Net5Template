namespace CoreTemplate.Infrastructure.ES
{
    public class ESConnectionStringConfiguration
    {
        public ElasticSearchConfigurationNode ElasticSearch { get; set; }

    }
    public class ElasticSearchConfigurationNode
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
