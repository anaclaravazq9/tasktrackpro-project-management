using Excepciones;
using Excepciones.MensajesError;

namespace Dominio;

public class Tarea
{
    public int Id { get; set; }
    public string Titulo { get; private set; }
    public string Descripcion { get; private set; }
    public int DuracionEnDias { get; private set; }
    public DateTime FechaInicioMasTemprana { get; set; }
    public DateTime FechaFinMasTemprana { get; set; }
    public DateTime FechaDeEjecucion { get; private set; } = DateTime.MinValue;
    public EstadoTarea Estado { get; set; } = EstadoTarea.Pendiente;
    public int Holgura { get; set; }
    public bool FechaInicioFijadaManualmente { get; set; } = false;
    public virtual ICollection<Usuario> UsuariosAsignados { get; set; }
    public virtual ICollection<RecursoNecesario> RecursosNecesarios { get; set; }
    public virtual ICollection<Dependencia> Dependencias { get; set; }

    public Tarea()
    {
        UsuariosAsignados = new List<Usuario>();
        RecursosNecesarios = new List<RecursoNecesario>();
        Dependencias = new List<Dependencia>();
    }
    
    public Tarea(string titulo, string descripcion, int duracionEnDias, DateTime fechaInicioMasTemprana)
    {
        ValidarStringNoVacioNiNull(titulo, MensajesErrorDominio.TituloTareaVacio);
        ValidarIntNoNegativoNiCero(duracionEnDias, MensajesErrorDominio.DuracionTareaInvalida);
        ValidarStringNoVacioNiNull(descripcion, MensajesErrorDominio.DescripcionVacia);
        ValidarFechaInicio(fechaInicioMasTemprana);
        
        Titulo = titulo;
        Descripcion = descripcion;
        DuracionEnDias = duracionEnDias;
        FechaInicioMasTemprana = fechaInicioMasTemprana;
        Estado = EstadoTarea.Pendiente;
        UsuariosAsignados = new List<Usuario>();
        RecursosNecesarios = new List<RecursoNecesario>();
        Dependencias = new List<Dependencia>();
        
        CalcularFechaFinMasTemprana();
    }

    public void CambiarEstado(EstadoTarea nuevoEstado)
    {
        ValidarTransicionInvalida(Estado, nuevoEstado);
        
        Estado = nuevoEstado;
        
        if (nuevoEstado == EstadoTarea.EnProceso)
        {
            ModificarFechaInicioMasTemprana(DateTime.Today);
        }
        else if (nuevoEstado == EstadoTarea.Completada)
        {
            LiberarRecursos();
            FechaDeEjecucion = DateTime.Today;
            FechaFinMasTemprana = DateTime.Today;
            DuracionEnDias = (int)(FechaFinMasTemprana - FechaInicioMasTemprana).TotalDays + 1;
            Holgura = 0;
        }
    }

    public void AsignarUsuario(Usuario usuario)
    {
        ValidarObjetoNoNull(usuario, MensajesErrorDominio.UsuarioNullEnAsignacion);
        VerificarUsuarioNoEstaAsignado(usuario);
        
        UsuariosAsignados.Add(usuario);
    }

    public void EliminarUsuario(int idUsuario)
    {
        VerificarMiembrosNoVacia();
        Usuario usuarioAEliminar = BuscarMiembroPorId(idUsuario);
        ValidarObjetoNoNull(usuarioAEliminar, MensajesErrorDominio.UsuarioNoAsignado);
        UsuariosAsignados.Remove(usuarioAEliminar);
    }

    public void AsignarRecurso(Recurso recurso, int cantidad)
    {
        ValidarObjetoNoNull(recurso, MensajesErrorDominio.RecursoNullParaAgregar);
        VerificarRecursoNoEstaAgregado(recurso);
        recurso.AgregarRangoDeUso(FechaInicioMasTemprana, FechaFinMasTemprana, cantidad);
        RecursoNecesario recursoNecesario = new RecursoNecesario(recurso, cantidad);
        RecursosNecesarios.Add(recursoNecesario);
        recurso.IncrementarCantidadDeTareasUsandolo();
    }
    
    public void AsignarRecursoForzado(Recurso recurso, int cantidad)
    {
        ValidarObjetoNoNull(recurso, MensajesErrorDominio.RecursoNullParaAgregar);
        VerificarRecursoNoEstaAgregado(recurso);
        recurso.AgregarRangoDeUsoForzado(FechaInicioMasTemprana, FechaFinMasTemprana, cantidad);
        RecursoNecesario recursoNecesario = new RecursoNecesario(recurso, cantidad);
        RecursosNecesarios.Add(recursoNecesario);
        recurso.IncrementarCantidadDeTareasUsandolo();
    }

