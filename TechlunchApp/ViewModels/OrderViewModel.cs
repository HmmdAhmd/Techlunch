using System;
using System.ComponentModel.DataAnnotations;

namespace TechlunchApp.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Required]
        public float TotalPrice { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public bool Confirmed { get; set; }
    }
}
