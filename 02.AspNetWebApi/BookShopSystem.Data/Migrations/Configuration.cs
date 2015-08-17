using System.Collections.Generic;
using System.Globalization;
using System.IO;
using BookShopSystem.Models;
using BookShopSystem.Models.DbModels;

namespace BookShopSystem.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BookShopSystem.Data.BookShopContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            this.ContextKey = "BookShopSystem.Data.BooksShopContext";
        }

        protected override void Seed(BookShopSystem.Data.BookShopContext context)
        {
            if (!context.Authors.Any())
            {
                var random = new Random();
                List<Category> categories = new List<Category>();
                using (var reader = new StreamReader("../../../data/categories.txt"))
                {
                    var line = reader.ReadLine();
                    while (line != null)
                    {
                        var category = line;
                        var cat = new Category() { Name = category };
                        context.Categories.Add(cat);
                        categories.Add(cat);

                        line = reader.ReadLine();
                    }
                }

                List<Author> authors = new List<Author>();
                using (var reader = new StreamReader("../../../data/authors.txt"))
                {
                    var line = reader.ReadLine();
                    line = reader.ReadLine();
                    while (line != null)
                    {
                        var data = line.Split(' ');
                        var firstName = data[0];
                        var lastname = data[1];
                        var author = new Author() { FirstName = firstName, LastName = lastname };
                        authors.Add(author);
                        context.Authors.Add(author);

                        line = reader.ReadLine();
                    }
                }

                using (var reader = new StreamReader("../../../data/books.txt"))
                {
                    var line = reader.ReadLine();
                    line = reader.ReadLine();
                    while (line != null)
                    {
                        var data = line.Split(new[] { ' ' }, 6);
                        var authorIndex = random.Next(0, authors.Count);
                        var author = authors[authorIndex];
                        var edition = (Edition)int.Parse(data[0]);
                        var releaseDate = DateTime.ParseExact(data[1], "d/M/yyyy", CultureInfo.InvariantCulture);
                        var copies = int.Parse(data[2]);
                        var price = decimal.Parse(data[3]);
                        var ageRestriction = (AgeRestriction)int.Parse(data[4]);
                        var title = data[5];
                        var book = new Book()
                        {
                            Author = author,
                            Edition = edition,
                            ReleaseDate = releaseDate,
                            Copies = copies,
                            Price = price,
                            AgeRestriction = ageRestriction,
                            Title = title,
                        };

                        var numOfCat = random.Next(1, 3);
                        for (int i = 0; i < numOfCat; i++)
                        {
                            List<Category> categoriesToBeAdded = new List<Category>();
                            Category c = categories[random.Next(0, categories.Count)];
                            if (!categoriesToBeAdded.Contains(c))
                            {
                                categoriesToBeAdded.Add(c);
                            }
                            foreach (var category in categoriesToBeAdded)
                            {
                                book.Categories.Add(category);
                            }
                        }
                        context.Books.Add(book);

                        line = reader.ReadLine();
                    }
                }
                
            }
        }
    }
}
