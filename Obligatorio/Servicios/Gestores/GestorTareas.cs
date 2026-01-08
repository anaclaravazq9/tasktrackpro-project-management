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

public class GestorTareas : IGestorTareas
{
    private readonly IRepositorioProyectos _repositorioProyectos;
    private IRepositorioUsuarios _repositorioUsuarios;
    private IRepositorio<Recurso> _repositorioRecursos;
    private readonly INotificador _notificador;
    private readonly ICalculadorCaminoCritico _caminoCritico;

    public GestorTareas(IRepositorioProyectos repositorioProyectos, IRepositorioUsuarios repositorioUsuarios,
        IRepositorio<Recurso> repositorioRecursos,
        INotificador notificador, ICalculadorCaminoCritico caminoCritico)
    {
        _repositorioProyectos = repositorioProyectos;
        _repositorioUsuarios = repositorioUsuarios;
        _repositorioRecursos = repositorioRecursos;
        _notificador = notificador;
        _caminoCritico = caminoCritico;
    }

    public void AgregarTareaAlProyecto(int idProyecto, UsuarioDTO solicitanteDTO, TareaDTO nuevaTareaDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea nuevaTarea = nuevaTareaDTO.AEntidad();

        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);

        ValidarTareaIniciaDespuesDelProyecto(proyecto, nuevaTarea);

        proyecto.AgregarTarea(nuevaTarea);

        RecalcularCaminoCriticoYActualizarProyecto(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.TareaAgregada(nuevaTarea.Id, proyecto.Nombre));

