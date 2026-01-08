using Dominio;

namespace Servicios.Notificaciones;

public static class MensajesNotificacion
{
    public static string FechaFinMasTempranaActualizada(string nombreProyecto, DateTime nuevaFecha) =>
        $"Se cambió la fecha de fin más temprana del proyecto '{nombreProyecto}' a '{nuevaFecha:dd/MM/yyyy}'.";

    public static string ProyectoCreado(string nombre) =>
        $"Se creó el proyecto '{nombre}'.";

    public static string ProyectoEliminado(string nombre) =>
        $"Se eliminó el proyecto '{nombre}'.";

    public static string NombreProyectoModificado(string anterior, string nuevo) =>
        $"Se cambió el nombre del proyecto '{anterior}' a '{nuevo}'.";

    public static string DescripcionProyectoModificada(string nombre, string nuevaDescripcion) =>
        $"Se cambió la descripción del proyecto '{nombre}' a '{nuevaDescripcion}'.";

    public static string FechaInicioProyectoModificada(string nombre, DateTime nuevaFecha) =>
        $"Se cambió la fecha de inicio del proyecto '{nombre}' a '{nuevaFecha:dd/MM/yyyy}'.";

    public static string AdministradorProyectoModificado(string nombreProyecto, string nuevoAdmin) =>
        $"Se cambió el administrador del proyecto '{nombreProyecto}'. El nuevo administrador es '{nuevoAdmin}'.";
    
    public static string LiderAsignado(string nombreProyecto, string nuevoLider) =>
         $"Se asignó a {nuevoLider} como líder del proyecto {nombreProyecto}.";

    public static string MiembroAgregado(string nombreProyecto, int idMiembro) =>
        $"Se agregó a un nuevo miembro (id {idMiembro}) al proyecto '{nombreProyecto}'.";

    public static string MiembroEliminado(string nombreProyecto, int idMiembro) =>
        $"Se eliminó al miembro (id {idMiembro}) del proyecto '{nombreProyecto}'.";


    public static string RecursoEliminado(string nombre, string tipo, string descripcion) =>
        $"Se eliminó el recurso {nombre} de tipo {tipo} - {descripcion}";

    public static string RecursoModificado(string nombreAnterior, string nuevosValores) =>
        $"El recurso '{nombreAnterior}' ha sido modificado. Nuevos valores: {nuevosValores}";


    public static string TareaAgregada(int idTarea, string nombreProyecto) =>
        $"Se agregó la tarea (id {idTarea}) al proyecto '{nombreProyecto}'.";

    public static string TareaEliminada(int idTarea, string nombreProyecto) =>
        $"Se eliminó la tarea (id {idTarea}) del proyecto '{nombreProyecto}'.";

    public static string CampoTareaModificado(string campo, int idTarea, string nombreProyecto) =>
        $"Se cambió el {campo} de la tarea (id {idTarea}) del proyecto '{nombreProyecto}'.";

    public static string EstadoTareaModificado(int idTarea, string nombreProyecto, EstadoTarea nuevoEstado) =>
        $"Se cambió el estado de la tarea (id {idTarea}) del proyecto '{nombreProyecto}' a {nuevoEstado}.";

    public static string DependenciaAgregada(int idTarea, string nombreProyecto, string tipo, int idDependencia) =>
        $"Se agregó una dependencia a la tarea id {idTarea} del proyecto '{nombreProyecto}' del tipo {tipo} con la tarea id {idDependencia}.";

    public static string DependenciaEliminada(int id1, int id2, string nombreProyecto) =>
        $"Se eliminó la dependencia de la tarea id {id1} con la tarea id {id2} del proyecto '{nombreProyecto}'.";

    public static string CampoTareaAgregado(string campo, int idTarea, string nombreProyecto) =>
        $"Se agregó el {campo} de la tarea (id {idTarea}) del proyecto '{nombreProyecto}'.";

    public static string CampoTareaEliminado(string campo, int idTarea, string nombreProyecto) =>
        $"Se eliminó el {campo} de la tarea (id {idTarea}) del proyecto '{nombreProyecto}'.";
    
    public static string ContrasenaReiniciada(string contrasena) =>
        $"Se reinició su contraseña. La nueva contraseña es {contrasena}";

    public static string ContrasenaModificada(string nuevaContrasena) =>
        $"Se modificó su contraseña. La nueva contraseña es {nuevaContrasena}";

    public static string UsuarioCreado(string nombre, string apellido) =>
        $"Se creó un nuevo usuario: {nombre} {apellido}";

    public static string UsuarioEliminado(string nombre, string apellido) =>
        $"Se eliminó un nuevo usuario. Nombre: {nombre}, Apellido: {apellido}";
    
    public static string LiderDesasignado(string nombre, string apellido) =>
        $"Se eliminó el líder. Nombre: {nombre}, Apellido: {apellido}";
}