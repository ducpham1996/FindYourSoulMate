using FindYourSoulMate.Models.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models
{
    public class Comment
    {
        [BsonId]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("by")]
        [BsonIgnoreIfNull]
        public Owner owner { get; set; }

        [BsonElement("message")]
        [BsonIgnoreIfNull]
        public string message { get; set; }

        [BsonElement("dateCreated")]
        [BsonIgnoreIfNull]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime date_created { get; set; }

        [BsonElement("like")]
        public int like { get; set; }

        [BsonElement("love")]
        public int love { get; set; }

        [BsonElement("angry")]
        public int angry { get; set; }

        [BsonElement("scare")]
        public int scare { get; set; }

        [BsonElement("haha")]
        public int haha { get; set; }

        [BsonElement("sad")]
        public int sad { get; set; }

        [BsonElement("amaze")]
        public int amaze { get; set; }

        [BsonElement("suprise")]
        public int suprise { get; set; }

        [BsonElement("comments")]
        [BsonIgnoreIfNull]
        public int comments { get; set; }

        [BsonElement("sub_comments")]
        [BsonIgnoreIfNull]
        public List<Comment> sub_comments { get; set; }

        [BsonIgnore]
        public string has_like { get; set; }

        [BsonIgnore]
        public bool is_own { get; set; }


    }
}
