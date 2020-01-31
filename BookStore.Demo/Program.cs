using System;
using System.IO;
using System.Linq;

using Vita.Data;
using Vita.Data.MsSql;
using Vita.Entities;

namespace BookStore.Demo {
  class Program  {
    static string ConnString = @"Data Source=.\MSSQL2017;Initial Catalog=VitaGuideApp;Integrated Security=True";
    const string LogFileName = "_bookStore.log"; // find it in bin/debug folder
    static BooksEntityApp _app; 

    static void Main(string[] args) {
      try {
        Console.WriteLine("Initializing...");
        Init();
        Console.WriteLine("Done. Connected sucessfully to the database, database tables created.");
        DeleteAll(); 
        Console.WriteLine("Creating data...");

        CreateSampleBook();
        RunDemo_StepByStep_Linq();
        RunDemo_LinqQuide();
        RunDemo_Linq_LinqUpdate(); 

      } catch(Exception ex) {
        Console.WriteLine("Error!!!");
        Console.WriteLine(ex.ToString());
      } finally {
        Console.WriteLine(); 
        Console.WriteLine("Demo completed, press any key...");
        Console.ReadKey();
      }
    }

    private static void CreateSampleBook() {
      var session = _app.OpenSession();
      // publisher
      var pub = session.NewPublisher("MS Publishing");
      // book
      var bk = pub.NewBook("c# programming", BookCategory.Programming, 350, 19.9m);
      // author
      var john = session.NewAuthor("John", "Sharp");
      var jack = session.NewAuthor("Jack", "Hacker");
      bk.Authors.Add(john);
      bk.Authors.Add(jack);
      session.SaveChanges();
      //check book counts
      var bkCount = session.EntitySet<IBook>().Count();
      Console.WriteLine($"Done. Create publisher and a book. Book count in db: {bkCount} ");

      // once the book is inserted, we have its Id value (it is Identity column);
      //   the value is returned from Db and put into entity
      var bkId = bk.Id;
      Console.WriteLine($" Id of created book: {bkId}");

      // Let's create a new session and retrieve the book
      session = _app.OpenSession();
      bk = session.GetEntity<IBook>(bkId);
      var pubName = bk.Publisher.Name; //Referenced entity is loaded lazily
      var authors = bk.Authors;
      Console.WriteLine($" Loaded book by Id, title: '{bk.Title}', published by '{pubName}', {authors.Count} authors.");

      // Reading pub.Books list property
      var pubBooks = bk.Publisher.Books;
      Console.WriteLine($" Publisher {pubName} book count: {pubBooks.Count}");
      var cmd = session.GetLastCommand();
      var sql = cmd.CommandText; 
    }

    // Step-by-step guide, LINQ section
    static void RunDemo_StepByStep_Linq() {
      var session = _app.OpenSession();

      // find inexpensive progr books
      var cheapBooks = session.EntitySet<IBook>()
                         .Where(b => b.Price < 20 && b.Category == BookCategory.Programming)
                         .OrderBy(b => b.Price)
                         .ToList();
      // Query using entity refs and lists; find books by "john"
      var booksByJohn = session.EntitySet<IBook>()
              .Where(b => b.Authors.Any(a => a.FirstName == "John"))
              .Select(b => new { b.Title, Publisher = b.Publisher.Name})             
              .ToList();
      // Retrieve SQL that was executed
      var cmd = session.GetLastCommand();
      var sql = cmd.CommandText;
    }

    static void RunDemo_LinqQuide() {
      var session = _app.OpenSession();
      // Query using entity refs and lists; find books by "john"
      var booksByJohn = session.EntitySet<IBook>()
              .Where(b => b.Authors.Any(a => a.FirstName == "John"))
              .OrderBy(b => b.Price)
              .Select(b => new { b.Title, Publisher = b.Publisher.Name })
              .Skip(0).Take(5) //paging
              .ToList();
      // Retrieve SQL that was executed
      var cmd = session.GetLastCommand();
      var sql = cmd.CommandText;

      // do the same using SQL-like syntax: 
      var booksByJohn2 = (
               from b in session.EntitySet<IBook>()
               where b.Authors.Any(a => a.FirstName == "John")
               orderby b.Price
               select new { b.Title, Publisher = b.Publisher.Name }
               )
               .Skip(0).Take(5)
               .ToList();
      cmd = session.GetLastCommand();

      var cheapBooks = session.EntitySet<IBook>()
          .Where(b => b.Price < 10)
          .ToList();

      var cheapBooksWithPubs = session.EntitySet<IBook>()
          .Where(b => b.Price < 10)
          .Select(b => new { Book = b, Pub = b.Publisher})
          .ToList();

      // group-by and aggregates
      var bksByCat = session.EntitySet<IBook>()
           .GroupBy(b => b.Category)
           .Select(g => new { Category = g.Key, BookCount = g.Count(), 
                        MaxPrice = g.Max(b => b.Price) })
           .OrderBy(rec => rec.Category) 
           .ToList();
      // aggregate over entire set
      var progrBooksCount = session.EntitySet<IBook>()
            .Where(b => b.Category == BookCategory.Programming)
            .Count();

      // fake group-by to get several aggregates
      var progrBooksStats = session.EntitySet<IBook>()
            .Where(b => b.Category == BookCategory.Programming)
            .GroupBy(b => 0)
            .Select(g => new { BookCount = g.Count(), MaxPrice = g.Max(b => b.Price) })
            .ToList();
    }

    static void RunDemo_Linq_LinqUpdate() {
    }

    static void Init() {
      // clear log
      if(File.Exists(LogFileName))
        File.Delete(LogFileName);
      // create and connect the app
      _app = new BooksEntityApp();
      _app.LogPath = LogFileName;
      _app.ErrorLogPath = "_errors.log";
      var driver = new MsSqlDbDriver();
      var dbSettings = new DbSettings(driver, MsSqlDbDriver.DefaultMsSqlDbOptions, ConnString);
      _app.ConnectTo(dbSettings);
    }

    static void DeleteAll() {
      var session = _app.OpenSession();
      DeleteAll<IBookAuthor>(session);
      DeleteAll<IAuthor>(session);
      DeleteAll<IBook>(session);
      DeleteAll<IPublisher>(session);
    }

    // Deletes all rows in database table. This method is intentionally not implemented in VITA
    //   -- too dangerous
    private static void DeleteAll<TEntity>(IEntitySession session) where TEntity : class {
      session.ExecuteDelete<TEntity>(session.EntitySet<TEntity>());
    }

  }
}