        nuevaTareaDTO.Id = nuevaTarea.Id;
    }

    public void EliminarTareaDelProyecto(int idProyecto, UsuarioDTO solicitanteDTO, int idTareaAEliminar)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoValidandoAdminOLider(idProyecto, solicitante);

        Tarea tareaAEliminar = proyecto.Tareas.FirstOrDefault(t => t.Id == idTareaAEliminar);

        ValidarTareaExistente(tareaAEliminar);

        ValidarTareaNoTieneSucesora(proyecto, idTareaAEliminar);

        VerificarTareaNoEsteEnProceso(tareaAEliminar);

        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTareaAEliminar);

        foreach (RecursoNecesario recursoNecesario in tarea.RecursosNecesarios.ToList())
        {
            tarea.EliminarRecurso(recursoNecesario.Recurso.Id);
        }

        proyecto.EliminarTarea(tareaAEliminar.Id);

        RecalcularCaminoCriticoYActualizarProyecto(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.TareaEliminada(idTareaAEliminar, proyecto.Nombre));
    }

    public TareaDTO ObtenerTareaPorId(int idProyecto, int idTarea)
    {
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        return TareaDTO.DesdeEntidad(tarea);
    }

    public void ModificarTituloTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, string nuevoTitulo)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea tarea = ObtenerTareaValidandoAdminOLider(solicitante, idProyecto, idTarea);

        VerificarTareaNoEsteEnProceso(tarea);

        tarea.ModificarTitulo(nuevoTitulo);

        _repositorioProyectos.ActualizarTarea(tarea);

        NotificarCambio("título", idTarea, idProyecto);
    }

    public void ModificarDescripcionTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto,
        string nuevaDescripcion)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea tarea = ObtenerTareaValidandoAdminOLider(solicitante, idProyecto, idTarea);

        VerificarTareaNoEsteEnProceso(tarea);

        tarea.ModificarDescripcion(nuevaDescripcion);

        _repositorioProyectos.ActualizarTarea(tarea);

        NotificarCambio("descripción", idTarea, idProyecto);
    }

    public void ModificarDuracionTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, int nuevaDuracion)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea tarea = ObtenerTareaValidandoAdminOLider(solicitante, idProyecto, idTarea);

        VerificarTareaNoEsteEnProceso(tarea);

        VerificarTareaNoTieneRecursos(tarea);

        tarea.ModificarDuracion(nuevaDuracion);

        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        RecalcularCaminoCriticoYActualizarProyecto(proyecto);

        NotificarCambio("duración", idTarea, idProyecto);
    }

    public void ModificarFechaInicioTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, DateTime nuevaFecha)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea tarea = ObtenerTareaValidandoAdminOLider(solicitante, idProyecto, idTarea);

        VerificarTareaNoTieneRecursos(tarea);
        VerificarTareaNoEsteEnProceso(tarea);

        tarea.FijarFechaInicio(nuevaFecha);

        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        RecalcularCaminoCriticoYActualizarProyecto(proyecto);

        NotificarCambio("fecha de inicio", idTarea, idProyecto);
    }

    public void CambiarEstadoTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto,
        EstadoTareaDTO nuevoEstadoDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        EstadoTarea nuevoEstado = (EstadoTarea)nuevoEstadoDTO;

        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);

        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        PermisosUsuarios.VerificarUsuarioTengaLaTareaAsignadaOSeaAdminOLiderDelProyecto(solicitante, tarea, proyecto);
        PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(solicitante.Id, proyecto);

        VerificarProyectoHayaComenzado(proyecto);
        VerificarEstadoEditablePorUsuario(nuevoEstado);
        
        if (nuevoEstado == EstadoTarea.EnProceso)
        {
            if (tarea.FechaInicioMasTemprana > DateTime.Today && tarea.RecursosNecesarios.Any())
            {
                throw new ExcepcionTarea(MensajesErrorServicios.NoPuedeComenzarConRecursosAntesDeFechaInicio);
            }
        }
        
        if (nuevoEstado == EstadoTarea.Completada &&
            tarea.FechaFinMasTemprana > DateTime.Today &&
            tarea.RecursosNecesarios.Any())
        {
            throw new ExcepcionTarea(MensajesErrorServicios.NoPuedeCompletarConRecursosAntesDeFechaFin);
        }
        
        
        
        tarea.CambiarEstado(nuevoEstado);

        _caminoCritico.CalcularCaminoCritico(proyecto);

        if (nuevoEstado == EstadoTarea.Completada)
        {
            foreach (RecursoNecesario recursoNecesario in tarea.RecursosNecesarios.ToList())
            {
                recursoNecesario.Recurso.EliminarRango(
                    tarea.FechaInicioMasTemprana,
                    tarea.FechaFinMasTemprana,
                    recursoNecesario.Cantidad
                );
            }
            ActualizarEstadosTareasDelProyecto(proyecto); //acá se modifican fechas en base a las recalculadas. Por ello no se puede llamar a RecalcularCaminoCriticoYActualizarProyecto();
        }

        _repositorioProyectos.Actualizar(proyecto);
        proyecto.Tareas.ToList().ForEach(tarea => _repositorioProyectos.ActualizarTarea(tarea));

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.EstadoTareaModificado(idTarea, proyecto.Nombre, nuevoEstado));
    }

    private void ActualizarEstadosTareasDelProyecto(Proyecto proyecto)
    {
        proyecto.Tareas.ToList().ForEach(tarea => tarea.ActualizarEstadoBloqueadaOPendiente());
    }
    
    public void AgregarDependenciaATarea(UsuarioDTO solicitanteDTO, int idTarea, int idTareaDependencia, int idProyecto,
        string tipoDependencia)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoValidandoAdminOLider(idProyecto, solicitante);
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);

        VerificarTareaNoEsteEnProceso(tarea);
        VerificarTareaNoTieneRecursos(tarea);

        Tarea tareaDependencia = ObtenerTareaDominioPorId(idProyecto, idTareaDependencia);
        Dependencia dependencia = new Dependencia(tipoDependencia, tareaDependencia);

        tarea.AgregarDependencia(dependencia);

        try
        {
            RecalcularCaminoCriticoYActualizarProyecto(proyecto);
        }
        catch (ExcepcionCaminoCritico)
        {
            tarea.EliminarDependencia(dependencia.Tarea.Id);
            throw new ExcepcionTarea(MensajesErrorServicios.GeneraCiclos);
        }

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.DependenciaAgregada(tarea.Id, proyecto.Nombre, tipoDependencia, tareaDependencia.Id));
    }

    public void EliminarDependenciaDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idTareaDependencia,
        int idProyecto)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Proyecto proyecto = ObtenerProyectoValidandoAdminOLider(idProyecto, solicitante);
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);

        VerificarTareaNoEsteEnProceso(tarea);
        VerificarTareaNoTieneRecursos(tarea);

        tarea.EliminarDependencia(idTareaDependencia);

        RecalcularCaminoCriticoYActualizarProyecto(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.DependenciaEliminada(idTareaDependencia, idTarea, proyecto.Nombre));
    }

    public void AgregarMiembroATarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, UsuarioDTO nuevoMiembroDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Usuario nuevoMiembro = ObtenerUsuarioPorDTO(nuevoMiembroDTO);
        Proyecto proyecto = ObtenerProyectoValidandoAdminOLider(idProyecto, solicitante);

        PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(nuevoMiembro.Id, proyecto);

        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);

        VerificarTareaNoEsteEnProceso(tarea);

        tarea.AsignarUsuario(nuevoMiembro);

        _repositorioProyectos.ActualizarTarea(tarea);

        NotificarAgregar($"miembro {nuevoMiembro.ToString()}", idTarea, idProyecto);
    }

    public void EliminarMiembroDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, UsuarioDTO miembroDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Usuario miembro = ObtenerUsuarioPorDTO(miembroDTO);
        Proyecto proyecto = ObtenerProyectoValidandoAdminOLider(idProyecto, solicitante);

        PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(miembro.Id, proyecto);

        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);

        VerificarTareaNoEsteEnProceso(tarea);

        tarea.EliminarUsuario(miembro.Id);

        _repositorioProyectos.ActualizarTarea(tarea);

        NotificarEliminar($"miembro {miembro.ToString()}", idTarea, idProyecto);
    }

    public void AsignarRecursoATarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO nuevoRecursoDTO,
        int cantidad)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso nuevoRecurso = ObtenerRecursoPorDTO(nuevoRecursoDTO);

        ObtenerProyectoValidandoAdmin(idProyecto, solicitante);

        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);

        VerificarTareaNoEsteEnProceso(tarea);

        tarea.AsignarRecurso(nuevoRecurso, cantidad);

        _repositorioProyectos.ActualizarTarea(tarea);
        _repositorioRecursos.Actualizar(nuevoRecurso);

        NotificarAgregar($"recurso {nuevoRecurso.Nombre}", idTarea, idProyecto);
    }

    public void EliminarRecursoDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO recursoDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoPorDTO(recursoDTO);

        ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        ValidarRecursoExistente(recurso, idTarea, idProyecto);

        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);

        VerificarTareaNoEsteEnProceso(tarea);

        tarea.EliminarRecurso(recurso.Id);

        _repositorioProyectos.ActualizarTarea(tarea);
        _repositorioRecursos.Actualizar(recurso);

        NotificarEliminar($"recurso {recurso.Nombre}", idTarea, idProyecto);
    }

    public void EncontrarRecursosAlternativosMismoTipo(UsuarioDTO solicitanteDTO, int idProyecto,
        RecursoDTO recursoOriginalDTO, DateTime FechaInicio, DateTime FechaFin, int cantidad)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recursoOriginal = ObtenerRecursoPorDTO(recursoOriginalDTO);
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);

        List<Recurso> todosLosRecursos = _repositorioRecursos.ObtenerTodos();

        List<Recurso> recursosAlternativos = new List<Recurso>();

        foreach (Recurso recurso in todosLosRecursos)
        {
            if (recurso.Tipo == recursoOriginal.Tipo && recurso.UsoSeAjustaANuevaCapacidad(cantidad) &&
                recurso.TieneCapacidadDisponible(FechaInicio, FechaFin, cantidad))
            {
                recursosAlternativos.Add(recurso);
            }
        }

        if (recursosAlternativos.Any())
        {
            string lista = string.Join(", ", recursosAlternativos.Select(r => r.Nombre));
            string mensaje =
                $"Se encontraron recursos alternativos disponibles del mismo tipo que '{recursoOriginal.Nombre}': {lista}.";
            _notificador.NotificarUno(proyecto.Administrador, mensaje);
        }
        else
        {
            string mensaje = $"No se encontraron recursos alternativos.";
            _notificador.NotificarUno(proyecto.Administrador, mensaje);
        }
    }

    public void ReprogramarTarea(UsuarioDTO solicitanteDTO, int idProyecto, int idTarea, RecursoDTO recursoDTO,
        int cantidad)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        Recurso recurso = ObtenerRecursoPorDTO(recursoDTO);
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);

        DateTime nuevaFechaInicio =
            recurso.BuscarProximaFechaDisponible(tarea.FechaInicioMasTemprana, tarea.DuracionEnDias, cantidad);

        string mensaje =
            $"La tarea '{tarea.Titulo}' puede reprogramarse para comenzar el {nuevaFechaInicio:dd/MM/yyyy} usando el recurso '{recurso.Nombre}' sin conflictos.";
        _notificador.NotificarUno(proyecto.Administrador, mensaje);
    }

    public void ForzarAsignacion(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO recursoDTO,
        int cantidad)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoPorDTO(recursoDTO);
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);

        ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        recurso.AgregarRangoDeUsoForzado(tarea.FechaInicioMasTemprana, tarea.FechaFinMasTemprana, cantidad);
        tarea.AsignarRecursoForzado(recurso, cantidad);

        string mensaje =
            $"Recurso {recurso.Nombre} fue asignado forzadamente a la tarea '{tarea.Titulo}', excediendo su capacidad.";
        _notificador.NotificarMuchos(proyecto.Miembros.ToList(), mensaje);
    }

    public bool EsMiembroDeTarea(UsuarioDTO usuarioDTO, int idTarea, int idProyecto)
    {
        Usuario usuario = ObtenerUsuarioPorDTO(usuarioDTO);
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        return tarea.EsMiembro(usuario);
    }

    private Tarea ObtenerTareaDominioPorId(int idProyecto, int idTarea)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        Tarea tarea = proyecto.Tareas.FirstOrDefault(t => t.Id == idTarea);

        if (tarea == null)
        {
            throw new ExcepcionTarea(MensajesErrorServicios.TareaNoExistente);
        }

        return tarea;
    }

    private Proyecto ObtenerProyectoPorId(int idProyecto)
    {
        Proyecto proyecto = _repositorioProyectos.ObtenerPorId(idProyecto);

        if (proyecto == null)
        {
            throw new ExcepcionProyecto(MensajesErrorServicios.ProyectoNoEncontrado);
        }

        return proyecto;
    }

    private Proyecto ObtenerProyectoValidandoAdmin(int idProyecto, Usuario solicitante)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");
        PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);

        return proyecto;
    }

    private void NotificarCambio(string campo, int idTarea, int idProyecto)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.CampoTareaModificado(campo, idTarea, proyecto.Nombre));
    }
   
    private void NotificarEliminar(string campo, int idTarea, int idProyecto)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.CampoTareaEliminado(campo, idTarea, proyecto.Nombre));
    }


    private void NotificarAgregar(string campo, int idTarea, int idProyecto)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.CampoTareaAgregado(campo, idTarea, proyecto.Nombre));
    }
    
    private void VerificarEstadoEditablePorUsuario(EstadoTarea estado)
    {
        if (estado != EstadoTarea.EnProceso && estado != EstadoTarea.Completada)
        {
            throw new ExcepcionTarea(MensajesErrorServicios.EstadoNoEditable);
        }
    }
    
    private void ValidarRecursoExistente(Recurso recurso, int idTarea, int idProyecto)
    {
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        if (!tarea.RecursosNecesarios.Any(rn => rn.Recurso.Equals(recurso)))
        {
            throw new ExcepcionTarea(MensajesErrorServicios.RecursoNoAsignado);
        }
    }

    private void ValidarTareaNoTieneSucesora(Proyecto proyecto, int idTarea)
    {
        if (proyecto.Tareas.Any(tarea => tarea.EsSucesoraDe(idTarea)))
        {
            throw new ExcepcionTarea(MensajesErrorServicios.TareaConSucesoras);
        }
    }

    private void ValidarTareaIniciaDespuesDelProyecto(Proyecto proyecto, Tarea nuevaTarea)
    {
        if (nuevaTarea.FechaInicioMasTemprana < proyecto.FechaInicio)
        {
            throw new ExcepcionTarea(MensajesErrorServicios.FechaInicioTarea);
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

    private Recurso ObtenerRecursoPorDTO(RecursoDTO recursoDTO)
    {
        Recurso recurso = _repositorioRecursos.ObtenerPorId(recursoDTO.Id);

        if (recurso == null)
        {
            throw new ExcepcionRecurso(MensajesErrorServicios.RecursoNoEncontrado);
        }

        return recurso;
    }

    public void ValidarYAsignarRecurso(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO recursoDTO,
        int cantidad)
    {
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        Recurso recurso = ObtenerRecursoPorDTO(recursoDTO);

        // Si la cantidad pedida directamente supera la capacidad total
        if (cantidad > recurso.Capacidad)
            throw new ExcepcionRecurso(MensajesErrorServicios.CapacidadInsuficiente);

        // Si hay conflicto en los días del rango
        if (!recurso.TieneCapacidadDisponible(tarea.FechaInicioMasTemprana, tarea.FechaFinMasTemprana, cantidad))
            throw new ExcepcionConflicto(MensajesErrorServicios.ExisteConflicto);

        // Si pasa las validaciones anteriores, asignar el recurso
        AsignarRecursoATarea(solicitanteDTO, idTarea, idProyecto, recursoDTO, cantidad);
    }
    
    private void RecalcularCaminoCriticoYActualizarProyecto(Proyecto proyecto)
    {
        _caminoCritico.CalcularCaminoCritico(proyecto);
        _repositorioProyectos.Actualizar(proyecto);
        proyecto.Tareas.ToList().ForEach(tarea => _repositorioProyectos.ActualizarTarea(tarea));
    }
    
    private Proyecto ObtenerProyectoValidandoAdminOLider(int idProyecto, Usuario solicitante)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioEsAdminOLiderDelProyecto(proyecto, solicitante);
        
        return proyecto;
    }
    
    private Tarea ObtenerTareaValidandoAdminOLider(Usuario solicitante, int idProyecto, int idTarea)
        {
            Proyecto proyecto = ObtenerProyectoValidandoAdminOLider(idProyecto, solicitante);
            return ObtenerTareaDominioPorId(proyecto.Id, idTarea);
        }

        
    private void VerificarTareaNoTieneRecursos(Tarea tarea)
    {
        if (tarea.RecursosNecesarios.Any())
        {
            throw new ExcepcionTarea(MensajesErrorServicios.TareaConRecursosAsignados);
        }
    }
    
    private void VerificarProyectoHayaComenzado(Proyecto proyecto)
    {
        if (proyecto.FechaInicio > DateTime.Today)
        {
            throw new ExcepcionTarea(MensajesErrorServicios.ProyectoNoComenzado);
        }
    }

    private void VerificarTareaNoEsteEnProceso(Tarea tarea)
    {
        if (tarea.Estado == EstadoTarea.EnProceso)
            throw new ExcepcionTarea(MensajesErrorServicios.TareaEnProceso);
    }

    private void ValidarTareaExistente(Tarea? tareaAEliminar)
    {
        if (tareaAEliminar == null)
            throw new ExcepcionTarea(MensajesErrorServicios.TareaNoExistente);
    }
}