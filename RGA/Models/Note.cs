using System;
using System.ComponentModel.DataAnnotations;

namespace RGA.Models
{
    public class Note
    {
        [Key]
        public string Id { get; set; }

        public virtual User Creator { get; set; }
        public virtual User Driver { get; set; }
        public string Content { get; set; }

        public DateTime DateAdded { get; set; }
    }
}