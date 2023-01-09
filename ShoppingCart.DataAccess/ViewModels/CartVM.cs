using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ShoppingCart.Models;

namespace ShoppingCart.DataAccess.ViewModels
{
    public class CartVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ValidateNever]
        public Product Product { get; set; }
        [ValidateNever]
        public string ApplicationUserId { get; set; }
        [ValidateNever]
        public ApplicationUser AppUser { get; set; }
        public int Count { get; set; }
        public IEnumerable<Cart> ListOfCart { get; set; }
        public OrderHeader ItemOrderHeader { get; set; }
    }
}
