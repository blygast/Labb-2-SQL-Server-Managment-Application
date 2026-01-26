using System;
using System.Collections.Generic;

namespace BokhandelApp.Model;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public int StoreId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Store Store { get; set; } = null!;
}
