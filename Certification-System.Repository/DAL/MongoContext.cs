using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Certification_System.Repository.Context
{
    public class MongoContext
    {
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
            using (StreamReader sr = new StreamReader("../Certification-System.Repository/DAL/dbCredentials.json"))
            {
                var json = sr.ReadToEnd();
                try
                {
                    dynamic deserializedObject = JsonConvert.DeserializeObject(json);
                    _database = deserializedObject.DatabaseName;
                    _connectionstring = deserializedObject.ConnectionString;
                }
                catch (Exception e)
                {
                }
            }
        }

        private void InitializeContextInstantion()
        {
            _mongoClient = new MongoClient(_connectionstring);
            db = _mongoClient.GetDatabase(_database);
        }
    }
}