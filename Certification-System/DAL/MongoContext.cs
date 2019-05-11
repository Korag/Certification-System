using MongoDB.Driver;
using System.Configuration;

namespace Certification_System.DAL
{
    public class MongoContext
    {
        private string _user;
        private string _password;
        private string _database;
        private string _connectionstring;

        private MongoClient _mongoClient;

        public IMongoDatabase db;

        public MongoContext()
        {
            InitializeDatabaseCredentials();
            InitializeContextInstantion();
        }

        private void InitializeDatabaseCredentials()
        {
            _user = "Admin";
            _password = "certification_admin1";
            _database = "certification_system";
            _connectionstring = $"mongodb://{_user}:{_password}@ds026898.mlab.com:26898/{_database}";
        }

        private void InitializeContextInstantion()
        {
            _mongoClient = new MongoClient(_connectionstring);
            db = _mongoClient.GetDatabase(_database);
        }
    }
}