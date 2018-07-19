using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models
{
    [BsonIgnoreExtraElements]
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        [BsonElement("title")]
        public string title { get; set; }
        [BsonElement("description")]
        public string description { get; set; }
        [BsonElement("by")]
        public string by { get; set; }
        [BsonElement("url")]
        public string url { get; set; }

        [BsonElement("tags")]
        [BsonIgnoreIfNull]
        public List<string> tags { get; set; }

        [BsonElement("likes")]
        [BsonIgnoreIfNull]
        public double likes { get; set; }

        [BsonElement("comments")]
        [BsonIgnoreIfNull]
        public List<Comment> comments { get; set; }
    }
}
