using Project.Models;

namespace Project.ViewModels
{
    public class CommentVM
    {
        public string Comment { get; set; }
        public decimal? rate { get; set; }
        public DateTime Date { get; set; }
        public string userFName { get; set; }
        public string userLName { get; set; }

    }
}
