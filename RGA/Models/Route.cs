using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;
using GoogleMapsApi.Entities.Directions.Response;
using RGA.Helpers;

namespace RGA.Models
{
    public class Route
    {
        [Key]
        public string Id { get; set; }
        [Display(Name = "Obraz")]
        public byte[] Image { get; set; } //jpeg
        [Display(Name = "Opis")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Podsumowanie")]
        public string Summary { get; set; }

        [Display(Name = "Notatki")]
        public virtual ICollection<Note> Notes { get; set; }

        //  public virtual List<Leg> Sections { get; set; }// public virtual ICollection

        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        public DateTime StartDateTime { get; set; }

        [Display(Name = "Kierowca")]
        public virtual User Driver { get; set; } //driver who drives that route

        [Display(Name = "Pracownik")]
        public virtual User Worker { get; set; } //worker who created that route

        [Display(Name = "Adres początkowy")]
        public string StartAddress { get; set; }

        [Display(Name = "Przesyłki")]
        public virtual ICollection<Shipment> Shipments { get; set; }
        [Display(Name = "Adres końcowy")]
        public string EndAddress { get; set; }

        [Display(Name = "Czas trwania")]
        [DataType(DataType.Time)]
        public TimeSpan Duaration { get; set; }
        [Display(Name = "Dystans")]
        public long Distance { get; set; }

        [Display(Name = "Stan")]
        public RouteState State { get; set; }

        [Display(Name = "Algorytm optymalizacji")]
        public RouteOptimizationAlgorithm RouteOptimizationAlgorithm { get; set; }

        [Display(Name = "Dostawca optymalizacji")]
        public RouteOptimizationProvider RouteOptimizationProvider { get; set; }

        [Display(Name = "Kryterium optymalizacji")]
        public RouteOptimizationType RouteOptimizationType { get; set; }
    }
}