namespace Crytex.GameServers.Models
{
    public class GameHostParam
    {
        public string UserId { get; set; }
        public string ServerId { get; set; }
        public string GameId { get; set; }
        public int GamePort { get; set; }
        public string GamePassword { get; set; }
        public int Slots { get; set; }
        public int MinCpu { get; set; }
        public string Url { get; set; }
    }
}
