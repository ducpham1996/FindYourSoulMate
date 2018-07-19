using FindYourSoulMate.Models.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models
{
    [BsonIgnoreExtraElements]
    public class Post
    {
        [Key]
        [BsonIgnore]
        public string id { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("title")]
        [Required(ErrorMessage = "Title is required")]
        [DisplayName("Title")]
        public string title { get; set; }

        [BsonElement("description")]
        [Required(ErrorMessage = "Description is required")]
        [DisplayName("Description")]
        public string description { get; set; }

        [BsonElement("by")]
        public Owner owner { get; set; }

        [BsonElement("dateCreated")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime date_created { get; set; }

        [BsonElement("tags")]
        [BsonIgnoreIfNull]
        public List<String> tags { get; set; }

        [BsonElement("video_image")]
        [BsonIgnoreIfNull]
        public List<VideoImage> video_image { get; set; }

        [BsonElement("has_modified")]
        public bool has_modified { get; set; }

        [BsonElement("feeling")]
        public string feeling { get; set; }

        [BsonElement("modify_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime modify_date { get; set; }

        [BsonElement("location")]
        public string location { get; set; }

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
        public int comments { get; set; }

        [BsonElement("sub_comments")]
        public int sub_comments { get; set; }

        [BsonElement("status")]
        public int status { get; set; }
    }


}
