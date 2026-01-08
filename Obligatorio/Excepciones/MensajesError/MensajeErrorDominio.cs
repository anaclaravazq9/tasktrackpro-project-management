namespace Excepciones.MensajesError;

public static class MensajesErrorDominio
{
    public const string NotificacionNoExiste = "No existe la notificación.";
    public const string AtributoVacio = "El atributo {0} no puede ser vacío.";
    public const string EdadMinima = "El usuario debe tener más de {0} años.";
    public const string EdadMaxima = "El usuario debe tener menos de {0} años.";
    public const string EmailInvalido = "El email tiene un formato inválido.";

    public const string TipoDependenciaInvalido = "El tipo de dependencia debe ser 'FS' o 'SS'.";
    public const string TareaNula = "Una tarea no puede ser nula.";

    public const string DescripcionMuyLarga = "La descripción no puede superar los {0} caracteres.";
    public const string TareaYaAgregada = "La tarea ya fue agregada al proyecto.";

    public const string NoPuedeEliminarAdmin =
        "No se puede eliminar al administrador actual. Asigne un nuevo administrador antes.";

    public const string MiembroYaEnProyecto = "El miembro ya pertenece al proyecto.";
    public const string FechaInicioProyectoMenorAHoy = "La fecha de inicio no puede ser anterior a hoy.";

    public const string FechaInicioProyectoMayorQueTareas =
        "La fecha de inicio no puede ser posterior a la de alguna tarea.";

    public const string FechaFinProyectoMenorQueInicio =
        "La fecha de fin más temprana no puede ser anterior a la fecha de inicio del proyecto.";

    public const string FechaFinProyectoMenorQueTareas =
        "La fecha de fin más temprana no puede ser menor que la fecha de fin de una tarea.";

    public const string FechaInicioProyectoMayorQueFin =
        "La fecha de inicio no puede ser mayor que la fecha de fin más temprana.";

    public const string FechaInicioProyectoIgualFin =
        "La fecha de inicio no puede ser la misma que la fecha de fin más temprana.";

    public const string NombreProyectoVacio = "El nombre del proyecto no puede estar vacío o null.";
    public const string DescripcionVacia = "La descripción no puede estar vacía o null.";
    public const string ProyectoSinAdministrador = "El proyecto debe tener un administrador.";
    public const string MiembrosProyectoNull = "La lista de miembros no puede ser null.";
    public const string TareaNull = "No se puede agregar una tarea null.";
    public const string TareaNoPerteneceAlProyecto = "La tarea no pertenece al proyecto.";
    public const string MiembroNull = "No se puede agregar un miembro null.";
    public const string LiderNull = "No se puede asignar un líder null.";
    public const string UsuarioYaEsLider = "El usuario ya es líder del proyecto.";
    public const string UsuarioNoLider = "El usuario no es el líder de este proyecto.";
    public const string UsuarioNoEsMiembroDelProyecto = "El usuario no es miembro del proyecto.";

    public const string RecursoYaEsExclusivo = "El recurso ya es exclusivo de otro proyecto.";

    public const string CantidadTareasRecursoNegativa =
        "La cantidad de tareas usando este recurso no puede ser menor a cero.";
    public const string CapacidadRecursoInvalida = "La capacidad del recurso debe ser mayor a cero.";
    public const string CapacidadRequeridaInvalida = "La capacidad requerida no puede ser mayor a la capacidad del recurso.";
    public const string CapacidadInsuficienteEnElRango = "No hay capacidad suficiente en el recurso para ese rango de fechas.";

    public const string CapacidadNoReducible =
        "No se puede reducir la capacidad del recurso porque se hace uso de la capacidad máxima en un rango de fechas.";

    public const string MensajeNotificacionVacio = "El mensaje de la notificación no puede estar vacío o null.";

    public const string FechaTareaInvalida = "La fecha no puede ser anterior a hoy.";
    public const string DuracionTareaInvalida = "La duración de la tarea debe ser mayor a cero días.";
    public const string UsuariosAsignadosVacio = "La lista de usuarios asignados está vacía o no está inicializada.";
    public const string FechaInicioInvalida = "La fecha de inicio debe ser igual o posterior a la fecha de hoy.";
    public const string EdicionTareaEnProceso = "No se puede editar la tarea mientras está en proceso.";
    public const string FechaMenorAFechMinima =
        "La fecha de inicio de la tarea no puede ser anterior a la permitida por sus dependencias.";


    public static string TransicionEstadoInvalidaDesdeHacia(string desde, string hacia)
        => $"No se puede cambiar una tarea de estado {desde} a estado {hacia}.";


    public const string UsuarioYaAgregado = "El usuario ya fue agregado a la tarea.";
    public const string RecursoYaAgregado = "El recurso ya fue agregado.";
    public const string DependenciaYaAgregada = "La dependencia ya fue agregada.";
    public const string TituloTareaVacio = "El título de la tarea no puede estar vacío o ser nulo.";
    public const string UsuarioNullEnAsignacion = "No se puede asignar una tarea a un usuario null.";
    public const string UsuarioNoAsignado = "El usuario no está asignado a la tarea.";
    public const string RecursoNullParaAgregar = "No se puede agregar un recurso null.";
    public const string RecursoNoNecesario = "El recurso no se encuentra dentro de los recursos necesarios.";
    public const string DependenciaNullEnTarea = "No se puede agregar una dependencia null.";

    public const string DependenciaNoExisteEnTarea =
        "La dependencia no se encuentra dentro de la lista de dependencias.";
    
    public const string ActualizarEntidadNoCoincidente =
        "No se puede actualizar la entidad por una distinta a la actual";
    
    public const string FechaInicioRangoMayorQueFin =
        "La fecha de inicio no puede ser mayor que la fecha de fin.";
    public const string RangoDeUsoNoPuedeSerCeroOMenos = "El rango de uso no puede ser cero o negativo.";
    public static string TareaNoPuedeSerNula = "Una tarea nula no puede hacer uso en el rango de uso.";
    public static string RecursoYaAgregadoATarea = "El recurso ya fue agregado a la tarea.";
    public const string RangoNoEncontrado ="No se encontró un rango con los datos especificados.";
    public const string CantidadMayorACero ="La cantidad debe ser mayor a cero.";

}