namespace FuseCs.Matching
{
    public class MatcherOptions
    {
        public double Threshold { get; set; } = 0.6d;
        public int Distance { get; set; } = 100;
        public int MaxLength { get; set; } = 32;
        public bool IgnoreCase { get; set; } = true;
    }
}