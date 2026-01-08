namespace Excepciones.MensajesError;

public static class MensajesErrorServicios
{
    public const string PermisoDenegado = "El usuario no tiene permisos necesarios para realizar esta acción.";
   
    public static string PermisoDenegadoPara(string accion) =>
        $"No tiene los permisos necesarios para {accion}.";
    public static string PermisoDenegadoPorTipo(string tipoUsuario)
        => $"El {tipoUsuario} no tiene permisos necesarios para realizar esta acción.";

    public const string ProyectoNoEncontrado = "El proyecto solicitado no existe.";
    public const string RecursoNoEncontrado = "El recurso solicitado no existe.";
    public const string UsuarioNoEncontrado = "El usuario solicitado no existe.";

    public const string ContrasenaIncorrecta = "La contraseña ingresada es incorrecta.";

    public const string PrimerAdminSistema = "No se puede eliminar al primer administrador del sistema.";

    public const string RecursoEnUso = "El recurso esta en uso.";

    public const string NoEsAdminDelProyecto = "Solo el administrador del proyecto puede realizar esta acción.";
    public const string UsuarioNoMiembroDelProyecto = "El usuario no es miembro del proyecto.";
    public const string UsuarioConTareas = "El usuario tiene tareas asignadas";
    public const string UsuarioMiembroDeProyecto = "No puede eliminar un usuario que es miembro de un proyecto.";
    public static string UsuarioNoAdminSistema(string accion) => $"No puede {accion}. El solicitante no es administrador de sistema.";
    public const string UsuarioNoAdminProyecto = "El usuario a desasignar no es administrador de proyectos.";
    public const string UsuarioAdministrandoProyecto = "El usuario está administrando un proyecto.";
    public const string UsuarioNoAdministraProyectos = "No se encontró un proyecto administrado por ese usuario.";
    public const string UsuarioNoAdministraProyectoOEsAdminSistema = "No se encontró un proyecto administrado por ese usuario, ni el usuario tiene permisos de administrador de sistema.";
    public const string UsuarioNoEsLider = "El ususario no es lider del proyecto.";

    public const string NombreRepetido = "Ya existe ese nombre. Debe elegir uno distinto.";
    public const string EmailRepetido = "Ya existe un usuario registrado con ese email. Debe elegir uno distinto.";
    
    public const string TareaConSucesoras = "No se puede eliminar la tarea porque tiene tareas sucesoras.";
    public const string TareaNoExistente = "La tarea no existe.";
    public const string GeneraCiclos =
        "No se puede  agregar la dependencia de la tarea ya que se generarían dependencias cíclicas.";
    public const string EstadoNoEditable =
        "No se puede cambiar manualmente a un estado distinto de 'En Proceso' o 'Completada'.";
    public const string RecursoNoAsignado = "El recurso no está asignado a la tarea.";
    public const string FechaInicioTarea =
        "La fecha de inicio de la tarea no puede ser anterior a la fecha de inicio del proyecto.";
    public const string GrafoConCiclos = "El grafo de tareas tiene dependencias cíclicas.";
    
    public static string ContrasenaMuyCorta(int largoMinimoContrasena) =>
        $"La contraseña debe tener al menos {largoMinimoContrasena} caracteres.";
    public const string ContrasenaSinMayuscula = "La contraseña debe incluir al menos una letra mayúscula (A-Z).";
    public const string ContrasenaSinMinuscula = "La contraseña debe incluir al menos una letra minúscula (a-z).";
    public const string ContrasenaSinNumero = "La contraseña debe incluir al menos un número (0-9).";
    public const string ContrasenaSinCaracterEspecial =
        "La contraseña debe incluir al menos un carácter especial (como @, #, $, etc.).";
    
    public const string UsuarioNoEsAdminNiLider = "El usuario no tiene permisos para realizar esta acción (debe ser administrador o líder del proyecto).";
    public const string ProyectoNoComenzado = "El proyecto no ha comenzado aún.";
    public const string PermisoParaCambiarDeEstado = "El usuario debe tener la tarea asignada o ser administrador del proyecto o ser lider del proyecto para realizar esta acción.";

    public const string TareaEnProceso = "No se puede modificar una tarea que está en proceso.";
    
    public const string FormatoNoSoportado = "El formato solicitado no está soportado por el sistema.";

    public const string CapacidadInsuficiente = "El recurso no tiene capacidad suficiente.";
    public const string ExisteConflicto = "Existe un conflicto en ese rango de fechas.";
    
    public const string TareaConRecursosAsignados = "No se puede modificar esta tarea porque tiene recursos asignados. Debe eliminar los recursos primero.";
    public const string NoPuedeComenzarConRecursosAntesDeFechaInicio = "No se puede comenzar una tarea con recursos antes de su fecha de inicio. Debe eliminar los recursos primero."; 
    public const string NoPuedeCompletarConRecursosAntesDeFechaFin= "No se puede completar una tarea antes de su fecha de fin si tiene recursos asignados.";
}