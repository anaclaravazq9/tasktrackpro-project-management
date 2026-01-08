using System.ComponentModel.DataAnnotations;

namespace DTOs;

public class ModeloCambioContraseña
{
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
        ErrorMessage =
            "La nueva contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial.")]
    public string NuevaContrasena { get; set; } = string.Empty;
}