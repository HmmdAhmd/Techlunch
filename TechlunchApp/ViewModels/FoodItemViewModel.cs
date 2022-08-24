using System.ComponentModel.DataAnnotations;

namespace TechlunchApp.ViewModels
{
    public class FoodItemViewModel
    {
      
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter price")]
        public float Price { get; set; }

        public int Quantity { get; set; }

        public int AvailableQuantity { get; set; }

    }
}
