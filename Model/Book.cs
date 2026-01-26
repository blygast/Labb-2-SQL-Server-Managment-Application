using System;
using System.Collections.Generic;

namespace BokhandelApp.Model;

public partial class Book
{
    public string Isbn { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Language { get; set; } = null!;

    public decimal Price { get; set; }

    public DateOnly PublishDate { get; set; }

    public int? Pages { get; set; }

    public string? Format { get; set; }

    public int? PublisherId { get; set; }

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    public virtual ICollection<BookBalance> BookBalances { get; set; } = new List<BookBalance>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Publisher? Publisher { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
