using Dominio;
using Excepciones;

namespace Tests.DominioTests;

[TestClass]
public class ProyectoTests
{
    private Proyecto _proyecto;
    private Usuario _admin;
    private List<Usuario> _miembros;

    [TestInitialize]
    public void AntesDeCadaTest()
    {
        _admin = CrearAdmin(1);
        _miembros = new List<Usuario>();
    }
    
    [TestMethod]
    public void Constructor_ConParametrosAsignadosCorrectamente()
    {
        string nombre = "Proyecto 1";
        string descripcion = "Descripción";
        DateTime fechaInicio = DateTime.Today.AddDays(1);

        _proyecto = new Proyecto(nombre, descripcion, fechaInicio, _admin, _miembros);

        Assert.AreEqual(nombre, _proyecto.Nombre);
        Assert.AreEqual(descripcion, _proyecto.Descripcion);
        Assert.AreEqual(_admin, _proyecto.Administrador);
        Assert.AreEqual(_miembros, _proyecto.Miembros);
        Assert.AreEqual(fechaInicio, _proyecto.FechaInicio);
        Assert.AreEqual(fechaInicio.AddDays(100000), _proyecto.FechaFinMasTemprana);
        Assert.AreEqual(_admin.CantidadProyectosAsignados, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDescripcionSupera400Caracteres()
    {
        string descripcion = new string('a', 401);
        DateTime fechaInicio = DateTime.Today.AddDays(1);

        _proyecto = new Proyecto("Proyecto", descripcion, fechaInicio, _admin, _miembros);
    }

    [TestMethod]
    public void Constructor_PermiteDescripcionConMenosDe400Caracteres()
    {
        string descripcion = new string('a', 399);
        DateTime fechaInicio = DateTime.Today.AddDays(1);

        _proyecto = new Proyecto("Proyecto", descripcion, fechaInicio, _admin, _miembros);

        Assert.AreEqual(descripcion, _proyecto.Descripcion);
    }

    [TestMethod]
    public void Constructor_PermiteDescripcionDeHasta400Caracteres()
    {
        string descripcion = new string('a', 400); // 400 caracteres exactos
        DateTime fechaInicio = DateTime.Today.AddDays(1);

        _proyecto = new Proyecto("Proyecto", descripcion, fechaInicio, _admin, _miembros);

        Assert.AreEqual(descripcion, _proyecto.Descripcion);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiNombreEsVacio()
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        _proyecto = new Proyecto("", "Descripción válida", fechaInicio, _admin, _miembros);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiNombreEsNull()
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        _proyecto = new Proyecto(null, "Descripción válida", fechaInicio, _admin, _miembros);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDescripcionEsVacia()
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        _proyecto = new Proyecto("Nombre válido", "", fechaInicio, _admin, _miembros);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDescripcionEsNull()
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        _proyecto = new Proyecto("Nombre válido", null, fechaInicio, _admin, _miembros);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiFechaInicioEsMenorQueHoy()
    {
        DateTime fechaInvalida = DateTime.Today.AddDays(-1);
        _proyecto = new Proyecto("Proyecto", "Descripción", fechaInvalida, _admin, _miembros);
    }

    [TestMethod]
    public void Constructor_AsignaCorrectamenteFechaInicio()
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        _proyecto = new Proyecto("Proyecto", "Descripción", fechaInicio, _admin, _miembros);

        Assert.AreEqual(fechaInicio, _proyecto.FechaInicio);
    }

    [TestMethod]
    public void Constructor_AsignaFechaFinMasTempranaSegunFechaInicio()
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        _proyecto = new Proyecto("Proyecto", "Descripción", fechaInicio, _admin, _miembros);

        Assert.AreEqual(fechaInicio.AddDays(100000), _proyecto.FechaFinMasTemprana);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiAdministradorEsNull()
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        _proyecto = new Proyecto("Nombre", "Descripción", fechaInicio, null, _miembros);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiMiembrosEsNull()
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        _proyecto = new Proyecto("Nombre", "Descripción", fechaInicio, _admin, null);
    }

    [TestMethod]
    public void AsignarID_DeberiaAsignarCorrectamenteElId()
    {
        Usuario admin = CrearAdmin(1);
        Proyecto proyecto = CrearProyectoCon(admin);
        proyecto.Id = 42;
        Assert.AreEqual(42, proyecto.Id);
    }

    [TestMethod]
    public void AgregarTarea_AgregarUnaTareaALaLista()
    {
        Proyecto proyecto = CrearProyectoCon(_admin);
        Tarea tarea = CrearTarea();

        proyecto.AgregarTarea(tarea);

        Assert.IsTrue(proyecto.Tareas.Contains(tarea));
        Assert.AreEqual(1, proyecto.Tareas.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AgregarTarea_LanzarExcepcionSiTareaEsNull()
    {
        _proyecto = CrearProyectoCon(_admin);
        Tarea tarea1 = null;
        _proyecto.AgregarTarea(tarea1);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AgregarTarea_LanzarExcepcionSiTareaYaEstaEnTareas()
    {
        Tarea tarea1 = CrearTarea();
        _proyecto = CrearProyectoCon(_admin);
        _proyecto.AgregarTarea(tarea1);
        _proyecto.AgregarTarea(tarea1);
    }

    [TestMethod]
    public void EliminarTarea_EliminaTareaDeLaLista()
    {
        Tarea tarea = CrearTarea(1);
        tarea.Id = 1;
        _proyecto = CrearProyectoCon(_admin, _miembros);
        _proyecto.AgregarTarea(tarea);
        _proyecto.EliminarTarea(1);

        Assert.IsFalse(_proyecto.Tareas.Any(t => t.Id == 1));
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarTarea_LanzaExcepcionSiTareaNoExiste()
    {
        Tarea tarea = CrearTarea(1);

        _proyecto = CrearProyectoCon(_admin);
        _proyecto.AgregarTarea(tarea);
        _proyecto.EliminarTarea(2); // ID que no existe
    }
    
    [TestMethod]
    public void AsignarMiembro_AgregarUsuarioALaListaDeMiembros()
    {
        _proyecto = CrearProyectoCon(_admin);

        Usuario nuevoMiembro = CrearMiembro(10);

        _proyecto.AsignarMiembro(nuevoMiembro);

        Assert.IsTrue(_proyecto.Miembros.Contains(nuevoMiembro));
        Assert.AreEqual(1, nuevoMiembro.CantidadProyectosAsignados);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AsignarMiembro_LanzarExcepcionSiUsuarioEsNull()
    {
        _proyecto = CrearProyectoCon(_admin);

        _proyecto.AsignarMiembro(null);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AsignarMiembro_LanzarExcepcionSiUsuarioYaEstaEnMiembros()
    {
        _proyecto = CrearProyectoCon(_admin);

        _proyecto.AsignarMiembro(_admin);
    }
    
    [TestMethod]
    public void EliminarMiembro_EliminaUsuarioCorrectamenteDeLaLista()
    {
        Usuario admin = CrearAdmin(1);
        Usuario miembro = CrearMiembro(2);
        _proyecto = CrearProyectoCon(admin);
        _proyecto.AsignarMiembro(miembro);
        _proyecto.EliminarMiembro(miembro.Id); // Elimino al miembro

        Assert.IsFalse(_proyecto.Miembros.Any(u => u.Id == 2));
        Assert.AreEqual(1, _proyecto.Miembros.Count);
        Assert.AreEqual(0, miembro.CantidadProyectosAsignados);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarMiembro_LanzaExcepcionSiElUsuarioNoExisteEnMiembros()
    {
        Usuario miembro = CrearMiembro(2);
        _proyecto = CrearProyectoCon(_admin);

        _proyecto.AsignarMiembro(miembro);
        _proyecto.EliminarMiembro(3); // ID que no existe
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarMiembro_LanzaExcepcionSiUsuarioEsAdministrador()
    {
        Usuario miembro = CrearMiembro(2);

        _proyecto = CrearProyectoCon(_admin);

        _proyecto.AsignarMiembro(miembro);
        _proyecto.EliminarMiembro(_admin.Id); // Intenta eliminar al admin
    }
    
    [TestMethod]
    public void EliminarMiembro_QueEsLider_DejaLiderEnNull()
    {
        Usuario lider = CrearMiembro(3);
        _miembros.Add(lider);
        Proyecto proyecto = CrearProyectoCon(_admin, _miembros);
        proyecto.AsignarLider(lider);

        proyecto.EliminarMiembro(lider.Id);

        Assert.IsNull(proyecto.Lider);
    }

    [TestMethod]
    public void EliminarMiembro_QueEsLider_ActualizaEsLiderFalse()
    {
        Usuario lider = CrearMiembro(3);
        _miembros.Add(lider);
        Proyecto proyecto = CrearProyectoCon(_admin, _miembros);
        proyecto.AsignarLider(lider);

        proyecto.EliminarMiembro(lider.Id);

        Assert.IsFalse(lider.EsLider);
    }
    
    [TestMethod]
    public void EsAdministrador_RetornaTrueSiUsuarioEsAdministrador()
    {
        _proyecto = CrearProyectoCon(_admin, _miembros);

        bool resultado = _proyecto.EsAdministrador(_admin);

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void EsAdministrador_RetornarFalseSiUsuarioNoEsAdministrador()
    {
        Usuario otro = CrearMiembro(2);
        _proyecto = CrearProyectoCon(_admin, _miembros);
        _proyecto.AsignarMiembro(otro);
        bool resultado = _proyecto.EsAdministrador(otro);
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void ModificarFechaInicio_ActualizaLaFechaOK()
    {
        Proyecto _proyecto = CrearProyectoCon(_admin, _miembros);
        DateTime nuevaFecha = new DateTime(2026, 5, 1);
        _proyecto.ModificarFechaInicio(nuevaFecha);

        Assert.AreEqual(nuevaFecha, _proyecto.FechaInicio);
    }


    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarFechaInicio_LanzaExcepcionSiFechaEsAnteriorAHoy()
    {
        _proyecto = CrearProyectoCon(_admin);

        DateTime fechaPasada = DateTime.Now.AddDays(-1);

        _proyecto.ModificarFechaInicio(fechaPasada);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarFechaInicio_LanzaExcepcionSiEsPosteriorALaFechaDeInicioDeUnaTarea()
    {
        Proyecto _proyecto = CrearProyectoCon(_admin, _miembros);
        Tarea tarea = CrearTarea(1);
        _proyecto.AgregarTarea(tarea);

        _proyecto.ModificarFechaInicio(new DateTime(2600, 1, 1));
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarFechaInicio_LanzaExcepcionSiEsIgualALaFechaFinMasTemprana()
    {
        _proyecto = CrearProyectoCon(_admin);
        DateTime fecha = DateTime.Today.AddDays(3);

        _proyecto.FechaFinMasTemprana = fecha;

        _proyecto.ModificarFechaInicio(fecha);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarFechaInicio_LanzaExcepcionSiEsMayorALaFechaFinMasTemprana()
    {
        _proyecto = CrearProyectoCon(_admin);
        DateTime fechaFin = DateTime.Today.AddDays(5);
        _proyecto.FechaFinMasTemprana = fechaFin;

        DateTime fechaInicioInvalida = fechaFin.AddDays(1);
        _proyecto.ModificarFechaInicio(fechaInicioInvalida);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarFechaFinMasTemprana_LanzaExcepcionSiEsAnteriorALaFechaInicio()
    {
        _proyecto = CrearProyectoCon(_admin);
        DateTime nuevaFechaFin = _proyecto.FechaInicio.AddDays(-1);
        _proyecto.ModificarFechaFinMasTemprana(nuevaFechaFin);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarFechaFinMasTemprana_LanzaExcepcionSiEsMenorALaFechaFinDeUnaTarea()
    {
        _proyecto = CrearProyectoCon(_admin);

        Tarea tarea = CrearTarea(1);

        _proyecto.AgregarTarea(tarea);

        _proyecto.ModificarFechaFinMasTemprana(DateTime.Today.AddDays(1));
    }

    [TestMethod]
    public void ModificarFechaFinMasTemprana_ActualizaCorrectamente()
    {
        _proyecto = CrearProyectoCon(_admin);
        DateTime fecha = _proyecto.FechaInicio.AddDays(10);

        _proyecto.ModificarFechaFinMasTemprana(fecha);

        Assert.AreEqual(fecha, _proyecto.FechaFinMasTemprana);
    }
    

    [TestMethod]
    public void ModificarNombre_DeberiaActualizarElNombre()
    {
        _proyecto = CrearProyectoCon(_admin, _miembros);

        _proyecto.ModificarNombre("nombre nuevo");

        Assert.AreEqual("nombre nuevo", _proyecto.Nombre);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarNombre_LanzaExcepcionSiNombreEsNull()
    {
        _proyecto = CrearProyectoCon(_admin);

        _proyecto.ModificarNombre(null);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarNombre_LanzaExcepcionSiNombreEsVacio()
    {
        _proyecto = CrearProyectoCon(_admin);

        _proyecto.ModificarNombre("");
    }
    
    [TestMethod]
    public void ModificarDescripcion_ActualizaLaDescripcion()
    {
        _proyecto = CrearProyectoCon(_admin);

        _proyecto.ModificarDescripcion("Descripcion nueva");

        Assert.AreEqual("Descripcion nueva", _proyecto.Descripcion);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarDescripcion_LanzaExcepcionSiDescripcionEsNull()
    {
        _proyecto = CrearProyectoCon(_admin);

        _proyecto.ModificarDescripcion(null);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarDescripcion_LanzaExcepcionSiDescripcionEsVacia()
    {
        _proyecto = CrearProyectoCon(_admin);

        _proyecto.ModificarDescripcion("");
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarDescripcion_LanzaExcepcionSiDescripcionSupera400Caracteres()
    {
        _proyecto = CrearProyectoCon(_admin);

        string descripcionLarga = new string('a', 401); // 401 caracteres

        _proyecto.ModificarDescripcion(descripcionLarga);
    }

    [TestMethod]
    public void AsignarNuevoAdministrador_CambiaElAdministradorDelProyecto()
    {
        Usuario nuevoAdmin = CrearMiembro(2);

        _proyecto = CrearProyectoCon(_admin);

        _proyecto.AsignarMiembro(nuevoAdmin);
        _proyecto.AsignarNuevoAdministrador(nuevoAdmin);

        Assert.AreEqual(nuevoAdmin, _proyecto.Administrador);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AsignarNuevoAdministrador_LanzaExcepcionSiIdNoCorrespondeAMiembro()
    {
        Usuario miembro = CrearMiembro(2);

        _proyecto = CrearProyectoCon(_admin);

        _proyecto.AsignarNuevoAdministrador(miembro);
    }


    [TestMethod]
    public void EsMiembro_PorId_DevuelveTrueSiUsuarioPertenece()
    {
        Usuario miembro = CrearMiembro(2);
        Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { miembro });

        bool resultado = proyecto.EsMiembro(2);

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void EsMiembro_PorId_DevuelveFalseSiUsuarioNoPertenece()
    {
        Proyecto proyecto = CrearProyectoCon(_admin);
        Assert.IsFalse(proyecto.EsMiembro(100));
    }

    [TestMethod]
    public void EsMiembro_PorObjeto_DevuelveTrueSiUsuarioPertenece()
    {
        Usuario miembro = CrearMiembro(2);
        Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { miembro });

        Assert.IsTrue(proyecto.EsMiembro(miembro.Id));
    }

    [TestMethod]
    public void EsMiembro_PorObjeto_DevuelveFalseSiUsuarioNoPertenece()
    {
        Usuario otro = CrearMiembro(2);
        Usuario miembro = CrearMiembro(3);
        Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { miembro });

        Assert.IsFalse(proyecto.EsMiembro(otro.Id));
    }
    
    [TestMethod]
    public void TieneTareas_DaTrueSiHayTareas()
    {
        _proyecto = CrearProyectoCon(_admin);
        Tarea tarea = CrearTarea(1);
        _proyecto.AgregarTarea(tarea);

        Assert.IsTrue(_proyecto.TieneTareas());
    }

    [TestMethod]
    public void TieneTareas_DaFalseSiNoHayTareas()
    {
        _proyecto = CrearProyectoCon(_admin);

        Assert.IsFalse(_proyecto.TieneTareas());
    }
    
    [TestMethod]
    public void SeActualizaNombreCorrectamente()
    {
        _proyecto = CrearProyectoCon(_admin);
        Proyecto nuevoProyecto = new Proyecto("Proyecto nuevo", "Descripción", DateTime.Today, _admin, new List<Usuario>());

        _proyecto.Actualizar(nuevoProyecto);
        Assert.AreEqual("Proyecto nuevo", _proyecto.Nombre);
    }
    
    [TestMethod]
    public void SeActualizaDescripcionCorrectamente()
    {
        _proyecto = CrearProyectoCon(_admin);
        Proyecto nuevoProyecto = new Proyecto("Proyecto", "Descripción nueva", DateTime.Today, _admin, new List<Usuario>());

        _proyecto.Actualizar(nuevoProyecto);
        Assert.AreEqual("Descripción nueva", _proyecto.Descripcion);
    }
    
    [TestMethod]
    public void SeActualizaFechaInicioCorrectamente()
    {
        _proyecto = CrearProyectoCon(_admin);
        DateTime nuevaFecha = DateTime.Today.AddDays(5);
        Proyecto nuevoProyecto = new Proyecto("Proyecto", "Descripción", nuevaFecha, _admin, new List<Usuario>());

        _proyecto.Actualizar(nuevoProyecto);
        Assert.AreEqual(nuevaFecha, _proyecto.FechaInicio);
    }
    
    [TestMethod]
    public void SeActualizaFechaFinCorrectamente()
    {
        _proyecto = CrearProyectoCon(_admin);
        DateTime nuevaFechaFin = DateTime.Today.AddDays(10);
        Proyecto nuevoProyecto = new Proyecto("Proyecto", "Descripción", DateTime.Today, _admin, new List<Usuario>())
        {
            FechaFinMasTemprana = nuevaFechaFin
        };

        _proyecto.Actualizar(nuevoProyecto);
        Assert.AreEqual(nuevaFechaFin, _proyecto.FechaFinMasTemprana);
    }

    [ExpectedException(typeof(ExcepcionProyecto))]
    [TestMethod]
    public void Actualizar_LanzaExcepcionSiElProyectoNoEsElMismo()
    {
        _proyecto = CrearProyectoCon(_admin);
        Proyecto proyectoDiferente = new Proyecto("Otro Proyecto", "Descripción", DateTime.Today, _admin, new List<Usuario>());
        proyectoDiferente.Id = 999; // Id diferente para simular un proyecto distinto

        _proyecto.Actualizar(proyectoDiferente);
    }
    
    [TestMethod]
    public void Equals_RetornaTrueSiLosIdsNoSonIguales()
    {
        Proyecto proyecto1 = CrearProyectoCon(_admin);
        bool sonIguales = proyecto1.Equals(proyecto1);
        Assert.IsTrue(sonIguales);
    }

    [TestMethod]
    public void Equals_RetornaFalseSiLosIdsNoSonIguales()
    {
        Proyecto proyecto1 = CrearProyectoCon(_admin);
        Usuario otroAdmin = CrearAdmin(2);
        Proyecto proyecto2 = CrearProyectoCon(otroAdmin);
        proyecto2.Id = 1;
        bool sonIguales = proyecto1.Equals(proyecto2);
        Assert.IsFalse(sonIguales);
    }

    [TestMethod]
    public void Equals_RetornaFalseSiUnObjetoEsNull()
    {
        Proyecto proyecto1 = CrearProyectoCon(_admin);
        bool sonIguales = proyecto1.Equals(null);
        Assert.IsFalse(sonIguales);
    }

    [TestMethod]
    public void Equals_RetornaFalseSiUnObjetoNoEsProyecto()
    {
        Proyecto proyecto1 = CrearProyectoCon(_admin);
        int otro = 0;
        bool sonIguales = proyecto1.Equals(otro);
        Assert.IsFalse(sonIguales);
    }

    [TestMethod]
    public void GetHashCode_EsDistintoEnProyectosDistintos()
    {
        Proyecto proyecto1 = CrearProyectoCon(_admin);
        Usuario admin2 = CrearAdmin(2);
        Proyecto proyecto2 = CrearProyectoCon(admin2);
        //ambos tienen mismo id ya que no hay un gestor que maneje ids
        Usuario admin3 = CrearAdmin(3);
        Proyecto proyecto3 = CrearProyectoCon(admin3);
        proyecto3.Id =
            3; // proyecto con id distinto a los otros 2 (se hardcodea en vez de llamar al gestor por simplicidad)
        Assert.AreEqual(proyecto1.GetHashCode(), proyecto2.GetHashCode());
        Assert.AreNotEqual(proyecto3.GetHashCode(), proyecto1.GetHashCode());
    }

    private Usuario CrearAdmin(int id)
    {
        Usuario admin = new Usuario("Juan", "Perez", new DateTime(1999, 2, 2), "unemail@gmail.com", "Contrase#a3");
        admin.EsAdministradorProyecto = true;
        admin.Id = id; // Se hardcodea pero en realidad lo gestiona el repo de usuarios
        return admin;
    }

    private Usuario CrearMiembro(int id)
    {
        Usuario miembro = new Usuario("Mateo", "Perez", new DateTime(1999, 2, 2), "unemail@gmail.com", "Contrase#a3");
        miembro.Id = id; // Se hardcodea pero en realidad lo gestiona el repo de usuarios
        return miembro;
    }

    private Proyecto CrearProyectoCon(Usuario admin, List<Usuario> miembros = null)
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        miembros ??= new List<Usuario>();
        return new Proyecto("Proyecto", "Descripción", fechaInicio, admin, miembros);
    }

    private Tarea CrearTarea(int id = 1, DateTime? inicio = null)
    {
        Tarea tarea = new Tarea("titulo", "descripcion", 5, DateTime.Today)
        {
            Id = id,
        };
        tarea.ModificarFechaInicioMasTemprana(inicio ?? DateTime.Today);
        return tarea;
    }
    
    [TestMethod]
    public void ConstructorSinParametros_CreaInstanciaCorrectamente()
    {
        var proyecto = new Proyecto();
        Assert.IsNotNull(proyecto);
    }
    
    [TestMethod]
    public void Proyecto_SeCreaSinLiderPorDefecto()
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        Proyecto proyecto = new Proyecto("Proyecto", "Descripcion", fechaInicio, _admin, _miembros);

        Assert.IsNull(proyecto.Lider);
    }
    
    [TestMethod]
    public void AsignarLider_AsignaCorrectamente()
    {
        Usuario miembro = CrearMiembro(2);
        _miembros.Add(miembro);
        Proyecto proyecto = CrearProyectoCon(_admin, _miembros);

        proyecto.AsignarLider(miembro);

        Assert.AreEqual(miembro, proyecto.Lider);
        Assert.IsTrue(miembro.EsLider);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AsignarLider_LanzaExcepcionSiUsuarioNoEsMiembro()
    {
        Usuario noMiembro = CrearMiembro(99);
        Proyecto proyecto = CrearProyectoCon(_admin, _miembros);

        proyecto.AsignarLider(noMiembro);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AsignarLider_LanzaExcepcionSiUsuarioEsNull()
    {
        Proyecto proyecto = CrearProyectoCon(_admin, _miembros);
        proyecto.AsignarLider(null);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AsignarLider_LanzaExcepcionSiUsuarioYaEsLider()
    {
        Usuario miembro = CrearMiembro(2);
        _miembros.Add(miembro);
        Proyecto proyecto = CrearProyectoCon(_admin, _miembros);

        proyecto.AsignarLider(miembro);
        proyecto.AsignarLider(miembro); 
    }
    
    [TestMethod]
    public void AsignarLider_CambiaLiderYActualizaRoles()
    {
        Usuario miembro1 = CrearMiembro(2);
        Usuario miembro2 = CrearMiembro(3);
        _miembros.AddRange(new[] { miembro1, miembro2 });

        Proyecto proyecto = CrearProyectoCon(_admin, _miembros);
        proyecto.AsignarLider(miembro1); // primer líder
        proyecto.AsignarLider(miembro2); // cambio

        Assert.AreEqual(miembro2, proyecto.Lider);
        Assert.IsTrue(miembro2.EsLider);
        Assert.IsFalse(miembro1.EsLider); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void DesasignarLider_LanzaExcepcionSiUsuarioNoEsLider()
    {
        Usuario miembro = CrearMiembro(2);
        _miembros.Add(miembro);
        Proyecto proyecto = CrearProyectoCon(_admin, _miembros);

        proyecto.DesasignarLider(miembro); 
    }
    
    [TestMethod]
    public void DesasignarLider_ConContadorEnUnoRemueveRolYLider()
    {
        Usuario miembro = CrearMiembro(2);
        _miembros.Add(miembro);
    
        Proyecto proyecto = CrearProyectoCon(_admin, _miembros);
        proyecto.AsignarLider(miembro);

        proyecto.DesasignarLider(miembro);

        Assert.IsFalse(miembro.EsLider);
        Assert.AreEqual(0, miembro.CantidadProyectosLiderando);
        Assert.IsNull(proyecto.Lider);
    }
    
    [TestMethod]
    public void DesasignarLider_ConContadorMayorA1MantieneRolYLiderNull()
    {
        Usuario miembro = CrearMiembro(2);
        miembro.AsignarRolLider();
        miembro.CantidadProyectosLiderando = 1;
        _miembros.Add(miembro);

        Proyecto proyecto = CrearProyectoCon(_admin, _miembros);
        proyecto.AsignarLider(miembro);

        proyecto.DesasignarLider(miembro);

        Assert.IsTrue(miembro.EsLider);
        Assert.AreEqual(1, miembro.CantidadProyectosLiderando);
        Assert.IsNull(proyecto.Lider);
    }
    
    [TestMethod]
    public void EsLider_DevuelveTrueSiUsuarioEsElLider()
    {
        Usuario miembro = CrearMiembro(2);
        _miembros.Add(miembro);
        Proyecto proyecto = CrearProyectoCon(_admin, _miembros);
        proyecto.AsignarLider(miembro);

        bool resultado = proyecto.EsLider(miembro);

        Assert.IsTrue(resultado);
    }
    
    [TestMethod]
    public void EsLider_DevuelveFalseSiUsuarioNoEsElLider()
    {
        Usuario miembro1 = CrearMiembro(2);
        Usuario miembro2 = CrearMiembro(3);
        _miembros.AddRange(new[] { miembro1, miembro2 });

        Proyecto proyecto = CrearProyectoCon(_admin, _miembros);
        proyecto.AsignarLider(miembro1);

        bool resultado = proyecto.EsLider(miembro2);

        Assert.IsFalse(resultado);
    }
}