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
        public IMongoDatabase getConnection()
        {
            MongoClient dbClient = new MongoClient("mongodb://findsoulmate:!duc12345@den1.mongo1.gear.host:27001/findsoulmate");
            IMongoDatabase db = dbClient.GetDatabase("findsoulmate");
            return db;
        }

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
            var posts = db.GetCollection<Post>("Post");
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse("5b00ce7afcb00a38b09e2365"));
            
            //var update = Builders<BsonDocument>.Update.Set("title", "C#");

            //update comment
            //var update = Builders<BsonDocument>.Update.AddToSet("comments", new Comment() { user = "me", message = "hi there", date_created = new DateTime(2018, 5, 20, 0, 0, 0, DateTimeKind.Utc), like = 0.0 });
            //await collection.UpdateOneAsync(filter, update);

            //BsonDocument document = new BsonDocument();
            //var skills = new List<string> { "C++", "Java", "C#" };
            //document.Add("skills", new BsonArray(skills));

            //var aPost = posts.AsQueryable<Post>().ToArray().Where(p => p._id.Equals(ObjectId.Parse("5b00ce7afcb00a38b09e2365"))).ToArray();
            
            //await posts.InsertOneAsync(new Post { title = "test", description = "testing", by = "me", url = "google.com" });
        }
    }
}
