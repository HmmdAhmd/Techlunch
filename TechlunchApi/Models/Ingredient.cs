using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

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
        [StringLength(100)]
        public string Name { get; set; }

        public Boolean Status { get; set; }

        
    }
}
