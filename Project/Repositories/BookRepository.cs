using Project.Models;
using Project.ViewModels;

namespace Project.Repositories
{

    public interface IBookRepository       //the interface (written here instead of creating another file)
    {
        public BookDetailsVM GetBookDetails(int id);
    }


    public class BookRepository:IBookRepository
    {
        BookStoreContext db;
        public BookRepository()
        {
            db = new BookStoreContext();
        }

        public BookDetailsVM GetBookDetails(int id)
        {
            Book book = db.Books.FirstOrDefault(x => x.ID == id);
            List<Comment> comments = db.Comments.Where(x => x.book_id == book.ID).ToList();

            List<CommentVM> comments_vm = new List<CommentVM>();

            CommentVM cmVM;
            foreach (var comment in comments)
            {
                cmVM = new CommentVM()
                {
                    Comment = comment.comment,
                    Date = comment.Date,
                    //userFName = comment.user.FirstName,
                    //userLName = comment.user.LastName
                };
                comments_vm.Add(cmVM);
            }


            BookDetailsVM bookvm = new BookDetailsVM()
            {
                ID = book.ID,
                Name = book.Name,
                Description = book.Description,
                Price = book.Price,
                Rate = book.Rate,
                Image = book.Image,
                Quantity = book.Quantity,
                Author = db.Authors.FirstOrDefault(x => x.ID == book.Author_id),
                Category = db.Categories.FirstOrDefault(x => x.ID == book.Category_id),
                Discount = db.Discounts.FirstOrDefault(x => x.ID == book.Discount_id),
                authorBooks = db.Books.Where(x => x.Author_id == book.Author_id).ToList(),
                categoryBooks = db.Books.Where(x => x.Category_id == book.Category_id).ToList(),
                Comments = comments_vm
            };
            return bookvm;
        }
    }
}
