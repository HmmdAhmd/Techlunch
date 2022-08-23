using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace TechlunchApp.ViewModels
{
    public class IngredientViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        [StringLength(100, ErrorMessage = "Name length can be atmost 100 characters")]
        public string Name { get; set; }
    }
}
