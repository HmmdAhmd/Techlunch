using System.ComponentModel.DataAnnotations;

namespace TechlunchApp.ViewModels
{
    public class FoodItemViewModel
    {
      
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        [StringLength(100)]

        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter price")]
        [Range(1, float.MaxValue, ErrorMessage = "Price must be greater than or equal to 1")]
        public float? Price { get; set; }

    }
}
