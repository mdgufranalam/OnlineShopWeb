using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Category
    {
        [Key]
        [Display(Name ="Category Id")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Category name cannot be blank.")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        public string Description { get; set; }
        [Display(Name ="Created Date")]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
