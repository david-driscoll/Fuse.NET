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
                Distance = 1000,
                MaxLength = 400,
                IgnoreCase = true,
                Tokenize = true
            });

            // When_searching_for_the_term_Woodhouse
            var search = fuse.Search(books, "Jeeves Woodhouse");
            Assert.Equal(6, search.Count());
        }
    }
}
