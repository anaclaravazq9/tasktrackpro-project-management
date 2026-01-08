# GUÍA DE INSTALACIÓN

### Requisitos
- Tener instalado **Docker Desktop**
- Tener instalado **DBeaver**
- Tener instalado **.NET Desktop Runtime**
- Tener los archivos del release **descargados y descomprimidos**

### Configurar el entorno con Docker
1. Abrir **Docker Desktop** y asegurarse de que esté corriendo correctamente.
2. Ingresar a la carpeta `release` (ya descomprimida) y abrir una terminal en ese directorio.
3. Ejecutar el siguiente comando:
 `docker compose up`

### Configurar la base de datos en DBeaver
1. Abrir _DBeaver_
2. Crear una nueva conexión de tipo SQL Server, con los siguientes datos:
    - Host: localhost
    - Port: 1433
    - Nombre de usuario: sa
    - Contraseña: Admin123$

    _Si DBeaver pide instalar drivers, aceptar y dejar que se descarguen._

3. Una vez conectado, crear una nueva base de datos:
    - Expandir el navegador de Bases de Datos en la columna izquierda.
    - Click derecho sobre **Databases** → **Crear Nuevo Database**
    - Ingresar nombre: **BDD**
4. Hacer click derecho sobre **BDD** → **Establecer por defecto**  _(Debería quedar en negrita)_.
5. Click derecho nuevamente sobre **BDD** → **Editor SQL** → **Nuevo script SQL**

### Crear la base de datos: dos opciones
**Opción 1: Base vacía (solo con el usuario administrador inicial)**
- Abrir el archivo `scriptCreacion.sql`. (dentro de la carpeta release) y copiar todo su contenido.
- Pegalo en el editor SQL de DBeaver y ejecutar (`Ctrl + A` y luego `Ctrl + Enter`).

**Opción 2: Base con datos de prueba**
- Seguir los pasos de la opción 1 para crear la base vacía.
- Borrar el contenido del script anterior y en su lugar, pegar el contenido del archivo `datosDePrueba.sql`.
- Ejecutar el nuevo script.

#### Levantar la aplicación
- Ingresar a la carpeta `publish`.
- Ejecutar el archivo `Interfaz.exe`.

Se abrirá una ventana de terminal que indicará en qué dirección se puede abrir la aplicación en tu navegador (por ejemplo, http://localhost:5000).

