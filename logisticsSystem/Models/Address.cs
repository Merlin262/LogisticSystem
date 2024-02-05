using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class Address
{
    public string Country { get; set; }

    public string State { get; set; }

    public string City { get; set; }

    public string Street { get; set; }

    public string Number { get; set; }

    public string Complement { get; set; }

    public string Zipcode { get; set; }

    public int Id { get; set; }

    public virtual ICollection<Person> People { get; set; } = new List<Person>();

    public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();
}
