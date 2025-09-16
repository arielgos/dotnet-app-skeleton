using System.ComponentModel.DataAnnotations;

namespace Model
{
    [Serializable]
    public class Login
    {
        [Display(Name = "Nombre de usuario"), Required(AllowEmptyStrings = false, ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, ErrorMessage = "El dato no debe exceder los {1} caracteres")]
        public string login { get; set; }

        [Display(Name = "Contraseña"), Required(AllowEmptyStrings = false, ErrorMessage = "La contraseña es requerida")]
        [StringLength(50, ErrorMessage = "El dato no debe exceder los {1} caracteres")]
        public string password { get; set; }
    }
}