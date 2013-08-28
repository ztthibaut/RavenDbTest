using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Document;
using Raven.Client.Linq;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            var documentStore = new DocumentStore
                {
                   Url="http://localhost:8080/databases/test"
                };
            documentStore.Initialize();
            var bookName = "Book";
            using (var session = documentStore.OpenSession())
            {
                session.Store(new Book
                    {
                        Name = bookName,
                        Posts = new List<BookPost> {new BookPost()
                                    {
                                        Title = "A post",
                                        Type = BookPost.BookPostType.BooPost1
                                    }
                                }
                    });

                session.Store(new Book
                    {
                        Name = bookName,
                        Posts = new List<BookPost> {new BookPost()
                                    {
                                        Title = "A post",
                                        Type = BookPost.BookPostType.BooPost2
                                    }
                                }
                    });
                session.SaveChanges();
            }

             using (var session = documentStore.OpenSession())
            {
                var ravenQueryable = session.Query<Book>().Customize(b=>b.WaitForNonStaleResultsAsOfLastWrite()).ToList();
            }
            
            using (var session = documentStore.OpenSession())
            {
                var bookToGet = new List<string>() {bookName};
                var bookPostToGet = new List<BookPost.BookPostType?> {BookPost.BookPostType.BooPost1};
                var books = session.Query<Book>().Where(b => b.Name.In(bookToGet));
                books = books.Where(b => b.Posts.Any(p => p.Type.In(bookPostToGet)));

                var bookPage = books.ToList();
            
            }


        }
    }

    public class Book
    {
        public String Name { get; set; }

        public List<BookPost> Posts { get; set; }
    }

    public class BookPost
    {
        public string Title { get; set; }

        public BookPostType? Type { get; set; }

        public enum BookPostType
        {
            BooPost1,
            BooPost2,
            BooPost3
        }
    }

}
