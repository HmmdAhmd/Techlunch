using System;
using System.ComponentModel.DataAnnotations;

namespace TechlunchApi.Models
{
    public class Order
    {
        public Order()
        {
            Status = true;
        }
        public int Id { get; set; }

        [Required]
        public float TotalPrice { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public Boolean Status { get; set; }
    }
}
