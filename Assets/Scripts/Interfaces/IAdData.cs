namespace Interfaces
{
    public interface IAdData
    {
        public int AdFrequency { get; }
        public string BannerId { get; }
        public string VideoId { get; }
        public string RewardedVideoId { get; }
        public string SkipLevelRewardIdentity { get; }
    }
}