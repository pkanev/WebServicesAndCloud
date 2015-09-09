namespace OnlineShop.Services.Models.BindingModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CreateAdBindingModel
    {
        [Required]
        //[MinLength(3)]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }
        
        [Required]
        //[MaxLength(1000)]
        [StringLength(1000)]
        public string Description { get; set; }
        
        [Required]
        public int TypeId { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        public IEnumerable<int> Categories { get; set; }
    }
}