using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Entities
{
    public class Post_Comment
    {
        [BsonId]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId post_id { get; set; }

        [BsonElement("comments")]
        public List<Comment> comments { get; set; }
    }
}
