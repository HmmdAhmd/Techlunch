using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechlunchApi.Models
{
    public class GeneralInventory
    {
        public int Id { get; set; }


        [Required]
        public int IngredientId { get; set; }

        [ForeignKey("IngredientId")]
        public Ingredient Ingredient { get; set; }


        [Required]
        public int AvailableQuantity { get; set; }

        [Required]
        public float AveragePrice { get; set; }

    }
}
