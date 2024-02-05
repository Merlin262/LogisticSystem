
﻿using logisticsSystem.Models;

﻿using System.Text.Json.Serialization;


namespace logisticsSystem.DTOs;

public class PhoneDTO
{
    [JsonIgnore]
    public int Id { get; set; }
    public string AreaCode { get; set; }
    public string Number { get; set; }
    public int FkPersonId { get; set; }

    
}
//public PersonDTO FkPerson { get; set; }