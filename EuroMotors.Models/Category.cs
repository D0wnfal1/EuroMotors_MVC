using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EuroMotors.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Ви не ввели Назву Категорії!")]
        [DisplayName("Назва Категорії")]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Порядок відображення обов'язковий!")]
        [DisplayName("Порядок Відображення")]
        [Range(1, 100, ErrorMessage = "Значення повинно бути від 1 до 100!")]
        public int? DisplayOrder { get; set; }
    }
}
