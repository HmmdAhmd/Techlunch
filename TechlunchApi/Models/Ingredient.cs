using System;
using System.ComponentModel.DataAnnotations;

namespace TechlunchApi.Models
{
    public class Ingredient
    {
        public Ingredient()
        {
            Status = true;
        }
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Boolean Status { get; set; }

        
    }
}
