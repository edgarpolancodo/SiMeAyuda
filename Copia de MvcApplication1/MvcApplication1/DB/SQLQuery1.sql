
select s.ID, s.FechaCreacion, s.UltimaModificacion, s.Descripcion, s.Solucion, u.Nombre, s.Prioridad, c.Nombre, (select Nombre from Usuarios where ID =s.UsuarioTecnicoID)
from Solicitudes s, Usuarios u, Categorias c
where s.UsuarioCreadorID = u.ID and c.ID = s.CategoriaID


SELECT     s.ID, s.FechaCreacion, s.UltimaModificacion, s.Descripcion, s.Solucion, u.Nombre, s.Prioridad, c.Nombre AS Expr1,
                          (SELECT     Nombre
                            FROM          Usuarios
                            WHERE      (ID = s.UsuarioTecnicoID)) AS Expr2
FROM         Solicitudes AS s INNER JOIN
                      Usuarios AS u ON s.UsuarioCreadorID = u.ID INNER JOIN
                      Categorias AS c ON s.CategoriaID = c.ID