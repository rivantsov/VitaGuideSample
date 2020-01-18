using System;
using Vita.Entities;

namespace BookStore {

  [Entity]
  public interface IPublisher {

    [PrimaryKey, Identity]
    int Id { get; }

    [Size(100)]
    string Name { get; set; }
  }

  [Entity]
  public interface IBook {

    [PrimaryKey, Identity]
    int Id { get; }

    [Size(100)]
    string Title { get; set; }

    BookCategory Category { get; set; }

    [Nullable, Size(200)]
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
