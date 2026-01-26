using System;
using System.Collections.Generic;

namespace BokhandelApp.Model;

public partial class BookAuthor
{
    public string Isbn { get; set; } = null!;

    public int AuthorId { get; set; }

    public string? Role { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Book IsbnNavigation { get; set; } = null!;
}
