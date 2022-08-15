using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechlunchApi.Models
{
    public class Inventory
    {
        public int Id { get; set; }


        [Required]
        public int IngredientId { get; set; }

        [ForeignKey("IngredientId")]
        public Ingredient IngredientFK { get; set; }


        [Required]
        public int Quantity { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public DateTime AddedOn { get; set; }

    }
}
