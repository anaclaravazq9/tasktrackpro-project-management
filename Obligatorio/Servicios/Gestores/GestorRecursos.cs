using Dominio;
using DTOs;
using Excepciones;
using Excepciones.MensajesError;
using IRepositorios;
using IServicios;
using IServicios.IGestores;
using Servicios.Notificaciones;
using Utilidades;

namespace Servicios.Gestores;

public class GestorRecursos : IGestorRecursos
{
    private readonly IRepositorio<Recurso> _repositorioRecursos;
    private readonly IGestorProyectos _gestorProyectos;
    private readonly IRepositorioUsuarios _repositorioUsuarios;
    private readonly INotificador _notificador;

    public GestorRecursos(
        IRepositorio<Recurso> repositorioRecursos,
        IGestorProyectos gestorProyectos,
        IRepositorioUsuarios repositorioUsuarios, INotificador notificador)
    {
        _repositorioRecursos = repositorioRecursos;
        _gestorProyectos = gestorProyectos;
        _repositorioUsuarios = repositorioUsuarios;
        _notificador = notificador;
    }

    public void AgregarRecurso(UsuarioDTO solicitanteDTO, RecursoDTO recursoDTO, bool esExclusivo)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = recursoDTO.AEntidad();
        
        PermisosUsuarios.VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "agregar recursos");
        
        if (solicitante.EstaAdministrandoUnProyecto && esExclusivo)
        {
            AsociarRecursoAProyectoQueAdministra(solicitante, recurso);
        }

        _repositorioRecursos.Agregar(recurso);
        recursoDTO.Id = recurso.Id;
    }

    public void EliminarRecurso(UsuarioDTO solicitanteDTO, int idRecurso)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
        
        PermisosUsuarios.VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "eliminar un recurso");
        VerificarRecursoEnUso(recurso);
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "eliminar");
        
        _repositorioRecursos.Eliminar(recurso.Id);
        NotificarEliminacion(recurso);
    }

    public RecursoDTO ObtenerRecursoPorId(int idRecurso)
    {
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
        return RecursoDTO.DesdeEntidad(recurso);
    }

    public List<RecursoDTO> ObtenerRecursosGenerales()
    {
        List<Recurso> recursosGenerales =
            _repositorioRecursos.ObtenerTodos().Where(recurso => !recurso.EsExclusivo()).ToList();
        return recursosGenerales.Select(RecursoDTO.DesdeEntidad).ToList();
    }

    public List<RecursoDTO> ObtenerRecursosExclusivos(int idProyecto)
    {
        List<Recurso> recursosExclusivos =
            _repositorioRecursos.ObtenerTodos().Where(recurso => recurso.EsExclusivo() && recurso.ProyectoAsociado.Id == idProyecto).ToList();
        return recursosExclusivos.Select(RecursoDTO.DesdeEntidad).ToList();
    }

    public void ModificarNombreRecurso(UsuarioDTO solicitanteDTO, int idRecurso, string nuevoNombre)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
        
        PermisosUsuarios.VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar el nombre de un recurso");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar el nombre de");
        
        string nombreAnterior = recurso.Nombre;
        recurso.ModificarNombre(nuevoNombre);
        _repositorioRecursos.Actualizar(recurso);
        
        NotificarModificacion(recurso, nombreAnterior);
    }
    
    public void ModificarCapacidadRecurso(UsuarioDTO solicitanteDTO, int idRecurso, int nuevaCapacidad)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
    
        PermisosUsuarios.VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar la capacidad de un recurso");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar la capacidad de");

        recurso.ModificarCapacidad(nuevaCapacidad);
        NotificarModificacion(recurso, recurso.Nombre);
    }

    public void ModificarTipoRecurso(UsuarioDTO solicitanteDTO, int idRecurso, string nuevoTipo)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
        
        PermisosUsuarios.VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar el tipo de un recurso");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar el tipo de");
        
        recurso.ModificarTipo(nuevoTipo);
        _repositorioRecursos.Actualizar(recurso);
        
        NotificarModificacion(recurso, recurso.Nombre);
    }

    public void ModificarDescripcionRecurso(UsuarioDTO solicitanteDTO, int idRecurso, string nuevaDescripcion)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
        
        PermisosUsuarios.VerificarPermisoAdminSistemaOAdminProyecto(solicitante,
            "modificar la descripción de un recurso");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar la descripción de");
        
        recurso.ModificarDescripcion(nuevaDescripcion);
        
        _repositorioRecursos.Actualizar(recurso);
        
        NotificarModificacion(recurso, recurso.Nombre);
    }

    public RecursoDTO ObtenerRecursoExclusivoPorId(int idProyecto, int idRecurso)
    {
        List<RecursoDTO> recursosExclusivos = ObtenerRecursosExclusivos(idProyecto);
        RecursoDTO recurso = recursosExclusivos.FirstOrDefault(recurso => recurso.Id == idRecurso);
        
        if (recurso == null)
        {
            throw new ExcepcionRecurso(MensajesErrorServicios.RecursoNoEncontrado);
        }

        return recurso;
    }
    
    public List<RecursoPanelDTO> ObtenerRecursosParaPanel(int idProyecto)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        
        List<RecursoDTO> recursosDTOs = ObtenerRecursosExclusivos(proyecto.Id).Concat(ObtenerRecursosGenerales()).ToList();
        List<Recurso> recursos = recursosDTOs.Select(r => ObtenerRecursoDominioPorId(r.Id)).ToList();
        
        List<Tarea> tareasEnProceso = proyecto.Tareas.Where(t => t.Estado == EstadoTarea.EnProceso).ToList();

        List<RecursoPanelDTO> panel = new();
        foreach (Recurso recurso in recursos)
        {
            int nivelDeUso = tareasEnProceso.Count(t => t.UsaRecurso(recurso.Id));
            List<RangoDeUso> rangos = recurso.RangosEnUso.ToList();

            panel.Add(RecursoPanelDTO.DesdeEntidad(recurso, rangos, nivelDeUso));
        }
        return panel;
    }

    private Recurso ObtenerRecursoDominioPorId(int idRecurso)
    {
        Recurso recurso = _repositorioRecursos.ObtenerPorId(idRecurso);
        
        if (recurso == null)
        {
            throw new ExcepcionRecurso(MensajesErrorServicios.RecursoNoEncontrado);
        }

        return recurso;
    }

    private void AsociarRecursoAProyectoQueAdministra(Usuario administradorProyecto, Recurso recurso)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDelAdministrador(administradorProyecto.Id);
        recurso.AsociarAProyecto(proyecto);
        _repositorioRecursos.Actualizar(recurso);
    }

    private void VerificarRecursoEnUso(Recurso recurso)
    {
        if (recurso.SeEstaUsando())
        {
            throw new ExcepcionRecurso(MensajesErrorServicios.RecursoEnUso);
        }
    }

    private void VerificarRecursoExclusivoDelAdministradorProyecto(Usuario administradorProyecto, Recurso recurso,
        string accion)
    {
        if (administradorProyecto.EsAdministradorSistema)
        {
            return;
        }

        if (recurso.ProyectoAsociado == null)
        {
            throw new ExcepcionPermisos(MensajesErrorServicios.PermisoDenegadoPara($"{accion} recursos generales."));
        }

        Proyecto proyectoQueAdministra = _gestorProyectos.ObtenerProyectoDelAdministrador(administradorProyecto.Id);
        
        if (!recurso.EsExclusivo() || !recurso.ProyectoAsociado.Equals(proyectoQueAdministra))
        {
            throw new ExcepcionPermisos(
                MensajesErrorServicios.PermisoDenegadoPara($"{accion} recursos que no son exclusivos de su proyecto"));
        }
    }

    private void NotificarEliminacion(Recurso recurso)
    {
        if (recurso.EsExclusivo())
        {
            _notificador.NotificarUno(recurso.ProyectoAsociado.Administrador,
                MensajesNotificacion.RecursoEliminado(recurso.Nombre, recurso.Tipo, recurso.Descripcion));
        }
        else
        {
            // decidimos que si el recurso no es exclusivo, se notifica a todos los admins de todos los proyectos.
            _gestorProyectos.NotificarAdministradoresDeProyectos(_gestorProyectos.ObtenerTodosDominio(),
                MensajesNotificacion.RecursoEliminado(recurso.Nombre, recurso.Tipo, recurso.Descripcion));
        }
    }

    private void NotificarModificacion(Recurso recurso, string nombreAnterior)
    {
        string mensaje = MensajesNotificacion.RecursoModificado(nombreAnterior, recurso.ToString());
        if (recurso.EsExclusivo())
        {
            _notificador.NotificarUno(recurso.ProyectoAsociado.Administrador, mensaje);
        }
        else
        {
            NotificarAdministradoresDeProyectosQueUsanRecurso(recurso, mensaje);
        }
    }

    private void NotificarAdministradoresDeProyectosQueUsanRecurso(Recurso recurso, string mensaje)
    {
        List<Proyecto> proyectosQueUsanElRecurso = _gestorProyectos.ObtenerTodosDominio()
            .Where(proyecto => RecursosNecesariosPorProyecto(proyecto).Contains(recurso)).ToList();
        
        _gestorProyectos.NotificarAdministradoresDeProyectos(proyectosQueUsanElRecurso, mensaje);
    }

    private List<Recurso> RecursosNecesariosPorProyecto(Proyecto proyecto)
    {
        return proyecto.Tareas
            .SelectMany(tarea => tarea.RecursosNecesarios)
            .Select(rn => rn.Recurso)
            .Distinct()
            .ToList();;
    }

    private Usuario ObtenerUsuarioPorDTO(UsuarioDTO usuarioDTO)
    {
        Usuario usuario = _repositorioUsuarios.ObtenerPorId(usuarioDTO.Id);
        
        if (usuario == null)
        {
            throw new ExcepcionUsuario(MensajesErrorServicios.UsuarioNoEncontrado);
        }

        return usuario;
    }
}