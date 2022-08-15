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
        public string Name { get; set; }

        [Required]
        public float Price { get; set; }

        public Boolean Status { get; set; }


    }
}
