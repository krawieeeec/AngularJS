CREATE TABLE [dbo].[Actor] (
    [Id] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](max),
    [Film_Id] [int],
    CONSTRAINT [PK_dbo.Actor] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_Film_Id] ON [dbo].[Actor]([Film_Id])
CREATE TABLE [dbo].[Film] (
    [Id] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](max),
    [OriginalName] [nvarchar](max),
    [ReleaseDate] [nvarchar](max),
    [Genre] [nvarchar](max),
    [Description] [nvarchar](max),
    [Cover] [nvarchar](max),
    [FilmwebUrl] [nvarchar](max),
    CONSTRAINT [PK_dbo.Film] PRIMARY KEY ([Id])
)
CREATE TABLE [dbo].[Director] (
    [Id] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](max),
    [Film_Id] [int],
    CONSTRAINT [PK_dbo.Director] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_Film_Id] ON [dbo].[Director]([Film_Id])
CREATE TABLE [dbo].[FilmTag] (
    [Id] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](max),
    [Film_Id] [int],
    CONSTRAINT [PK_dbo.FilmTag] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_Film_Id] ON [dbo].[FilmTag]([Film_Id])
CREATE TABLE [dbo].[List] (
    [Id] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](max),
    [User_Id] [nvarchar](128),
    CONSTRAINT [PK_dbo.List] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_User_Id] ON [dbo].[List]([User_Id])
CREATE TABLE [dbo].[AspNetUsers] (
    [Id] [nvarchar](128) NOT NULL,
    [UserName] [nvarchar](256) NOT NULL,
    [Email] [nvarchar](256),
    [EmailConfirmed] [bit] NOT NULL,
    [PasswordHash] [nvarchar](max),
    [SecurityStamp] [nvarchar](max),
    [PhoneNumber] [nvarchar](max),
    [PhoneNumberConfirmed] [bit] NOT NULL,
    [TwoFactorEnabled] [bit] NOT NULL,
    [LockoutEndDateUtc] [datetime],
    [LockoutEnabled] [bit] NOT NULL,
    [AccessFailedCount] [int] NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY ([Id])
)
CREATE UNIQUE INDEX [UserNameIndex] ON [dbo].[AspNetUsers]([UserName])
CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id] [int] NOT NULL IDENTITY,
    [UserId] [nvarchar](128) NOT NULL,
    [ClaimType] [nvarchar](max),
    [ClaimValue] [nvarchar](max),
    CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_UserId] ON [dbo].[AspNetUserClaims]([UserId])
CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider] [nvarchar](128) NOT NULL,
    [ProviderKey] [nvarchar](128) NOT NULL,
    [UserId] [nvarchar](128) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey], [UserId])
)
CREATE INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]([UserId])
CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] [nvarchar](128) NOT NULL,
    [RoleId] [nvarchar](128) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId])
)
CREATE INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]([UserId])
CREATE INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]([RoleId])
CREATE TABLE [dbo].[AspNetRoles] (
    [Id] [nvarchar](128) NOT NULL,
    [Name] [nvarchar](256) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY ([Id])
)
CREATE UNIQUE INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]([Name])
CREATE TABLE [dbo].[ListFilm] (
    [List_Id] [int] NOT NULL,
    [Film_Id] [int] NOT NULL,
    CONSTRAINT [PK_dbo.ListFilm] PRIMARY KEY ([List_Id], [Film_Id])
)
CREATE INDEX [IX_List_Id] ON [dbo].[ListFilm]([List_Id])
CREATE INDEX [IX_Film_Id] ON [dbo].[ListFilm]([Film_Id])
ALTER TABLE [dbo].[Actor] ADD CONSTRAINT [FK_dbo.Actor_dbo.Film_Film_Id] FOREIGN KEY ([Film_Id]) REFERENCES [dbo].[Film] ([Id])
ALTER TABLE [dbo].[Director] ADD CONSTRAINT [FK_dbo.Director_dbo.Film_Film_Id] FOREIGN KEY ([Film_Id]) REFERENCES [dbo].[Film] ([Id])
ALTER TABLE [dbo].[FilmTag] ADD CONSTRAINT [FK_dbo.FilmTag_dbo.Film_Film_Id] FOREIGN KEY ([Film_Id]) REFERENCES [dbo].[Film] ([Id])
ALTER TABLE [dbo].[List] ADD CONSTRAINT [FK_dbo.List_dbo.AspNetUsers_User_Id] FOREIGN KEY ([User_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])
ALTER TABLE [dbo].[AspNetUserClaims] ADD CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[AspNetUserLogins] ADD CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[AspNetUserRoles] ADD CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[AspNetUserRoles] ADD CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[ListFilm] ADD CONSTRAINT [FK_dbo.ListFilm_dbo.List_List_Id] FOREIGN KEY ([List_Id]) REFERENCES [dbo].[List] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[ListFilm] ADD CONSTRAINT [FK_dbo.ListFilm_dbo.Film_Film_Id] FOREIGN KEY ([Film_Id]) REFERENCES [dbo].[Film] ([Id]) ON DELETE CASCADE

