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