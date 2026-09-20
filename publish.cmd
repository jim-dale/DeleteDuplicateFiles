@REM Publish the project

@SETLOCAL
@SET PROJ_NAME=DeleteDuplicateFiles
@SET PROJ_TO_PUBLISH=src/%PROJ_NAME%.csproj
@SET PROJ_CONFIG=Release
@SET ARTIFACT=%PROJ_NAME%.zip
@SET ARTIFACT_FOLDER=publish

dotnet clean --configuration %PROJ_CONFIG%

dotnet restore "%PROJ_TO_PUBLISH%"

dotnet build "%PROJ_TO_PUBLISH%" --configuration %PROJ_CONFIG% --no-restore

dotnet publish "%PROJ_TO_PUBLISH%" --no-build --configuration %PROJ_CONFIG% --output "%ARTIFACT_FOLDER%"


@IF EXIST "%ARTIFACT%" @DEL "%ARTIFACT%"

tar -a -c -f "%ARTIFACT%" "%ARTIFACT_FOLDER%"

@ENDLOCAL
