using System;
using System.Collections.Generic;
using Vita.Entities;

namespace BookStore {

  [Entity]
  public interface IPublisher {

    [PrimaryKey, Identity]
    int Id { get; }

    [Size(100), Unique]
    string Name { get; set; }

    [OrderBy("Title:desc")]
    IList<IBook> Books { get; }
  }

  [Entity, ClusteredIndex("Category,Id")]
  public interface IBook {

    [PrimaryKey, Identity]
    int Id { get; }

    [Size(100)]
    string Title { get; set; }

    BookCategory Category { get; set; }

    [Nullable, Unlimited]
    string Description { get; set; }

    int PageCount { get; set; }

    [Index]
    decimal Price { get; set; }

    IPublisher Publisher { get; set; }

    [ManyToMany(typeof(IBookAuthor)), OrderBy("LastName,FirstName")]
    IList<IAuthor> Authors { get; }
  }

  public enum BookCategory {
    Programming,
    Fiction,
    Kids,
  }

  [Entity]
  public interface IAuthor {
    [PrimaryKey, Identity]
    int Id { get; set; }

    [Size(50)]
    string FirstName { get; set; }

    [Size(50)]
    string LastName { get; set; }

    [ManyToMany(typeof(IBookAuthor)), OrderBy("Title")]
    IList<IBook> Books { get; }
  }

  [Entity, PrimaryKey("Book,Author")] 
  public interface IBookAuthor {
    IBook Book { get; set; }
    IAuthor Author { get; set; }
  }

}
