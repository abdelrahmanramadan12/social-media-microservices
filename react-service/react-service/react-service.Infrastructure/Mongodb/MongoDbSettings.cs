namespace react_service.Infrastructure.Mongodb
{
    public class MongoDbSettings
    {
        public string? ConnectionString { get; set; }
        public string? PostsDatabaseName { get; set; }
        public string? ReactionsDatabaseName { get; set; }
    }
}
