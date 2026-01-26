using System;
using System.Collections.Generic;

namespace BokhandelApp.Model;

public partial class Store
{
    public int StoreId { get; set; }

    public string StoreName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual ICollection<BookBalance> BookBalances { get; set; } = new List<BookBalance>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
