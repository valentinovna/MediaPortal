CREATE TABLE [dbo].[FileSystem] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [UserId]        NVARCHAR (128) NOT NULL,
    [ParentId]      INT            NULL,
    [Name]          NVARCHAR (256) NOT NULL,
    [Size]          INT            NULL,
    [Type]          NVARCHAR (256) NOT NULL,
    [BlobLink]      NVARCHAR (256) NULL,
    [BlobThumbnail] NVARCHAR (256) NULL,
    [CreationDate]  DATETIME       NOT NULL,
    [UploadDate]    DATETIME       NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([ParentId]) REFERENCES [dbo].[FileSystem] ([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    UNIQUE NONCLUSTERED ([UserId] ASC, [ParentId] ASC, [Name] ASC)
);


GO
CREATE TRIGGER TR_FileSystem_DEL
ON FileSystem
FOR DELETE
AS 
begin
	delete fs
	from FileSystem fs left join (select Id from deleted) d on fs.Id = d.Id		
	where fs.ParentId = d.Id
end