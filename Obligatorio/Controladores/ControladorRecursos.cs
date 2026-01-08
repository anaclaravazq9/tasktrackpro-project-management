using DTOs;
using IServicios.IGestores;

namespace Controladores;

public class ControladorRecursos
{
    private IGestorRecursos _gestorRecursos;

    public ControladorRecursos(IGestorRecursos gestorRecursos)
    {
        _gestorRecursos = gestorRecursos;
    }

    public void AgregarRecurso(UsuarioDTO solicitanteDTO, RecursoDTO nuevoRecursoDTO, bool esExclusivo)
    {
        _gestorRecursos.AgregarRecurso(solicitanteDTO, nuevoRecursoDTO, esExclusivo);
    }

    public void EliminarRecurso(UsuarioDTO solicitanteDTO, int idRecursoAEliminar)
    {
        _gestorRecursos.EliminarRecurso(solicitanteDTO, idRecursoAEliminar);
    }

    public RecursoDTO ObtenerRecursoPorId(int idRecurso)
    {
        return _gestorRecursos.ObtenerRecursoPorId(idRecurso);
    }

    public List<RecursoDTO> ObtenerRecursosGenerales()
    {
        return _gestorRecursos.ObtenerRecursosGenerales();
    }

    public List<RecursoDTO> ObtenerRecursosExclusivos(int idProyecto)
    {
        return _gestorRecursos.ObtenerRecursosExclusivos(idProyecto);
    }

    public void ModificarNombreRecurso(UsuarioDTO solicitanteDTO, int idRecurso, string nuevoNombre)
    {
        _gestorRecursos.ModificarNombreRecurso(solicitanteDTO, idRecurso, nuevoNombre);
    }

    public void ModificarDescripcionRecurso(UsuarioDTO solicitanteDTO, int idRecurso, string nuevaDescripcion)
    {
        _gestorRecursos.ModificarDescripcionRecurso(solicitanteDTO, idRecurso, nuevaDescripcion);
    }

    public void ModificarTipoRecurso(UsuarioDTO solicitanteDTO, int idRecurso, string nuevoTipo)
    {
        _gestorRecursos.ModificarTipoRecurso(solicitanteDTO, idRecurso, nuevoTipo);
    }
    
    public void ModificarCapacidadRecurso(UsuarioDTO solicitanteDTO, int idRecurso, int nuevaCapacidad)
    {
        _gestorRecursos.ModificarCapacidadRecurso(solicitanteDTO, idRecurso, nuevaCapacidad);
    }
    
    public List<RecursoPanelDTO> ObtenerPanelRecursos(int idProyecto)
    {
        return _gestorRecursos.ObtenerRecursosParaPanel(idProyecto);
    }


    public RecursoDTO ObtenerRecursoExclusivoPorId(int idProyecto, int idRecurso)
    {
        return _gestorRecursos.ObtenerRecursoExclusivoPorId(idProyecto, idRecurso);
    }
}