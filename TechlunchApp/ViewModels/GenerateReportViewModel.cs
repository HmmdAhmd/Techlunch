using System;
using System.ComponentModel.DataAnnotations;

namespace TechlunchApp.ViewModels
{
    public class GenerateReportViewModel
    {
        [Required(ErrorMessage = "Please enter start date")]
        public DateTime? StartingTime { get; set; }

        [Required(ErrorMessage = "Please enter end date")]
        public DateTime? EndingTime { get; set; }
    }
}
