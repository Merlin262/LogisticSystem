﻿using System.Text.Json.Serialization;


namespace logisticsSystem.DTOs;


public class AddressDTO
{
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? Number { get; set; }
    public string? Complement { get; set; }
    public string? Zipcode { get; set; }
    [JsonIgnore]
    public int Id { get; set; }

}
