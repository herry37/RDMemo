using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Models
{
    public class ShoppingListCreateRequest
    {
        [Required(ErrorMessage = "購買日期不能為空")]
        public DateTime BuyDate { get; set; }

        [Required(ErrorMessage = "標題不能為空")]
        [StringLength(100, ErrorMessage = "標題長度不能超過 100 個字元")]
        public string Title { get; set; } = string.Empty;
    }
}
