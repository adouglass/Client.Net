namespace Subscrio.Client.Models
{
    public class CompanyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string WebhookUrl { get; set; }
        public long CacheTimeoutTicks { get; set; }
    }
}
