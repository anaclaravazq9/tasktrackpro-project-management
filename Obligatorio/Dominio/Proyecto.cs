using Excepciones;
using Excepciones.MensajesError;

namespace Dominio;

public class Proyecto
{
    private static int _maximoCaracteresDescripcion = 400;
    public int Id { get; set; }
    public string Nombre { get; private set; }
    public string Descripcion { get; private set; }
    public virtual ICollection<Tarea> Tareas { get; }
    public Usuario Administrador { get; set; }
    public Usuario? Lider { get;  set; } 
    public virtual ICollection<Usuario> Miembros { get; }
    public DateTime FechaInicio { get; set; } = DateTime.Today;
    public DateTime FechaFinMasTemprana { get; set; } = DateTime.MaxValue;

    public Proyecto()
    {
        Tareas = new List<Tarea>();
        Miembros = new List<Usuario>();
    }
    
    public Proyecto(string nombre, string descripcion, DateTime fechaInicio, Usuario administrador,
        List<Usuario> miembros)
    {
        ValidarTextoObligatorio(nombre, MensajesErrorDominio.NombreProyectoVacio);
        ValidarTextoObligatorio(descripcion, MensajesErrorDominio.DescripcionVacia);
        ValidarNoNulo(administrador, MensajesErrorDominio.ProyectoSinAdministrador);
        ValidarNoNulo(miembros, MensajesErrorDominio.MiembrosProyectoNull);
        ValidarLargoDescripción(descripcion);
        ValidarFechaInicioMayorAActual(fechaInicio);

        Miembros = miembros;
        ValidarAdministradorEsteEnMiembros(administrador);

        Nombre = nombre;
        Descripcion = descripcion;
        Tareas = new List<Tarea>();
        Administrador = administrador;
        FechaInicio = fechaInicio;
        FechaFinMasTemprana = fechaInicio.AddDays(100000);
        miembros.ForEach(usuario => usuario.CantidadProyectosAsignados++);
    }

    public void AgregarTarea(Tarea tarea)
    {
        ValidarNoNulo(tarea, MensajesErrorDominio.TareaNull);
        ValidarTareaNoDuplicada(tarea);

        Tareas.Add(tarea);
    }

    public void EliminarTarea(int idTarea)
    {
        Tarea tareaAEliminar = BuscarTareaPorId(idTarea);

        ValidarNoNulo(tareaAEliminar, MensajesErrorDominio.TareaNoPerteneceAlProyecto);

        Tareas.Remove(tareaAEliminar);
    }

    public void AsignarMiembro(Usuario usuario)
    {
        ValidarNoNulo(usuario, MensajesErrorDominio.MiembroNull);
        ValidarUsuarioNoSeaMiembro(usuario);

        Miembros.Add(usuario);
        usuario.CantidadProyectosAsignados++;
    }

    public void EliminarMiembro(int idUsuario)
    {
        Usuario usuarioAEliminar = BuscarUsuarioPorId(idUsuario);

        ValidarNoNulo(usuarioAEliminar, MensajesErrorDominio.UsuarioNoEsMiembroDelProyecto);
        ValidarQueUsuarioAEliminarNoSeaAdministrador(usuarioAEliminar);
        if (Lider != null && Lider.Id == usuarioAEliminar.Id)
        {
            usuarioAEliminar.RemoverRolLider();
            Lider = null;
        }
        Miembros.Remove(usuarioAEliminar);
        usuarioAEliminar.CantidadProyectosAsignados--;
    }

    public bool EsAdministrador(Usuario usuario)
    {
        return Administrador.Equals(usuario);
    }

    public void ModificarFechaInicio(DateTime nuevaFecha)
    {
        ValidarFechaInicioMayorAActual(nuevaFecha);
        ValidarFechaInicioNoPosteriorAFechaInicioDeTareas(nuevaFecha);
        ValidarFechaInicioMenorAFechaFinMasTemprana(nuevaFecha, FechaFinMasTemprana);

        FechaInicio = nuevaFecha;
    }

    public void ModificarFechaFinMasTemprana(DateTime nuevaFecha)
    {
        ValidarFechaFinMayorAInicio(nuevaFecha);
        ValidarFechaFinNoMenorALaDeLasTareas(nuevaFecha);

        FechaFinMasTemprana = nuevaFecha;
    }

    public void ModificarNombre(string nombreNuevo)
    {
        ValidarTextoObligatorio(nombreNuevo, MensajesErrorDominio.NombreProyectoVacio);

        Nombre = nombreNuevo;
    }

    public void ModificarDescripcion(string nuevaDescripcion)
    {
        ValidarTextoObligatorio(nuevaDescripcion, MensajesErrorDominio.DescripcionVacia);
        ValidarLargoDescripción(nuevaDescripcion);

        Descripcion = nuevaDescripcion;
    }

    public void AsignarNuevoAdministrador(Usuario nuevoAdministrador)
    {
        ValidarUsuarioEnMiembros(nuevoAdministrador.Id);

        foreach (Usuario usuario in Miembros)
        {
            if (usuario.Equals(nuevoAdministrador))
            {
                Administrador.EstaAdministrandoUnProyecto = false;
                Administrador = usuario;
                Administrador.EstaAdministrandoUnProyecto = true;
            }
        }
    }
    
    public void AsignarLider(Usuario usuario)
    {
        ValidarLiderCandidato(usuario);
        RemoverRolLiderAnteriorSiExiste();
        Lider = usuario;
        Lider.AsignarRolLider();
        Lider.CantidadProyectosLiderando++;
    }
    
