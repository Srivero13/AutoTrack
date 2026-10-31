$ErrorActionPreference = "Stop"

dotnet tool update --global dotnet-ef --version 8.0.13

$conexion = "server=localhost;port=3307;database=autotrack;user=autotrack;password=autotrack123"

dotnet ef dbcontext scaffold $conexion Pomelo.EntityFrameworkCore.MySql --project src/AutoTrack.Infrastructure --startup-project src/AutoTrack.Api --context AutoTrackContext --context-dir ../../scaffold-output/Data --output-dir ../../scaffold-output/Entities --namespace AutoTrack.Core.Entities --context-namespace AutoTrack.Infrastructure.Data --no-onconfiguring --force
