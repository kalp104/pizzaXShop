using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.ModelView
{
    public class EmailViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email is not valid")]
        public string? ToEmail { get; set; }
    }
}

