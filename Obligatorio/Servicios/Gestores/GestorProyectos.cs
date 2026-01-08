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

public class GestorProyectos : IGestorProyectos
{
    private readonly IRepositorioProyectos _proyectos;
    private readonly IRepositorioUsuarios _repositorioUsuarios;
    private readonly INotificador _notificador;
    private readonly ICalculadorCaminoCritico _caminoCritico;
    
    public GestorProyectos(IRepositorioUsuarios repositorioUsuarios, IRepositorioProyectos repositorioProyectos,
        INotificador notificador, ICalculadorCaminoCritico caminoCritico)
    {
        _proyectos = repositorioProyectos;
        _repositorioUsuarios = repositorioUsuarios;
        _notificador = notificador;
        _caminoCritico = caminoCritico;
    }

    public void CrearProyecto(ProyectoDTO proyectoDTO, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        
        PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        PermisosUsuarios.VerificarUsuarioNoAdministraOtroProyecto(solicitante);

        VerificarNombreNoRepetido(proyectoDTO.Nombre);
        
        Proyecto proyecto = proyectoDTO.AEntidad(solicitante);

        _proyectos.Agregar(proyecto);

        solicitante.EstaAdministrandoUnProyecto = true;
        
        _repositorioUsuarios.Actualizar(solicitante);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(), MensajesNotificacion.ProyectoCreado(proyecto.Nombre));
        
