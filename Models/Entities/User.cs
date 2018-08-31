using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Entities
{
    public class User
    {
        [Key]
        [BsonIgnore]
        public string id { get; set; }

        [Required(ErrorMessage = "First Name cannot be empty")]
        [DisplayName("First Name*")]
        [BsonElement("first_name")]
        public string first_name { get; set; }

        [Required(ErrorMessage = "Last Name cannot be empty")]
        [DisplayName("Last Name*")]
        [BsonElement("last_name")]
        public string last_name { get; set; }

        [Required(ErrorMessage = "Birth Date cannot be empty")]
        [DisplayName("Birth Date*")]
        [BsonElement("birth_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime birth_date { get; set; }

        [BsonElement("created_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime created_date { get; set; }

        [BsonElement("last_login")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime last_login { get; set; }

        [BsonElement("profile_img")]
        [DisplayName("Profile Picture")]
        public string profile_img { get; set; }

        [BsonElement("profile_background")]
        public string profile_background { get; set; }

        [Required]
        [BsonElement("gender")]
        [DisplayName("Gender*")]
        public int gender { get; set; }

        [BsonElement("phone_number")]
        [DisplayName("Phone Number")]
        public string phone_number { get; set; }

        [BsonElement("email")]
        [DisplayName("Email*")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Required(ErrorMessage = "Email is required")]
        public string email { get; set; }

        //[Required(ErrorMessage = "Password is required")]
        //[StringLength(50, ErrorMessage = "Must be between 5 and 50 characters", MinimumLength = 5)]
        //[DisplayName("Password*")]
        //[DataType(DataType.Password)]
        //[BsonIgnore]
        //public string password { get; set; }

        //[Required(ErrorMessage = "Confirm Password is required")]
        //[StringLength(50, ErrorMessage = "Must be between 5 and 50 characters", MinimumLength = 5)]
        //[DisplayName("Confirm Password*")]
        //[DataType(DataType.Password)]
        //[Compare("password")]
        //[BsonIgnore]
        //public string reenter_password { get; set; }
    }
}
