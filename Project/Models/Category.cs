using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Category
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public virtual List<Book> Books { set; get; } = new List<Book>();

    }
}
