using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Excepciones;
using Excepciones.MensajesError;

namespace Utilidades;

public static class UtilidadesContrasena
{
    private static readonly int _largoMinimoContrasena = 8;

    private static readonly int
        _largoMaximoContrasena = 15; //Se define para no autogenerar una contraseña demasiado larga

    public static string ValidarYEncriptarContrasena(string contrasena)
    {
        ValidarFormatoContrasena(contrasena);
        return BCrypt.Net.BCrypt.HashPassword(contrasena);
    }

    public static string AutogenerarContrasenaValida()
    {
        string minusculas = "abcdefghijklmnñopqrstuvwxyz";
        string mayusculas = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";
        string numeros = "1234567890";
        string simbolos = "!@#$%^&*()-_=+<>?{}[].,:;¡¿?'/~|°";
        string todosLosCaracteres = minusculas + mayusculas + numeros + simbolos;

        StringBuilder contrasenaAutogenerada = new StringBuilder();

        RandomNumberGenerator
            generadorDeNumerosAleatorio =
                RandomNumberGenerator.Create(); // generador de números aleatorios criptográficamente seguros

        int largo = GenerarNumeroAleatorio(_largoMinimoContrasena, _largoMaximoContrasena, generadorDeNumerosAleatorio);
        // agregar manualmente una mayúscula, una minúscula, un número y un caracter especial (para asegurar restricciones de contraseña)
        contrasenaAutogenerada.Append(GenerarCaracterAleatorio(minusculas, generadorDeNumerosAleatorio));
        contrasenaAutogenerada.Append(GenerarCaracterAleatorio(mayusculas, generadorDeNumerosAleatorio));
        contrasenaAutogenerada.Append(GenerarCaracterAleatorio(numeros, generadorDeNumerosAleatorio));
        contrasenaAutogenerada.Append(GenerarCaracterAleatorio(simbolos, generadorDeNumerosAleatorio));

        while (contrasenaAutogenerada.Length < largo){
            contrasenaAutogenerada.Append(GenerarCaracterAleatorio(todosLosCaracteres,
                generadorDeNumerosAleatorio));
        }
        return MezclarCaracteres(contrasenaAutogenerada.ToString(), generadorDeNumerosAleatorio);
    }

    private static void ValidarFormatoContrasena(string contrasena)
    {
        ValidarLargoContrasena(contrasena);
        ValidarAlgunaMayuscula(contrasena);
        ValidarAlgunaMinuscula(contrasena);
        ValidarAlgunNumero(contrasena);
        ValidarAlgunCaracterEspecial(contrasena);
    }

    private static void ValidarLargoContrasena(string contrasena)
    {
        if (contrasena.Length < _largoMinimoContrasena)
        {
            throw new ExcepcionContrasena(MensajesErrorServicios.ContrasenaMuyCorta(_largoMinimoContrasena));
        }
    }

    private static void ValidarAlgunaMayuscula(string contrasena)
    {
        if (!contrasena.Any(char.IsUpper))
        {
            throw new ExcepcionContrasena(MensajesErrorServicios.ContrasenaSinMayuscula);
        }
    }

    private static void ValidarAlgunaMinuscula(string contrasena)
    {
        if (!contrasena.Any(char.IsLower))
        {
            throw new ExcepcionContrasena(MensajesErrorServicios.ContrasenaSinMinuscula);
        }
    }

    private static void ValidarAlgunNumero(string contrasena)
    {
        if (!contrasena.Any(char.IsDigit))
        {
            throw new ExcepcionContrasena(MensajesErrorServicios.ContrasenaSinNumero);
        }
    }

    private static void ValidarAlgunCaracterEspecial(string contrasena)
    {
        if (!Regex.IsMatch(contrasena,
                "[^a-zA-Z0-9]")) // RegEx para que haya algún caracter distinto a minúsuclas, mayúsuclas o números
        {
            throw new ExcepcionContrasena(
                MensajesErrorServicios.ContrasenaSinCaracterEspecial);
        }
    }

    private static int GenerarNumeroAleatorio(int min, int max, RandomNumberGenerator generadorDeNumerosAleatorio)
    {
        byte[]
            buffer = new byte[sizeof(uint)]; 
        generadorDeNumerosAleatorio.GetBytes(buffer); // llena el buffer con números aleatorios
        uint numero = BitConverter.ToUInt32(buffer, 0); // se convierte el buffer en número de tipo uint
        // asegurar que el número esté en el rango
        int rango = max - min + 1;
        return (int)(numero % rango) + min;
    }

    private static char GenerarCaracterAleatorio(string caracteres, RandomNumberGenerator generadorDeNumerosAleatorio)
    {
        int indice = GenerarNumeroAleatorio(0, caracteres.Length - 1, generadorDeNumerosAleatorio);
        return caracteres[indice];
    }

    private static string MezclarCaracteres(string input, RandomNumberGenerator generadorDeNumerosAleatorio)
    {
        char[]
            array = input
                .ToCharArray();
        for (int i = array.Length - 1; i > 0; i--)
        {
            // shuffle
            int j = GenerarNumeroAleatorio(0, i, generadorDeNumerosAleatorio);
            (array[i], array[j]) = (array[j], array[i]);
        }

        return new string(array);
    }
}