    public void EliminarRecurso(int idRecurso)
    {
        RecursoNecesario recursoAEliminar = BuscarRecursoNecesarioPorId(idRecurso);
        ValidarObjetoNoNull(recursoAEliminar, MensajesErrorDominio.RecursoNoNecesario);
        int cantidad = recursoAEliminar.Cantidad;
        recursoAEliminar.Recurso.EliminarRango(FechaInicioMasTemprana, FechaFinMasTemprana, cantidad);
        recursoAEliminar.Recurso.DecrementarCantidadDeTareasUsandolo();
        RecursosNecesarios.Remove(recursoAEliminar);
    }

    public void AgregarDependencia(Dependencia dependencia)
    {
        ValidarObjetoNoNull(dependencia, MensajesErrorDominio.DependenciaNullEnTarea);
        VerificarDependenciaNoEstaAgregada(dependencia);
        
        Dependencias.Add(dependencia);
        ActualizarEstadoBloqueadaOPendiente();
    }

    public void EliminarDependencia(int idTarea)
    {
        Dependencia dependenciaAEliminar = BuscarDependenciaPorIdDeTarea(idTarea);
        ValidarObjetoNoNull(dependenciaAEliminar, MensajesErrorDominio.DependenciaNoExisteEnTarea);
        
        Dependencias.Remove(dependenciaAEliminar);
        ActualizarEstadoBloqueadaOPendiente();
    }

    public void ModificarTitulo(string tituloNuevo)
    {
        ValidarStringNoVacioNiNull(tituloNuevo, MensajesErrorDominio.TituloTareaVacio);
        Titulo = tituloNuevo;
    }

    public void ModificarDescripcion(string nuevaDescripcion)
    {
        ValidarStringNoVacioNiNull(nuevaDescripcion, MensajesErrorDominio.DescripcionVacia);
        Descripcion = nuevaDescripcion;
    }

    public void ModificarDuracion(int nuevaDuracion)
    {
        DuracionNoMenorACero(nuevaDuracion);
        DuracionEnDias = nuevaDuracion;
        CalcularFechaFinMasTemprana();
    }

    public void ModificarFechaInicioMasTemprana(DateTime fechaInicioNueva)
    {
        VerificarFechaNoMenorAHoy(fechaInicioNueva);
        FechaInicioMasTemprana = fechaInicioNueva;
        CalcularFechaFinMasTemprana();
    }
    
    public void FijarFechaInicio(DateTime fechaInicioNueva)
    {
        if (Dependencias.Any())
        {
            DateTime fechaMinimaPorDependencias = CalcularFechaMinimaPorDependencias();
            ValidarFechaInicioMayorAFechaMinimaPorDependencias(fechaInicioNueva, fechaMinimaPorDependencias);
        }
        ModificarFechaInicioMasTemprana(fechaInicioNueva);
        FechaInicioFijadaManualmente = true;
    }
    
    public bool EsCritica()
    {
        return Holgura == 0;
    }

    public void ActualizarEstadoBloqueadaOPendiente()
    {
        if (Estado == EstadoTarea.Bloqueada || Estado == EstadoTarea.Pendiente)
        {
            if (SePuedeRealizar())
            {
                CambiarEstado(EstadoTarea.Pendiente);
            }
            else
            {
                CambiarEstado(EstadoTarea.Bloqueada);
            }
        }
    }
    
    public bool UsaRecurso(int idRecurso)
    {
        return RecursosNecesarios.Any(rn => rn.Recurso.Id == idRecurso);
    }
    
    public bool EsMiembro(Usuario usuario)
    {
        return UsuariosAsignados.Contains(usuario);
    }

    public bool EsSucesoraDe(int idTarea)
    {
        return Dependencias.Any(d => d.Tarea.Id == idTarea);
    }
    
    public void Actualizar(Tarea tareaActualizada)
    {
        ValidarIdentidad(tareaActualizada);
        
        ModificarTitulo(tareaActualizada.Titulo);
        ModificarDescripcion(tareaActualizada.Descripcion);
        ModificarDuracion(tareaActualizada.DuracionEnDias);
        FechaInicioMasTemprana = tareaActualizada.FechaInicioMasTemprana; // se evita la validación para poder editar tareas que ya iniciaron. Las validaciones al crear/actualizar se hacen en interfaz/servicios
        FechaFinMasTemprana = tareaActualizada.FechaFinMasTemprana;
        FechaDeEjecucion = tareaActualizada.FechaDeEjecucion;
        Holgura = tareaActualizada.Holgura;
        FechaInicioFijadaManualmente = tareaActualizada.FechaInicioFijadaManualmente;
        CambiarEstado(tareaActualizada.Estado);
    }

    private void CalcularFechaFinMasTemprana()
    {
        FechaFinMasTemprana = FechaInicioMasTemprana.AddDays(DuracionEnDias - 1);
    }

    private void VerificarFechaNoMenorAHoy(DateTime fechaNueva)
    {
        if (fechaNueva < DateTime.Now.Date)
        {
            throw new ExcepcionDominio(MensajesErrorDominio.FechaTareaInvalida);
        }
    }

    private void DuracionNoMenorACero(int duracion)
    {
        if (duracion <= 0)
        {
            throw new ExcepcionDominio(MensajesErrorDominio.DuracionTareaInvalida);
        }
    }

