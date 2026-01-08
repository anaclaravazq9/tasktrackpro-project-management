using Controladores;
using DTOs;
using Excepciones;
using Excepciones.MensajesError;
using Microsoft.AspNetCore.Mvc;

namespace Interfaz.Components.ControladoresInterfaz;

[Route("exportar")]
public class ControladorExportacionWeb : Controller
{
    private readonly ControladorExportacion _controlador;

    public ControladorExportacionWeb(ControladorExportacion controlador)
    {
        _controlador = controlador;
    }

    [HttpGet("{formato}")]
    public async Task<IActionResult> Exportar(string formato)
    {
        try
        {
            ArchivoExportadoDTO archivo = await _controlador.Exportar(formato);
            return File(archivo.Contenido, archivo.TipoContenido, archivo.NombreArchivo);
        }
        catch (ExcepcionExportador e)
        {
            return BadRequest(new { error = e.Message });
        }
    }
}
