using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class Book
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        public decimal? Rate { get; set; }

        public string? Image { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("Admin")]
        public int Admin_id { get; set; }


        [ForeignKey("Author")]
        public int Author_id { get; set; }

        [ForeignKey("Category")]
        public int Category_id { get; set; }


        [ForeignKey("Discount")]
        public int Discount_id { get; set; }
        public ApplicationUser Admin { get; set; }

        public Author Author { get; set; }

        public Category Category { get; set; }
        public Discount Discount { get; set; }
        public virtual List<Comment> Comments { set; get; } = new List<Comment>();
        public virtual List<OrderDetails> OrderDetails { set; get; } = new List<OrderDetails>();

    }
}
