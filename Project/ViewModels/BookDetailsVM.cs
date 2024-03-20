using Project.Models;

namespace Project.ViewModels
{
    public class BookDetailsVM
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal? Rate { get; set; }

        public string? Image { get; set; }
        public int Quantity { get; set; }
        public Author Author { get; set; }
        public List<Book> authorBooks {  get; set; }  //books from the same author
        public Category Category { get; set; }
        public List<Book> categoryBooks { get; set; }  //books from the same category
        public Discount Discount { get; set; }
        public List<Comment> Comments { set; get; }      //create commentVM !!!!!!!!!!!!!

    }
}
