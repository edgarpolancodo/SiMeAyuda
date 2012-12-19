--Script para cambiar rol a usuario
--REQUERIMIENTO: Usuario existente y rol existente

--Escriba nombre de usuario dentro de las comillas 
declare @nombreusuario as varchar(50) = ''
--Escriba numero de rol dentro de comillas
--Guia: Solicitante=1, Tecnico=2, Supervisor=3
declare @rol as int = ''

--Proceda con ejecutar el script presionando F5 o dando clic al boton Execute









--*******************No tocar siguiente parte del codigo********************************************************
if exists(select * from Usuarios where NombreUsuario=@nombreusuario) and exists(select * from Roles where ID=@rol)
begin
update Usuarios set RolID = @rol where NombreUsuario=@nombreusuario
select 'Actualizado'
end
else
begin
select 'Ese nombre de usuario o rol no existe'
end
--**************************************************************************************************************