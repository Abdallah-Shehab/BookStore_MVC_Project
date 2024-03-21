using Project.ViewModels;

namespace Project.Repositories
{
    public interface IBookRepository
    {
        public BookDetailsVM GetBookDetails(int id);
    }
}