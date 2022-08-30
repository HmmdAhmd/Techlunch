using System;
using System.ComponentModel.DataAnnotations;

namespace TechlunchApi.Models
{
    public class FoodItem
    {
        public FoodItem()
        {
            Status = true;
        }
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(1, float.MaxValue)]
        public float Price { get; set; }

        public Boolean Status { get; set; }

    }
}
