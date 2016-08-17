using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FuseCs.Tests
{
    public class BookTests
    {
        private class Book
        {
            public string title { get; set; }
            public string author { get; set; }
        }

        private class User
        {
            public string name { get; set; }
            public string CML_rank { get; set; }
            public string registrationNumber { get; set; }
            public string alloted { get; set; }
        }

        [Fact]
        public void List_of_books_searching_title_and_author__When_searching_for_the_term_HTML5()
        {
            var books = Newtonsoft.Json.JsonConvert.DeserializeObject<Book[]>(File.ReadAllText("books.json"));
            var fuse = Fuse.Create(new FuseOptions<Book>()
            {
                Keys = new[]
                {
                    FuseKey.Create<Book>("title", x => new[] {x.title}),
                    FuseKey.Create<Book>("author", x => new[] {x.author})
                },
                IgnoreCase = true,
                Tokenize = true
            });

            var search = fuse.Search(books, "HTML5");
            Assert.Equal(3, search.Count());
        }

        [Fact]
        public void List_of_books_searching_title_and_author__When_searching_for_the_term_Woodhouse()
        {
            var books = Newtonsoft.Json.JsonConvert.DeserializeObject<Book[]>(File.ReadAllText("books.json"));
            var fuse = Fuse.Create(new FuseOptions<Book>()
            {
                Keys = new[]
                {
                    FuseKey.Create<Book>("title", x => x.title),
                    FuseKey.Create<Book>("author", x => x.author)
                },
                IgnoreCase = true,
                Tokenize = true
            });

            // When_searching_for_the_term_Woodhouse
            var search = fuse.Search(books, "Jeeves Woodhouse");
            Assert.Equal(6, search.Count());
        }

        [Fact]
        public void Weighted_search__When_searching_for_the_term_Man_where_the_author_is_weighted_higher_than_title()
        {
            var books = new[]
            {
                new Book()
                {
                    title = "bcd",
                    author = "bcd"
                },
                new Book()
                {
                    title = "Old Man's War fiction",
                    author = "John X"
                },
                new Book()
                {
                    title = "Right Ho Jeeves",
                    author = "P.D. Mans"
                },
            };
            var fuse = Fuse.Create(new FuseOptions<Book>()
            {
                Keys = new[]
                {
                    FuseKey.Create<Book>("title", x => x.title, 0.3),
                    FuseKey.Create<Book>("author", x => x.author, 0.7)
                },
                IgnoreCase = true
            });

            // When_searching_for_the_term_Woodhouse
            var search = fuse.Search(books, "Man");
            Assert.Equal("Right Ho Jeeves", search.First().title);
        }

        [Fact]
        public void Weighted_search__When_searching_for_the_term_Man_where_the_title_is_weighted_higher_than_author()
        {
            var books = new[]
            {
                new Book()
                {
                    title = "bcd",
                    author = "bcd"
                },
                new Book()
                {
                    title = "Old Man's War fiction",
                    author = "John X"
                },
                new Book()
                {
                    title = "Right Ho Jeeves",
                    author = "P.D. Mans"
                },
            };
            var fuse = Fuse.Create(new FuseOptions<Book>()
            {
                Keys = new[]
                {
                    FuseKey.Create<Book>("title", x => x.title, 0.7),
                    FuseKey.Create<Book>("author", x => x.author, 0.3)
                },
                IgnoreCase = true
            });

            // When_searching_for_the_term_Woodhouse
            var search = fuse.Search(books, "Man");
            Assert.Equal("John X", search.First().author);
        }

        [Fact]
        public void Search_location__When_searching_for_the_term_wor()
        {
            var books = new[]
            {
                new Book()
                {
                    title = "Hello World"
                },
            };
            var fuse = Fuse.Create(new FuseOptions<Book>()
            {
                Keys = new[]
                {
                    FuseKey.Create<Book>("title", x => x.title)
                },
                IgnoreCase = true
            });

            // When_searching_for_the_term_Woodhouse
            var search = fuse.Search(books, "wor");
            var match = search.Results.First().Matches[nameof(Book.title)];
            var a = match.First();
            var b = match.Last();
            Assert.Equal(4, a.Start);
            Assert.Equal(4, a.End);
            Assert.Equal(6, b.Start);
            Assert.Equal(8, b.End);
        }
    }
}
