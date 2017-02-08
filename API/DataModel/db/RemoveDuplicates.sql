/* remove Actor duplicates*/
DECLARE @RemoveActorDirectorDuplicatesCursor CURSOR;
DECLARE @FilmId int;
BEGIN
    SET @RemoveActorDirectorDuplicatesCursor = CURSOR FOR
	WITH CTE AS(
    SELECT [Id], [Name], [OriginalName], [ReleaseDate], [Genre], [Description], [Cover], [FilmwebUrl],
       RN = ROW_NUMBER()OVER(PARTITION BY Name ORDER BY Name)
    FROM dbo.Film
    )
    SELECT Id FROM CTE WHERE RN > 1

    OPEN @RemoveActorDirectorDuplicatesCursor 
    FETCH NEXT FROM @RemoveActorDirectorDuplicatesCursor 
    INTO @FilmId

    WHILE @@FETCH_STATUS = 0
    BEGIN
	  DELETE FROM Actor WHERE Film_Id=@FilmId;
	  DELETE FROM Director WHERE Film_Id=@FilmId;
      FETCH NEXT FROM @RemoveActorDirectorDuplicatesCursor 
      INTO @FilmId
    END; 

    CLOSE @RemoveActorDirectorDuplicatesCursor ;
    DEALLOCATE @RemoveActorDirectorDuplicatesCursor;
END;
GO


/* remove Film duplicates*/
WITH CTE AS(
   SELECT [Id], [Name], [OriginalName], [ReleaseDate], [Genre], [Description], [Cover], [FilmwebUrl],
       RN = ROW_NUMBER()OVER(PARTITION BY Name ORDER BY Name)
   FROM dbo.Film
)
DELETE FROM CTE WHERE RN > 1
