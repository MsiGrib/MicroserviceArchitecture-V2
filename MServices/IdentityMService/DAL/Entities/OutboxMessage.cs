using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("OutboxMessages")]
    public class OutboxMessage
    {
        [Key]
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column("EventType")]
        public string EventType { get; set; } = string.Empty;

        [Required]
        [Column("EventData")]
        public string EventData { get; set; } = string.Empty;

        [Required]
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("ProcessedAt")]
        public DateTime? ProcessedAt { get; set; } = null;

        [Column("Status")]
        public string Status { get; set; } = "Pending";

        [Column("RetryCount")]
        public int RetryCount { get; set; } = 0;

        [Column("ErrorMessage")]
        public string ErrorMessage { get; set; } = string.Empty;

        [Required]
        [Column("Topic")]
        public string Topic { get; set; } = string.Empty;

        [Column("CorrelationId")]
        public Guid? CorrelationId { get; set; } = null;
    }
}