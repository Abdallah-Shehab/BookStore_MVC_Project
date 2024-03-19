using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class Comment
    {


        public string comment { get; set; }

        public DateTime Date { get; set; }

        [ForeignKey("user")]
        public int user_id { get; set; }

        [ForeignKey("book")]
        public int book_id { get; set; }
        public ApplicationUser user { get; set; }
        public Book book { get; set; }


    }
}
