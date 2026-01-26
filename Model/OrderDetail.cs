using System;
using System.Collections.Generic;

namespace BokhandelApp.Model;

public partial class OrderDetail
{
    public int OrderId { get; set; }

    public string Isbn { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal BookPrice { get; set; }

    public virtual Book IsbnNavigation { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
