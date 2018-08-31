using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Entities
{
    public class Notification
    {
        [Key]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("is_read")]
        public bool is_read { get; set; }

        [BsonElement("sender")]
        public string sender_id { get; set; }

        [BsonIgnore]
        public string sender_pic { get; set; }
        [BsonIgnore]
        public string sender_name { get; set; }

        [BsonElement("activity")]
        public int activity { get; set; }

        [BsonElement("description")]
        public string description { get; set; }

        [BsonElement("url")]
        public string url { get; set; }

        [BsonElement("time_sent")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime time_sent { get; set; }

    }
}
