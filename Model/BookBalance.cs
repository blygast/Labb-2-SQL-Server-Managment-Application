using System;
using System.Collections.Generic;

namespace BokhandelApp.Model;

public partial class BookBalance
{
    public int StoreId { get; set; }

    public string Isbn { get; set; } = null!;

    public int AmountInStock { get; set; }

    public virtual Book IsbnNavigation { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;
}
