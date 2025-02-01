using System;
using System.ComponentModel.DataAnnotations;

namespace PaymentAPI.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; }

        public string Status { get; set; } = "Pending"; // Status inicial é "Pending"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedAt { get; set; }
    }
}