DECLARE @AddFilmIdToActorCursor CURSOR;
DECLARE @FilmId int;
DECLARE @ActorId int;
BEGIN
    SET @AddFilmIdToActorCursor = CURSOR FOR
	select a.id as 'Film Id', b.id as 'Actor Id' 
	from Film a JOIN Actor b ON a.Name=b.FilmName;

    OPEN @AddFilmIdToActorCursor 
    FETCH NEXT FROM @AddFilmIdToActorCursor 
    INTO @FilmId, @ActorId

    WHILE @@FETCH_STATUS = 0
    BEGIN
	  UPDATE Actor set Film_Id=@FilmId WHERE Id=@ActorId;
      FETCH NEXT FROM @AddFilmIdToActorCursor 
      INTO @FilmId, @ActorId
    END; 

    CLOSE @AddFilmIdToActorCursor ;
    DEALLOCATE @AddFilmIdToActorCursor;
END;

GO

DECLARE @AddFilmIdToDirectorCursor CURSOR;
DECLARE @FilmId int;
DECLARE @DirectorId int;
BEGIN
    SET @AddFilmIdToDirectorCursor = CURSOR FOR
	select a.id as 'Film Id', b.id as 'Director Id' 
	from Film a JOIN Director b ON a.Name=b.FilmName;

    OPEN @AddFilmIdToDirectorCursor 
    FETCH NEXT FROM @AddFilmIdToDirectorCursor 
    INTO @FilmId, @DirectorId

    WHILE @@FETCH_STATUS = 0
    BEGIN
	  UPDATE Director set Film_Id=@FilmId WHERE Id=@DirectorId;
      FETCH NEXT FROM @AddFilmIdToDirectorCursor 
      INTO @FilmId, @DirectorId
    END; 

    CLOSE @AddFilmIdToDirectorCursor ;
    DEALLOCATE @AddFilmIdToDirectorCursor;
END;

GO