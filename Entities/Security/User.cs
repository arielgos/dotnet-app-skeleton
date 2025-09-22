using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Entities.Security
{
    [Serializable]
    [Table("users", Schema = "security")]
    public class User : Base
    {
        [Column("id"), Required,Key]
        [StringLength(50)]
        public string Id { get; set; }

        [Column("name"), Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Column("login"), Required]
        [StringLength(50)]
        public string Login { get; set; }

        [Column("password"), Required]
        [StringLength(50)]
        public string Password { get; set; }

    }
}