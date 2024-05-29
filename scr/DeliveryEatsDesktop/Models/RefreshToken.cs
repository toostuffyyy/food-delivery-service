using System;
using System.ComponentModel.DataAnnotations;

namespace desktop.Models
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; }
    }
}
