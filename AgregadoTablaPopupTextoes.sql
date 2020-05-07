CREATE TABLE [dbo].[PopupTextoes] (
    [Id] [int] NOT NULL IDENTITY,
    [ShowPopup] [bit] NOT NULL,
    [Titulo] [nvarchar](max),
    [Contenido] [nvarchar](max),
    CONSTRAINT [PK_dbo.PopupTextoes] PRIMARY KEY ([Id])
)