CREATE TABLE [dbo].[UserFavourite] (
    [Id] [int] NOT NULL IDENTITY,
    [User_Id] [nvarchar](128) NOT NULL,
	[ListId] [int] NOT NULL DEFAULT 0,
    CONSTRAINT [PK_dbo.UserFavourite] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_User_Id] ON [dbo].[UserFavourite]([User_Id])
ALTER TABLE [dbo].[UserFavourite] ADD CONSTRAINT [FK_dbo.UserFavourite_dbo.AspNetUsers_User_Id] FOREIGN KEY ([User_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])

IF object_id(N'[dbo].[FK_dbo.UserFavourite_dbo.AspNetUsers_User_Id]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[UserFavourite] DROP CONSTRAINT [FK_dbo.UserFavourite_dbo.AspNetUsers_User_Id]
ALTER TABLE [dbo].[UserFavourite] ADD CONSTRAINT [FK_dbo.UserFavourite_dbo.AspNetUsers_User_Id] FOREIGN KEY ([User_Id]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE

CREATE TABLE [dbo].[Rating] (
    [Id] [int] NOT NULL IDENTITY,
    [Rate] [int] NOT NULL,
    [List_Id] [int] NOT NULL,
    [User_Id] [nvarchar](128) NOT NULL,
    CONSTRAINT [PK_dbo.Rating] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_List_Id] ON [dbo].[Rating]([List_Id])
CREATE INDEX [IX_User_Id] ON [dbo].[Rating]([User_Id])
ALTER TABLE [dbo].[List] ADD [Likes] [int] NOT NULL DEFAULT 0
ALTER TABLE [dbo].[List] ADD [Dislikes] [int] NOT NULL DEFAULT 0
ALTER TABLE [dbo].[Rating] ADD CONSTRAINT [FK_dbo.Rating_dbo.List_List_Id] FOREIGN KEY ([List_Id]) REFERENCES [dbo].[List] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[Rating] ADD CONSTRAINT [FK_dbo.Rating_dbo.AspNetUsers_User_Id] FOREIGN KEY ([User_Id]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE

CREATE TABLE [dbo].[FilmVote] (
    [Id] [int] NOT NULL IDENTITY,
    [Film_Id] [int] NOT NULL,
    [List_Id] [int] NOT NULL,
    [User_Id] [nvarchar](128) NOT NULL,
    CONSTRAINT [PK_dbo.FilmVote] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_Film_Id] ON [dbo].[FilmVote]([Film_Id])
CREATE INDEX [IX_List_Id] ON [dbo].[FilmVote]([List_Id])
CREATE INDEX [IX_User_Id] ON [dbo].[FilmVote]([User_Id])
ALTER TABLE [dbo].[FilmVote] ADD CONSTRAINT [FK_dbo.FilmVote_dbo.Film_Film_Id] FOREIGN KEY ([Film_Id]) REFERENCES [dbo].[Film] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[FilmVote] ADD CONSTRAINT [FK_dbo.FilmVote_dbo.List_List_Id] FOREIGN KEY ([List_Id]) REFERENCES [dbo].[List] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[FilmVote] ADD CONSTRAINT [FK_dbo.FilmVote_dbo.AspNetUsers_User_Id] FOREIGN KEY ([User_Id]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE