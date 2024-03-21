using Project.Models;
using Project.ViewModels;

namespace Project.Repositories
{


    public class BookRepository : IBookRepository
    {
        BookStoreContext db;
        public BookRepository()
        {
            db = new BookStoreContext();
        }

        public BookDetailsVM GetBookDetails(int id)
        {
            Book book = db.Books.FirstOrDefault(x => x.ID == id);
            var comments = db.Comments.Where(x => x.book_id == book.ID)
                .Select(b => new CommentVM { Comment = b.comment, Date = b.Date, rate=b.rate, userFName = b.user.FirstName, userLName = b.user.LastName }).ToList();

            BookDetailsVM bookvm = new BookDetailsVM()
            {
                ID = book.ID,
                Name = book.Name,
                Description = book.Description,
                Price = book.Price,
                Rate = book.Rate,
                Image = book.Image,
                Quantity = book.Quantity,
                categoryID = book.Category_id,
                commentsNum = comments.Count,
                Author = db.Authors.FirstOrDefault(x => x.ID == book.Author_id),
                Category = db.Categories.FirstOrDefault(x => x.ID == book.Category_id),
                Discount = db.Discounts.FirstOrDefault(x => x.ID == book.Discount_id),
                authorBooks = db.Books.Where(x => x.Author_id == book.Author_id && x.ID != book.ID).Select(x => new BookDetailsVM { ID = x.ID, Name = x.Name, Price = x.Price, Rate = x.Rate, Image = x.Image, Quantity = x.Quantity, Category = x.Category, commentsNum = db.Comments.Where(s => s.book_id == x.ID).Count() }).ToList(),
                categoryBooks = db.Books.Where(x => x.Category_id == book.Category_id && x.ID != book.ID).Select(x=>new BookDetailsVM { ID=x.ID , Name=x.Name, Price=x.Price,Rate=x.Rate,Image=x.Image,Quantity=x.Quantity,Author=x.Author,commentsNum=db.Comments.Where(s=>s.book_id==x.ID).Count() }).ToList(),
                Comments = comments
            };
            return bookvm;
        }
    }
}
