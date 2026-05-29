# Lista de endpoints: #

# Play #

| Metodo  | Ruta              | Nombre              |
|---------|-------------------|---------------------|
| Post    | /api/play         | Create              |
| Get     | /api/play         | GetAll              |
| Get     | /api/play/{id}    | GetById             |
| Put     | /api/play/{id}    | Update              |
| Delete  | /api/play/{id}    | Delete (SoftDelete) |
| Get     | /api/play/deleted | GetAllDeleted       |

# Seat #
<!-- El metodo create no debe ser utilizado ya que podría rompe las cosas,
    To Do: Elimimar de forma segura el create-->
| Metodo  | Ruta              | Nombre              |
|---------|-------------------|---------------------|
| Post    | /api/seat         | Create              |
| Get     | /api/seat         | GetAll              |
| Get     | /api/seat/{id}    | GetById             |
| Put     | /api/seat/{id}    | Update              |

# Performance #

| Metodo    | Ruta                     | Nombre               |
|-----------|--------------------------|----------------------|
| Post      | /api/performance         | Create               |
| Get       | /api/performance         | GetAll               |
| Get       | /api/performance/deleted | GetAllDeleted        |
| Get       | /api/performance/{id}    | GetById              |
| Put       | /api/performance/{id}    | Update               |
| Delete    | /api/play/{id}           | Delete (SoftDelete)  |

# Favorite #

| Metodo | Ruta              | Nombre | Descripción                                           |
|--------|-------------------|--------|-------------------------------------------------------|
| GET    | api/favorite      | GetAll | Lista todos los favoritos del usuario autenticado     |
| POST   | api/favorite      | Create | Crear conexión de un usuario a una obra en favorito   |
| DELETE | api/favorite/{id} | Delete | Soft Delete para la relación                          |

# Purchase #

| Método   | Ruta                      | Nombre       | Descripción                     |
|----------|---------------------------|--------------|---------------------------------|
| GET      | /api/purchase             | GetAll       | Todas las compras (admin)       |
| GET      | /api/purchase/my          | GetMine      | Compras del usuario autenticado |
| GET      | /api/purchase/{id}        | GetById      | Compra por ID                   |
| POST     | /api/purchase             | Create       | Crear compra                    |
| PATCH    | /api/purchase/{id}/status | UpdateStatus | Actualizar estado               |
| DELETE   | /api/purchase/{id}        | Delete       | Soft delete                     |

# Pqrs #

| Método | Ruta                   | Nombre  | Descripción                  |
|--------|------------------------|---------|------------------------------|
| GET    | /api/pqrs              | GetAll  | Todas las PQRS (admin)       |
| GET    | /api/pqrs/my           | GetMine | PQRS del usuario autenticado |
| GET    | /api/pqrs/{id}         | GetById | PQRS por ID                  |
| POST   | /api/pqrs              | Create  | Crear PQRS                   |
| PATCH  | /api/pqrs/{id}/respond | Respond | Responder (admin)            |
| DELETE | /api/pqrs/{id}         | Delete  | Soft delete                  |

# PerformanceSeat"

| Metodo | Ruta                                 | Nombre           | Descripción                           |
|--------|--------------------------------------|------------------|---------------------------------------|
| GET    | api/performanceseat                  | GetAll           | Obtener Todos los asientos            |
| GET    | api/performanceseat/{id}             | GetById          | Obtener asiento por Id                |
| GET    | api/performanceseat/performance/{id} | GetByPerformance | Obtener todos asientos de una función |
| PATCH  | api/performanceseat/{id}/status      | UpdateStatus     | Cambiar estado                        |
| PATCH  | api/performanceseat/{id}/scan        | Scan             | Registrar escaneo                     |