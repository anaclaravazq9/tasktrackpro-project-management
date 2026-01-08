using DTOs;

namespace IServicios.IGestores;

public interface IGestorRecursos
{
    void AgregarRecurso(UsuarioDTO solicitanteDTO, RecursoDTO recursoDTO, bool esExclusivo);
    
    void EliminarRecurso(UsuarioDTO solicitanteDTO, int idRecurso);
    
    RecursoDTO ObtenerRecursoPorId(int idRecurso);
    
    List<RecursoDTO> ObtenerRecursosGenerales();
    
    List<RecursoDTO> ObtenerRecursosExclusivos(int idProyecto);

    void ModificarNombreRecurso(UsuarioDTO solicitanteDTO, int idRecurso, string nuevoNombre);
    
    void ModificarTipoRecurso(UsuarioDTO solicitanteDTO, int idRecurso, string nuevoTipo);
    
    void ModificarDescripcionRecurso(UsuarioDTO solicitanteDTO, int idRecurso, string nuevaDescripcion);

    void ModificarCapacidadRecurso(UsuarioDTO solicitanteDTO, int idRecurso, int nuevaCapacidad);
    
    RecursoDTO ObtenerRecursoExclusivoPorId(int idProyecto, int idRecurso);
    
    List<RecursoPanelDTO> ObtenerRecursosParaPanel(int idProyecto);

}