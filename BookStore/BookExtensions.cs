using System;
using System.Collections.Generic;
using System.Text;
using Vita.Entities;

namespace BookStore {
  public static class BookExtensions {

    public static IPublisher NewPublisher(this IEntitySession session, string name) {
      var pub = session.NewEntity<IPublisher>();
      pub.Name = name;
      return pub; 
    }

    public static IBook NewBook(this IPublisher publisher, string title, BookCategory category, int pageCount, 
           decimal price, string description = null) {
      var session = EntityHelper.GetSession(publisher);
      var book = session.NewEntity<IBook>();
      book.Title = title;
      book.Category = category;
      book.PageCount = pageCount;
      book.Price = price;
      book.Description = description;
      book.Publisher = publisher; 
      return book; 
    }

    public static IAuthor NewAuthor(this IEntitySession session, string firstName, string lastName) {
      var author = session.NewEntity<IAuthor>();
      author.FirstName = firstName;
      author.LastName = lastName;
      return author; 
    }

  }
}
