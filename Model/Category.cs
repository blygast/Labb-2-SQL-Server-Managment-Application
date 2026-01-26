using System;
using System.Collections.Generic;

namespace BokhandelApp.Model;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Book> Isbns { get; set; } = new List<Book>();
}
