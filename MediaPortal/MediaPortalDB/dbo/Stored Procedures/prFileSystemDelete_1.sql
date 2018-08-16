CREATE PROCEDURE [dbo].[prFileSystemDelete]
    (@Id int)
AS
BEGIN
  
WITH [CTE](Id, ParentId, BlobLink, BlobThumbnail)
AS (
        SELECT Id, ParentId, BlobLink, BlobThumbnail
        FROM FileSystem fs
        WHERE (fs.Id = @Id)
 
        UNION ALL
 
        SELECT fs.Id, fs.ParentId, fs.BlobLink, fs.BlobThumbnail
            FROM FileSystem fs INNER JOIN [CTE] p ON fs.ParentId = p.Id      
)


--DELETE FROM FileSystem
--WHERE Id IN(
--    SELECT t.Id
--    FROM [CTE] t)

SELECT t.Id, t.BlobLink, t.BlobThumbnail
FROM [CTE] t
	
OPTION ( MAXRECURSION 0 )

END