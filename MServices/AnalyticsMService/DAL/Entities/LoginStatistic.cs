using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class LoginStatistic : UserAction
    {
        [Column("SourceTypeId")]
        public int SourceTypeId { get; init; }

        [Column("StatusTypeId")]
        public int StatusTypeId { get; init; }
    }
}