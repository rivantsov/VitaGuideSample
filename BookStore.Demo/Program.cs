using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vita.Data;
using Vita.Data.MsSql;
using Vita.Entities;

namespace BookStore.Demo
{
  class Program
  {
    static BooksEntityApp _app; 

    static void Main(string[] args)
    {
      try {
        Console.WriteLine("Initializing...");
        Init();
        Console.WriteLine("Done. Connected sucessfully to the database, database tables created.");
        DeleteAll(); 
        Console.WriteLine("Creating data...");

        var session = _app.OpenSession();
        var pub = session.NewEntity<IPublisher>();
        pub.Name = "MS Publishing";
        var bk = session.NewEntity<IBook>();
        bk.Title = "c# programming";
        bk.Category = BookCategory.Programming;
        bk.PageCount = 350;
        bk.Price = 19.99m;
        bk.Publisher = pub;
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
        Console.WriteLine($" Loaded book by Id, title: '{bk.Title}', published by '{pubName}'");


      } catch(Exception ex) {
        Console.WriteLine("Error!!!");
        Console.WriteLine(ex.ToString());
      } finally {
        Console.WriteLine(); 
        Console.WriteLine("Demo completed, press any key...");
        Console.ReadKey();
      }
    }

    static void Init() {
      string connString = @"Data Source=.\MSSQL2017;Initial Catalog=VitaGuideApp;Integrated Security=True"; 
      _app = new BooksEntityApp();
      var driver = new MsSqlDbDriver();
      var dbSettings = new DbSettings(driver, MsSqlDbDriver.DefaultMsSqlDbOptions, connString);
      _app.ConnectTo(dbSettings);
    }

    static void DeleteAll() {
      var session = _app.OpenSession();
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
