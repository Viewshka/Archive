namespace Archive.Application.Common.Options.MongoDb
{
    public class MongoDbOptions
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public Collections Collections { get; set; }
    }
}