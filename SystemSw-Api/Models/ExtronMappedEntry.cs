namespace SystemSw_Api.Models
{
    public sealed class ExtronMappedEntry
    {
        public int Channel { get; set; }
        public string ChannelName { get; set; }
        public bool IsActiveChannel { get; set; }
    }
}