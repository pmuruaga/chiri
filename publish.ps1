$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectPath = Join-Path $repoRoot "ControlBingosChiri\ControlBingosChiri.csproj"
$publishFolder = Join-Path $repoRoot "Publicacion"

$msbuildCandidates = @(
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
)

$msbuild = $msbuildCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $msbuild) {
    Write-Host ""
    Write-Host "No se encontro MSBuild. Instala una de estas opciones y volve a ejecutar:" -ForegroundColor Yellow
    Write-Host "  1) Visual Studio 2022 Community (recomendado)"
    Write-Host "     https://visualstudio.microsoft.com/es/vs/community/"
    Write-Host "     Workloads: 'Desarrollo de ASP.NET y web' y '.NET desktop'"
    Write-Host ""
    Write-Host "  2) Build Tools for Visual Studio 2022 (solo compilacion, sin IDE)"
    Write-Host "     https://visualstudio.microsoft.com/es/downloads/#build-tools-for-visual-studio-2022"
    Write-Host ""
    exit 1
}

Write-Host "Usando MSBuild: $msbuild"
Write-Host "Publicando en: $publishFolder"
Write-Host ""

& $msbuild $projectPath `
    /t:WebPublish `
    /p:WebPublishMethod=FileSystem `
    /p:PublishProfile=Publicacion `
    /p:Configuration=Release `
    /p:Platform="Any CPU" `
    /p:DeployOnBuild=true `
    /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "La publicacion fallo." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "Publicacion lista en:" -ForegroundColor Green
Write-Host "  $publishFolder"
Write-Host ""
Write-Host "Subi el contenido de esa carpeta por FTP al hosting (no la carpeta Publicacion en si)."
Write-Host "Luego recicla el Application Pool del sitio en IIS si aplica."
