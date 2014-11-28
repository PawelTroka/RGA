using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RGA.Models
{
    public class Note
    {
        [Key]
        public string Id { get; set; }
        public virtual User Creator { get; set; }
        public virtual User Driver { get; set; }
        public string Content { get; set; }
    }
}