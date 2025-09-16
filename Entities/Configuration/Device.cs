using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Configuration
{
    [Serializable]
    [Table("devices", Schema = "configuration")]
    public class Device : Base
    {
        [Column("id"), Required]
        [StringLength(50)]
        public string Id { get; set; }

        [Column("name"), Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Column("is_active"), Required]
        public bool IsActive { get; set; }

    }
}