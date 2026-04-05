using System;
using System.Collections.Generic;

namespace ProductApp.Models;

public partial class Productmaster
{
    public string Productid { get; set; } = null!;

    public string Productname { get; set; } = null!;

    public string Producttypeid { get; set; } = null!;

    public int Productstatus { get; set; }

    public string Createuser { get; set; } = null!;

    public DateTime Createdate { get; set; }

    public string? Updateuser { get; set; }

    public DateTime? Updatedate { get; set; }
}
