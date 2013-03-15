declare @a as int = (select COUNT(*) from Solicitudes)
if @a%20 = 0
begin
select @a/20,'exacto'
end
else
begin
select (@a/20)+1,'inexacto'
end

select 
select 

declare @pagina as int = 1
SELECT  ID
FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY ID ) AS RowNum, *
          FROM      Solicitudes
        ) AS RowConstrainedResult
WHERE   RowNum >= (20 * @pagina) - 19
    AND RowNum <= (20 * @pagina)
ORDER BY ID

select * from Solicitudes

exec ProcConsulta 

declare @a as int = (select COUNT(*) from Solicitudes)
	if @a%20 = 0
	begin
	select @a/20 as 'Paginas'
	end
	else
	begin
	select (@a/20)+1 as 'Paginas'
	end
	--SELECT  ID
--FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY ID ) AS RowNum, *
--          FROM      @tablaTemporal
--        ) AS RowConstrainedResult
--WHERE   RowNum >= (20 * @pagina) - 19
--    AND RowNum <= (20 * @pagina)
--ORDER BY ID