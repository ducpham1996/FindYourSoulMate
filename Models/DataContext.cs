using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models
{
    public class DataContext
    {
        public async Task testConnectionAsync()
        {
            MongoClient dbClient = new MongoClient("mongodb://findsoulmate:!duc12345@den1.mongo1.gear.host:27001/findsoulmate");
            IMongoDatabase db = dbClient.GetDatabase("findsoulmate");
            var collection = db.GetCollection<BsonDocument>("Post");
            Debug.WriteLine("The list of collections are :");
            using (var cursor = await collection.Find(new BsonDocument()).ToCursorAsync())
            {
                while (await cursor.MoveNextAsync())
                {
                    foreach (var doc in cursor.Current)
                    {
                        Debug.WriteLine(doc);
                    }
                }
            }
        }
    }
}
