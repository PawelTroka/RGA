using System.ComponentModel.DataAnnotations;

namespace RGA.Models
{
    public class Shipment
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Numer przesyłki")]
        [DataType(DataType.Text)]
        public string Number { get; set; }

        [Required]
        [Display(Name = "Adres")]
        [DataType(DataType.Text)]
        public string DestinationAddress { get; set; }
    }
}