        proyectoDTO.Id = proyecto.Id;
    }

    public void EliminarProyecto(int idProyecto, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);

        List<Usuario> miembros = proyecto.Miembros.ToList();
        
        BorrarDependencias(proyecto); // Permite borrar seguramente el proyecto de la base de datos
        
        _proyectos.Eliminar(idProyecto);

        solicitante.EstaAdministrandoUnProyecto = false;
        
        _repositorioUsuarios.Actualizar(solicitante);
        
        foreach (Usuario miembro in miembros)
        {
            miembro.CantidadProyectosAsignados--;
            _repositorioUsuarios.Actualizar(miembro);
        }

        _notificador.NotificarMuchos(miembros, MensajesNotificacion.ProyectoEliminado(proyecto.Nombre));
    }


    public List<ProyectoDTO> ObtenerTodos()
    {
        List<Proyecto> proyectos = _proyectos.ObtenerTodos();
        return proyectos.Select(ProyectoDTO.DesdeEntidad).ToList();
    }

    public List<Proyecto> ObtenerTodosDominio()
    {
        return _proyectos.ObtenerTodos();
    }

    public ProyectoDTO ObtenerProyectoPorId(int id)
    {
        Proyecto proyecto = ObtenerProyectoDominioPorId(id);
        return ProyectoDTO.DesdeEntidad(proyecto);
    }

    public void ModificarNombreDelProyecto(int idProyecto, string nuevoNombre, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);

        VerificarNombreNoRepetido(nuevoNombre);

        string nombreAnterior = proyecto.Nombre;

        proyecto.ModificarNombre(nuevoNombre);
        
        _proyectos.Actualizar(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.NombreProyectoModificado(nombreAnterior, proyecto.Nombre));
    }

    public void ModificarDescripcionDelProyecto(int idProyecto, string descripcion, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);

        proyecto.ModificarDescripcion(descripcion);
        
        _proyectos.Actualizar(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.DescripcionProyectoModificada(proyecto.Nombre, proyecto.Descripcion));
    }

    public void ModificarFechaDeInicioDelProyecto(int idProyecto, DateTime nuevaFecha, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);

        proyecto.ModificarFechaInicio(nuevaFecha);

        _caminoCritico.CalcularCaminoCritico(proyecto);
        
        _proyectos.Actualizar(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.FechaInicioProyectoModificada(proyecto.Nombre, nuevaFecha));
    }

    public void CambiarAdministradorDeProyecto(UsuarioDTO solicitanteDTO, int idProyecto, int idNuevoAdmin)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        PermisosUsuarios.VerificarPermisoAdminSistema(solicitante, "cambiar el administrador del proyecto");

        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(idNuevoAdmin, proyecto);

        Usuario nuevoAdmin = ObtenerMiembro(idNuevoAdmin, proyecto);

        PermisosUsuarios.VerificarUsuarioNoAdministraOtroProyecto(nuevoAdmin);

        PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(nuevoAdmin, "el nuevo administrador");

        Usuario adminAnterior = proyecto.Administrador;
        
        adminAnterior.EstaAdministrandoUnProyecto = false;
        proyecto.Administrador = nuevoAdmin;
        nuevoAdmin.EstaAdministrandoUnProyecto = true;
        
        _proyectos.Actualizar(proyecto);
        _repositorioUsuarios.Actualizar(adminAnterior);
        _repositorioUsuarios.Actualizar(nuevoAdmin);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.AdministradorProyectoModificado(proyecto.Nombre, nuevoAdmin.ToString()));
    }

    public void AgregarMiembroAProyecto(int idProyecto, UsuarioDTO solicitanteDTO, UsuarioDTO nuevoMiembroDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Usuario nuevoMiembro = ObtenerUsuarioPorDTO(nuevoMiembroDTO);

        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);

        proyecto.AsignarMiembro(nuevoMiembro);
        
        _proyectos.Actualizar(proyecto);
        _repositorioUsuarios.Actualizar(nuevoMiembro);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.MiembroAgregado(proyecto.Nombre, nuevoMiembro.Id));
    }

    public void EliminarMiembroDelProyecto(int idProyecto, UsuarioDTO solicitanteDTO, int idMiembroAEliminar)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);

        PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(idMiembroAEliminar, proyecto);

        VerificarUsuarioNoTieneTareasAsignadas(idProyecto, idMiembroAEliminar);

        Usuario miembroAEliminar = ObtenerMiembro(idMiembroAEliminar, proyecto);
        
        proyecto.EliminarMiembro(idMiembroAEliminar);
        
        _proyectos.Actualizar(proyecto);
        _repositorioUsuarios.Actualizar(miembroAEliminar);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.MiembroEliminado(proyecto.Nombre, idMiembroAEliminar));
    }

    public List<ProyectoDTO> ObtenerProyectosPorUsuario(int idUsuario)
    {
        List<Proyecto> proyectosDelUsuario =
            _proyectos.ObtenerTodos().Where(proyecto => proyecto.EsMiembro(idUsuario)).ToList();
        return proyectosDelUsuario.Select(ProyectoDTO.DesdeEntidad).ToList();
    }

    public Proyecto ObtenerProyectoDelAdministrador(int idAdministrador)
    {
        Proyecto proyecto = _proyectos.ObtenerTodos().FirstOrDefault(p => p.Administrador.Id == idAdministrador);

        if (proyecto == null)
        {
            throw new ExcepcionProyecto(MensajesErrorServicios.UsuarioNoAdministraProyectos);
        }

        return proyecto;
    }

    public void VerificarUsuarioNoTieneTareasAsignadas(int idProyecto, int idMiembroAEliminar)
    {
        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);
        Usuario miembroAEliminar = ObtenerMiembro(idMiembroAEliminar, proyecto);
        if (proyecto.Tareas.Any(tarea => tarea.EsMiembro(miembroAEliminar)))
        {
            throw new ExcepcionProyecto(MensajesErrorServicios.UsuarioConTareas);
        }
    }

    public void NotificarAdministradoresDeProyectos(List<Proyecto> proyectos, string mensaje)
    {
        proyectos.ForEach(proyecto => _notificador.NotificarUno(proyecto.Administrador, mensaje));
    }

    public void CalcularCaminoCritico(ProyectoDTO proyectoDTO)
    {
        Proyecto proyecto = ObtenerProyectoDominioPorId(proyectoDTO.Id);
        _caminoCritico.CalcularCaminoCritico(proyecto);
        _proyectos.Actualizar(proyecto);
        proyecto.Tareas.ToList().ForEach(tarea => _proyectos.ActualizarTarea(tarea));
    }
    
    public void AsignarLider(int idProyecto, UsuarioDTO solicitanteDTO, int idNuevoLider)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);
        PermisosUsuarios.VerificarUsuarioEsAdminDeEseProyectoOAdminSistema(solicitante, proyecto);

        Usuario nuevoLider = ObtenerUsuarioPorDTO(new UsuarioDTO { Id = idNuevoLider });

        proyecto.AsignarLider(nuevoLider);
        _repositorioUsuarios.Actualizar(nuevoLider);
        _proyectos.Actualizar(proyecto);
        
        string mensaje = MensajesNotificacion.LiderAsignado(proyecto.Nombre, nuevoLider.ToString());
        _notificador.NotificarMuchos(proyecto.Miembros.ToList(), mensaje);
    }
    
    public void DesasignarLider(int idProyecto, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);
        PermisosUsuarios.VerificarUsuarioEsAdminDeEseProyectoOAdminSistema(solicitante, proyecto);
        
        Usuario liderMiembros = proyecto.Miembros.FirstOrDefault(m => m.EsLider == true);
        Usuario liderAEliminar = ObtenerUsuarioPorDTO(new UsuarioDTO { Id =  liderMiembros.Id});
        proyecto.DesasignarLider(liderAEliminar);
        _repositorioUsuarios.Actualizar(liderAEliminar);
        _proyectos.Actualizar(proyecto);
        
        string mensaje = MensajesNotificacion.LiderDesasignado(proyecto.Nombre, liderAEliminar.ToString());
        _notificador.NotificarMuchos(proyecto.Miembros.ToList(), mensaje);
    }

    public bool ExisteLiderEnProyecto(int idProyecto)
    {
        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);
        if (proyecto.Lider == null)
        {
            return false;
        }
        return true;
    }
    public bool EsAdministradorDeProyecto(UsuarioDTO usuarioDTO, int idProyecto)
    {
        Usuario usuario = ObtenerUsuarioPorDTO(usuarioDTO);
        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);
        return proyecto.EsAdministrador(usuario);
    }

    public bool EsMiembroDeProyecto(int idUsuario, int idProyecto)
    {
        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);
        return proyecto.EsMiembro(idUsuario);
    }

    public bool EsLiderDeProyecto(UsuarioDTO usuarioDTO, int idProyecto)
    {
        Usuario usuario = ObtenerUsuarioPorDTO(usuarioDTO);
        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);
        return proyecto.EsLider(usuario);
    }

    public Proyecto ObtenerProyectoDominioPorId(int id)
    {
        Proyecto proyecto = _proyectos.ObtenerPorId(id);

        if (proyecto is null)
        {
            throw new ExcepcionProyecto(MensajesErrorServicios.ProyectoNoEncontrado);
        }

        return proyecto;
    }
    
    private void BorrarDependencias(Proyecto proyecto)
    {
        foreach (Tarea tarea in proyecto.Tareas)
        {
            tarea.Dependencias.Clear();
            _proyectos.ActualizarTarea(tarea);
        }
    }

    private Usuario ObtenerMiembro(int idMiembro, Proyecto proyecto)
    {
        Usuario miembro = proyecto.Miembros.FirstOrDefault(usuario => usuario.Id == idMiembro);
        return miembro;
    }

    private void VerificarNombreNoRepetido(string nuevoNombre)
    {
        bool existeOtro = _proyectos.ObtenerTodos().Any(proyecto => proyecto.Nombre == nuevoNombre);

        if (existeOtro)
        {
            throw new ExcepcionProyecto(MensajesErrorServicios.NombreRepetido);
        }
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