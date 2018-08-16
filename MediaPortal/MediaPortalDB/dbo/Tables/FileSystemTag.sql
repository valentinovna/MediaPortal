CREATE TABLE [dbo].[FileSystemTag] (
    [FileSystemId] INT NOT NULL,
    [TagId]        INT NOT NULL,
    PRIMARY KEY CLUSTERED ([FileSystemId] ASC, [TagId] ASC),
    FOREIGN KEY ([FileSystemId]) REFERENCES [dbo].[FileSystem] ([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([TagId]) REFERENCES [dbo].[Tag] ([Id]) ON DELETE CASCADE
);

