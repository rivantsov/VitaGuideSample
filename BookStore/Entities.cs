using System;
using System.Collections.Generic;
using Vita.Entities;

namespace BookStore {

  [Entity]
  public interface IPublisher {

    [PrimaryKey, Identity]
    int Id { get; }

    [Size(100)]
    string Name { get; set; }

    [OrderBy("Title:desc")]
    IList<IBook> Books { get; }
  }

  [Entity]
  public interface IBook {

    [PrimaryKey, Identity]
    int Id { get; }

    [Size(100)]
    string Title { get; set; }

    BookCategory Category { get; set; }

    [Nullable, Unlimited]
    string Description { get; set; }

    int PageCount { get; set; }

    decimal Price { get; set; }

    IPublisher Publisher { get; set; }
  }

  public enum BookCategory {
    Programming,
    Fiction,
    Kids,
  }

}
