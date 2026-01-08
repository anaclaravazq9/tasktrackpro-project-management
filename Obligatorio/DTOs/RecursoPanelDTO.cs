using Dominio;

namespace DTOs;

public class RecursoPanelDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Tipo { get; set; }
    public string Descripcion { get; set; }
    public int Capacidad { get; set; }
    public List<RangoDeUsoDTO> RangosEnUso { get; set; } = new();
    public int NivelDeUso { get; set; }
    
    public static RecursoPanelDTO DesdeEntidad(Recurso recurso, List<RangoDeUso> rangosEnUso, int nivelDeUso)
    {
        return new RecursoPanelDTO
        {
            Id = recurso.Id,
            Nombre = recurso.Nombre,
            Tipo = recurso.Tipo,
            Descripcion = recurso.Descripcion,
            Capacidad = recurso.Capacidad,
            NivelDeUso = nivelDeUso,
            RangosEnUso = rangosEnUso.Select(RangoDeUsoDTO.DesdeEntidad).ToList()
        };
    }
}