namespace MiningService.Infrastructure.Model
{
    public class MiningDbConfigModel
    {
        public const string Key = "MiningDb";

        public string TablePrefix { get; set; }
        public string ServiceUrl { get; set; }
    }
}
