using System;
using System.Collections.Generic;
using System.Text;
using Vita.Entities;

namespace BookStore {

  public class BooksEntityModule: EntityModule {
    public BooksEntityModule(EntityArea area): base(area, "BooksModule") {
      this.RegisterEntities(typeof(IPublisher), typeof(IBook), 
                            typeof(IAuthor), typeof(IBookAuthor));
    }
  }
}
