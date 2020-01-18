using System;
using System.Collections.Generic;
using System.Text;
using Vita.Entities;

namespace BookStore {

  public class BooksEntityApp : EntityApp {
    BooksEntityModule _booksModule; 

    public BooksEntityApp() {
      var area = base.AddArea("books"); // area is equivalent of schema
      _booksModule = new BooksEntityModule(area); 
    }
  }
}
