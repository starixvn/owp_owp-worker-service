namespace OWP.Adminsite.Models
{
    public class ProviderMatchIdTw
    {
        public bool? finish { get; set; }

        public bool? status { get; set; }

        public string message { get; set; }

        public IList<Content> content { get; set; }= new List<Content>();

    }

    public class Content
    {
        public int IbcMatchId { get; set; }

        public int ProviderMatchId { get; set; }

        public int ProviderId { get; set; }
    }
}