    private void VerificarMiembrosNoVacia()
    {
        if (!UsuariosAsignados.Any())
        {
            throw new ExcepcionDominio(MensajesErrorDominio.UsuariosAsignadosVacio);
        }
    }

    private void ValidarStringNoVacioNiNull(string valor, string mensajeError)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ExcepcionDominio(mensajeError);
    }

    private void ValidarIntNoNegativoNiCero(int valor, string mensajeError)
    {
        if (valor <= 0)
            throw new ExcepcionDominio(mensajeError);
    }

    private void ValidarFechaInicio(DateTime? fechaInicio)
    {
        if (fechaInicio.HasValue && fechaInicio.Value.Date < DateTime.Today)
        {
            throw new ExcepcionDominio(MensajesErrorDominio.FechaInicioInvalida);
        }
    }

    private void ValidarObjetoNoNull(object objeto, string mensajeError)
    {
        if (objeto is null)
            throw new ExcepcionDominio(mensajeError);
    }
    
    private void ValidarFechaInicioMayorAFechaMinimaPorDependencias(DateTime fechaInicioNueva, DateTime fechaMinimaPorDependencias)
    {
        if(fechaInicioNueva < fechaMinimaPorDependencias)
        {
            throw new ExcepcionTarea(MensajesErrorDominio.FechaMenorAFechMinima);
        }
    }
    private void ValidarTransicionInvalida(EstadoTarea actual, EstadoTarea nuevo)
    {
        if ((actual == EstadoTarea.Completada && nuevo == EstadoTarea.Pendiente) ||
            (actual == EstadoTarea.Completada && nuevo == EstadoTarea.EnProceso) ||
            (actual == EstadoTarea.Completada && nuevo == EstadoTarea.Bloqueada) ||
            (actual == EstadoTarea.EnProceso && nuevo == EstadoTarea.Pendiente) ||
            (actual == EstadoTarea.Pendiente && nuevo == EstadoTarea.Completada) ||
            (actual == EstadoTarea.Bloqueada && nuevo == EstadoTarea.Completada))
        {
            throw new ExcepcionDominio(
                MensajesErrorDominio.TransicionEstadoInvalidaDesdeHacia(actual.ToString(), nuevo.ToString()));
        }
    }

    private void VerificarUsuarioNoEstaAsignado(Usuario usuario)
    {
        if (UsuariosAsignados.Contains(usuario))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.UsuarioYaAgregado);
        }
    }

    private void VerificarRecursoNoEstaAgregado(Recurso recurso)
    {
        if (RecursosNecesarios.Any(rn => rn.Recurso.Id == recurso.Id))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.RecursoYaAgregado);
        }
    }

    private void VerificarDependenciaNoEstaAgregada(Dependencia dependencia)
    {
        if (Dependencias.Contains(dependencia))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.DependenciaYaAgregada);
        }
    }

    private Usuario BuscarMiembroPorId(int id)
    {
        return UsuariosAsignados.FirstOrDefault(u => u.Id == id);
    }

    private RecursoNecesario BuscarRecursoNecesarioPorId(int id)
    {
        return RecursosNecesarios.FirstOrDefault(rn => rn.Recurso.Id == id);
    }

    private Dependencia BuscarDependenciaPorIdDeTarea(int id)
    {
        return Dependencias.FirstOrDefault(d => d.Tarea.Id == id);
    }

    private void LiberarRecursos()
    {
        foreach (RecursoNecesario recursoNecesario in RecursosNecesarios)
        {
            recursoNecesario.Recurso.DecrementarCantidadDeTareasUsandolo();
        }
    }
    
    private DateTime CalcularFechaMinimaPorDependencias()
    {
        List<DateTime> fechas = new List<DateTime>();

        foreach (Dependencia dependencia in Dependencias)
        {
            if (dependencia.Tipo == "FS")
            {
                fechas.Add(dependencia.Tarea.FechaFinMasTemprana.AddDays(1));
            }
            else if (dependencia.Tipo == "SS")
            {
                fechas.Add(dependencia.Tarea.FechaInicioMasTemprana);
            }
        }
        return fechas.Max();
    }
    private bool SePuedeRealizar()
    {
        bool dependenciasFSCompletas =
            Dependencias.Where(d => d.Tipo == "FS").All(d => d.Tarea.Estado == EstadoTarea.Completada);
        bool dependenciasSSEnProcesoOCompletadas = Dependencias.Where(d => d.Tipo == "SS")
            .All(d => d.Tarea.Estado == EstadoTarea.EnProceso || d.Tarea.Estado == EstadoTarea.Completada);
        
        return dependenciasFSCompletas && dependenciasSSEnProcesoOCompletadas;
    }
    private void ValidarIdentidad(Tarea otraTarea)
    {
        if (!Equals(otraTarea))
        {
            throw new ExcepcionTarea(MensajesErrorDominio.ActualizarEntidadNoCoincidente);
        }
    }
    public override bool Equals(object obj)
    {
        if (obj is not Tarea otra) return false;
        return Id == otra.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}