using System.Text.Json.Serialization;

namespace ARABYTAK.APIS.DTOs
{
    public class AdvertismentDto
    {
        [JsonIgnore]
        public int id { get; set; }

        public string Description { get; set; }
        public DateTime StartCreateAdvertisement
        {
            set => value = DateTime.Now;
        }

        public string SellerEmail { get; set; }

        public string ContactInfo { get; set; }
        public decimal Price { get; set; }
        public DateTime ExpiryDate { get; set; }


        public string Status { get; set; }

        public string BrandName { get; set; }

        public string ModelName { get; set; }

        public string PlanType { get; set; }

        public int PhoneNumber  { get; set; }
        public string Facebook { get; set; }
        public int Whatsapp { get; set; }
        public string Instegram { get; set; }

        [JsonIgnore]
        public int AdPlanId { get; set; }

        [JsonIgnore]
        public int CarId { get; set; }



    }
}