    public void DesasignarLider(Usuario usuario)
    {
        if (!EsLider(usuario))
            throw new ExcepcionDominio(MensajesErrorDominio.UsuarioNoLider);

        usuario.CantidadProyectosLiderando--;

        if (usuario.CantidadProyectosLiderando <= 0)
        {
            usuario.RemoverRolLider();
        }

        Lider = null;
    }

    
    public bool EsLider(Usuario usuario)
    {
        return Lider != null && Lider.Equals(usuario);
    }
    
    private void ValidarLiderCandidato(Usuario usuario)
    {
        ValidarNoNulo(usuario, MensajesErrorDominio.LiderNull);
        ValidarUsuarioEnMiembros(usuario.Id);
        ValidarLiderDistintoAlAnterior(usuario);
    }

    private void ValidarLiderDistintoAlAnterior(Usuario usuario)
    {
        if (Lider != null && Lider.Equals(usuario))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.UsuarioYaEsLider);
        }
    }


    private void RemoverRolLiderAnteriorSiExiste()
    {
        if (Lider != null)
        {
            Lider.CantidadProyectosLiderando--;
            if (Lider.CantidadProyectosLiderando <= 0)
            {
                Lider.RemoverRolLider();
            }
        }
    }

    public bool EsMiembro(int idUsuario)
    {
        return Miembros.Any(u => u.Id == idUsuario);
    }

    public bool TieneTareas()
    {
        return Tareas.Any();
    }

    public void Actualizar(Proyecto proyectoActualizado)
    {
        ValidarIdentidad(proyectoActualizado);

        ModificarNombre(proyectoActualizado.Nombre);
        ModificarDescripcion(proyectoActualizado.Descripcion);
        FechaInicio = proyectoActualizado.FechaInicio; // se evita la validación para poder editar proyectos que ya iniciaron. Las validaciones al crear/actualizar se hacen en interfaz/servicios
        FechaFinMasTemprana = proyectoActualizado.FechaFinMasTemprana;
    }

    private Tarea BuscarTareaPorId(int id)
    {
        return Tareas.FirstOrDefault(t => t.Id == id);
    }

    private Usuario BuscarUsuarioPorId(int id)
    {
        return Miembros.FirstOrDefault(u => u.Id == id);
    }

    private void ValidarLargoDescripción(string descripcion)
    {
        if (descripcion.Length > _maximoCaracteresDescripcion)
        {
            throw new ExcepcionDominio(string.Format(MensajesErrorDominio.DescripcionMuyLarga,
                _maximoCaracteresDescripcion));
        }
    }

    private void ValidarTextoObligatorio(string valor, string mensajeError)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ExcepcionDominio(mensajeError);
        }
    }

    private void ValidarNoNulo(object objeto, string mensajeError)
    {
        if (objeto is null)
        {
            throw new ExcepcionDominio(mensajeError);
        }
    }

    private void ValidarTareaNoDuplicada(Tarea tarea)
    {
        if (Tareas.Contains(tarea))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.TareaYaAgregada);
        }
    }

    private void ValidarUsuarioEnMiembros(int idUsuario)
    {
        Usuario usuario = BuscarUsuarioPorId(idUsuario);
        ValidarNoNulo(usuario, MensajesErrorDominio.UsuarioNoEsMiembroDelProyecto);
    }

    private void ValidarQueUsuarioAEliminarNoSeaAdministrador(Usuario usuario)
    {
        if (EsAdministrador(usuario))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.NoPuedeEliminarAdmin);
        }
    }

    private void ValidarAdministradorEsteEnMiembros(Usuario administrador)
    {
        if (!Miembros.Contains(administrador))
        {
            Miembros.Add(administrador);
        }
    }

    private void ValidarUsuarioNoSeaMiembro(Usuario usuario)
    {
        if (Miembros.Contains(usuario))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.MiembroYaEnProyecto);
        }
    }

    private void ValidarFechaInicioMayorAActual(DateTime fecha)
    {
        if (fecha < DateTime.Today)
        {
            throw new ExcepcionDominio(MensajesErrorDominio.FechaInicioProyectoMenorAHoy);
        }
    }

    private void ValidarFechaInicioNoPosteriorAFechaInicioDeTareas(DateTime nuevaFecha)
    {
        if (Tareas.Any(t => nuevaFecha > t.FechaInicioMasTemprana))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.FechaInicioProyectoMayorQueTareas);
        }
    }

    private void ValidarFechaFinMayorAInicio(DateTime fecha)
    {
        if (fecha < FechaInicio)
        {
            throw new ExcepcionDominio(MensajesErrorDominio.FechaFinProyectoMenorQueInicio);
        }
    }

    private void ValidarFechaFinNoMenorALaDeLasTareas(DateTime fecha)
    {
        if (Tareas.Any(tarea => tarea.FechaFinMasTemprana > DateTime.MinValue && fecha < tarea.FechaFinMasTemprana))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.FechaFinProyectoMenorQueTareas);
        }
    }

    private void ValidarFechaInicioMenorAFechaFinMasTemprana(DateTime inicio, DateTime fin)
    {
        if (inicio > fin)
        {
            throw new ExcepcionDominio(MensajesErrorDominio.FechaInicioProyectoMayorQueFin);
        }

        if (inicio == fin)
        {
            throw new ExcepcionDominio(MensajesErrorDominio.FechaInicioProyectoIgualFin);
        }
    }
    
    private void ValidarIdentidad(Proyecto otroProyecto)
    {
        if (!Equals(otroProyecto))
        {
            throw new ExcepcionProyecto(MensajesErrorDominio.ActualizarEntidadNoCoincidente);
        }
    }

    public override bool Equals(object? otro)
    {
        Proyecto otroProyecto = otro as Proyecto;
        return otroProyecto != null && Id == otroProyecto.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}