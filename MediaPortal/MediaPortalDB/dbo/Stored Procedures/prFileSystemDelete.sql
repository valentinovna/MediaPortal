CREATE PROCEDURE [dbo].[prFileSystemDelete]
    (@Id int)
AS
BEGIN
  
WITH [CTE](Id, ParentId)
AS (
        SELECT Id, ParentId
        FROM FileSystem fs
        WHERE (fs.Id = @Id)
 
        UNION ALL
 
        SELECT fs.Id, fs.ParentId
            FROM FileSystem fs INNER JOIN [CTE] p ON fs.ParentId = p.Id      
)
 
DELETE FROM FileSystem
WHERE Id IN(
    SELECT TOP 100 PERCENT t.Id
    FROM [CTE] t)
 
OPTION ( MAXRECURSION 0 )
END