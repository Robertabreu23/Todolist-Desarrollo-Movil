# Todolist-Desarrollo-Movil con Xamarin/Sqlite/MAUI
#Hecho con dedicacion por Robert Abreu 23-0121




TodoApp
Aplicación de gestión de tareas que armé como proyecto para practicar desarrollo full-stack en .NET. Consta de dos partes que conversan entre sí: una API REST en el backend y una app móvil Android en el frontend.
¿Qué hace la app?
Permite llevar una lista de tareas pendientes, donde cada tarea puede tener:
Un nombre y una descripción opcional
Una prioridad (Baja, Normal, Alta o Urgente)
Una fecha y hora de vencimiento opcional
Un estado de completada o pendiente
Desde la app móvil podés crear, editar, eliminar tareas, marcarlas como completadas y verlas todas en una lista. Además, el backend tiene una funcionalidad interesante: si una tarea es urgente y le quedan menos de 2 horas para vencerse, el sistema lo detecta automáticamente y emite un aviso. Esto pasa en segundo plano, sin que el usuario tenga que hacer nada.
¿Cómo está organizado?
El proyecto está dividido en dos partes independientes que se comunican por HTTP:
TodoApp/
├── TodoApi/         → el backend (la API)
└── TodoAppClient/   → la app móvil que consume la API

La idea de tenerlos separados es que el backend no sabe nada del cliente, y el cliente sólo conoce las URLs y los formatos JSON que expone la API. Esto significa que más adelante podría conectarle otro cliente (una web, otra app, etc.) sin tocar el backend.
El backend — TodoApi
Hecho en ASP.NET Core 8 con Entity Framework para la base de datos y SQLite como motor de persistencia. Corre dentro de un contenedor Docker, lo que hace que sea fácil de arrancar en cualquier máquina sin instalar dependencias.
Lo que expone son seis endpoints bajo /api/tareas:
GET para listar todas o traer una por ID
POST para crear
PUT para editar
DELETE para eliminar
PATCH para alternar el estado completada/pendiente
Internamente está organizado por capas: controladores que reciben las peticiones, un repositorio que abstrae el acceso a datos, DTOs que definen los formatos de entrada y salida, y un middleware que centraliza el manejo de errores y los devuelve siempre con un formato consistente (ProblemDetails, el estándar RFC 7807).
La parte más interesante es el BackgroundService: un proceso que corre en paralelo cada minuto y revisa la base de datos. Cuando encuentra una tarea urgente que está por vencer, emite un aviso y la marca como ya notificada para no repetir. Si después el usuario cambia la prioridad o la fecha, el flag se resetea para que pueda volver a avisar.
Para correrlo:
cd TodoApi
docker compose up --build

Y se accede al Swagger en http://localhost:5000/swagger para probar los endpoints.
El cliente — TodoAppClient
Hecho en .NET MAUI apuntando a Android. Usa el patrón MVVM (Model-View-ViewModel) con CommunityToolkit.Mvvm para reducir el código repetitivo de bindings y comandos.
Tiene dos pantallas:
Lista de tareas, con pull-to-refresh, swipe para eliminar, tap para alternar completada, y un botón para crear una nueva.
Formulario, que sirve tanto para crear como editar, con campos para todos los atributos de la tarea.
Por dentro, el cliente está estructurado en:
Models: réplicas de los DTOs del backend
Services: un HttpClient tipado que llama a la API
ViewModels: la lógica de cada pantalla (cargar datos, manejar comandos, errores)
Views: el XAML con la UI
Una decisión consciente fue no compartir código entre backend y cliente — los DTOs están duplicados. Como sólo son tres, no justifica armar un proyecto compartido todavía. Si crecen, lo natural sería generar el cliente automáticamente desde el Swagger del backend.
Para correrlo, con un emulador Android abierto (yo usé Genymotion):
cd TodoAppClient
dotnet build -t:Run -f net8.0-android -c Debug

El diseño
La UI por ahora es funcional pero sin estilo definitivo. Prioricé que todo funcione end-to-end antes de pulir el diseño visual, que pienso rehacer después desde un diseño en Figma. Es la lógica de "primero que ande, después que se vea bien".
Lo que aprendí armándolo
Separación de capas y responsabilidades: cómo dividir un sistema en partes que se comunican por una interfaz clara (HTTP) en lugar de acoplarse.
Procesos en background: cómo usar BackgroundService para tareas periódicas sin bloquear las peticiones HTTP.
Manejo centralizado de errores: cómo un middleware puede capturar excepciones y devolver respuestas consistentes en toda la API.
MVVM en MAUI: cómo separar la lógica de la UI usando bindings y comandos.

Evidencias
<img width="1850" height="587" alt="Captura desde 2026-05-21 22-25-13" src="https://github.com/user-attachments/assets/77af3e03-f3b1-498c-ae6c-f63830371f8d" />
<img width="480" height="1009" alt="Captura desde 2026-05-21 22-23-14" src="https://github.com/user-attachments/assets/4ca731c7-52fe-439f-bd3b-b572bfcfca00" />
<img width="480" height="1009" alt="Captura desde 2026-05-21 22-22-56" src="https://github.com/user-attachments/assets/cd090cc9-c3ff-4534-8f4c-6eeed995583b" />
<img width="480" height="1009" alt="Captura desde 2026-05-21 22-22-39" src="https://github.com/user-attachments/assets/18bb7919-03bf-451d-ab90-6945a8dd5f02" />
<img width="480" height="1009" alt="Captura desde 2026-05-21 22-22-12" src="https://github.com/user-attachments/assets/3e24b412-c4f1-4fb3-a018-972be9cbbc31" />
<img width="480" height="1009" alt="Captura desde 2026-05-21 22-21-28" src="https://github.com/user-attachments/assets/290f95bd-6534-4d7e-b923-d499134c9cc1" />







