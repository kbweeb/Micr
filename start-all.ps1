param(
  [switch]$NoUI
)

Write-Host "Starting Domain.Host, DataAccess.Host, BusinessLogic.Host, and MicrUI..."

$procs = @()

function Start-Proj($name, $path) {
  $args = "run --no-build --project `"$path`""
  if ($NoUI) {
    Start-Process -FilePath dotnet -ArgumentList $args -NoNewWindow
  } else {
    Start-Process -FilePath dotnet -ArgumentList $args
  }
}

Start-Proj "Domain.Host" "Domain.Host"
Start-Proj "DataAccess.Host" "DataAccess.Host"
Start-Proj "BusinessLogic.Host" "BusinessLogic.Host"
Start-Proj "MicrUI" "MicrUI"

Write-Host "All processes launched."