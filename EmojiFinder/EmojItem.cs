namespace EmojiFinder
{
    public class EmojItem
    {
        public string Emoji { get; set; }
        public string Keyword => string.Join(" / ", Keywords);
        public string[] Keywords { get; set; }
    }
}
