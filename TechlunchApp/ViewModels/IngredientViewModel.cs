using System.ComponentModel.DataAnnotations;
namespace TechlunchApp.ViewModels
{
    public class IngredientViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}