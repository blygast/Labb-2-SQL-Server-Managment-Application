using System;
using System.Collections.Generic;

namespace BokhandelApp.Model;

public partial class TitlarPerForfattare
{
    public string Namn { get; set; } = null!;

    public string Ålder { get; set; } = null!;

    public string Titlar { get; set; } = null!;

    public string Lagervärde { get; set; } = null!;
}
