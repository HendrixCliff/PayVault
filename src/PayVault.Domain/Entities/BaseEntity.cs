using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayVault.Domain.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; protected set; } = Guid.NewGuid();
        
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        
        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedAt { get; protected set; }
        
        [Column(TypeName = "datetime2")]
        public DateTime? DeletedAt { get; protected set; }
    }
}