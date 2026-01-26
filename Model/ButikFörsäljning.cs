using System;
using System.Collections.Generic;

namespace BokhandelApp.Model;

public partial class ButikFörsäljning
{
    public int StoreId { get; set; }

    public string Butik { get; set; } = null!;

    public int? AntalOrdrar { get; set; }

    public int? UnikaKunder { get; set; }

    public decimal? Omsättning { get; set; }

    public decimal? SnittOrder { get; set; }
}
