using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ShoppingCart.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ValidateNever]
        public Product Product { get; set; }
        [ValidateNever]
        public string ApplicationUserId { get; set; }
        [ValidateNever]
        public string AppUser { get; set; }
        public int Count { get; set; }
    }
}
