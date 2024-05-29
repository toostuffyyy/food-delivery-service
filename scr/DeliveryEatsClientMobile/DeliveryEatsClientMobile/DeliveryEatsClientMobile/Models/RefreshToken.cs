using System.ComponentModel.DataAnnotations;

namespace DeliveryEatsClientMobile.Models
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; }
    }
}
