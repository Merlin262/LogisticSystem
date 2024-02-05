using System.Text.Json.Serialization;

namespace logisticsSystem.DTOs
{
    public class ShippingDTO
    {
        public DateOnly? SendDate { get; set; }

        public DateOnly? EstimatedDate { get; set; }

        public DateOnly? DeliveryDate { get; set; }

        public decimal TotalWeight { get; set; }

        public decimal DistanceKm { get; set; }

        public DateOnly? RegistrationDate { get; set; }

        [JsonIgnore]
        public int Id { get; set; }

        public decimal ShippingPrice { get; set; }

        public int FkClientId { get; set; }

        public int FkEmployeeId { get; set; }

        public int FkAddressId { get; set; }

        public int FkTruckId { get; set; }

    }


}