# Annoying Cat Launcher
$baseDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
if (-not $baseDir) { $baseDir = (Get-Location).Path }
else { $baseDir = (Resolve-Path $baseDir).Path }

Set-Location $baseDir

# Pass base directory explicitly into .NET AppDomain
[AppDomain]::CurrentDomain.SetData("APP_BASE_DIR", $baseDir)

# Check if sprites exist, if not run sprite extractor
$walkSprite = Join-Path $baseDir "assets\sprites\walk\walk_0.png"
if (-not (Test-Path $walkSprite)) {
    Write-Host "Extracting transparent cat sprites from sheets..." -ForegroundColor Yellow
    $csCode = Get-Content (Join-Path $baseDir "tools\SpriteExtractor.cs") -Raw
    Add-Type -TypeDefinition $csCode -ReferencedAssemblies System.Drawing
    [AnnoyingCat.Tools.SpriteExtractor]::Main(@($baseDir))
}

# Ensure binary is built
$exePath = Join-Path $baseDir "bin\Release\AnnoyingCat.exe"
if (-not (Test-Path $exePath)) {
    Write-Host "Building AnnoyingCat.exe..." -ForegroundColor Yellow
    & "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe" (Join-Path $baseDir "AnnoyingCat.csproj") /p:Configuration=Release /nologo
}

# Launch Annoying Cat natively
Add-Type -AssemblyName PresentationFramework, PresentationCore, WindowsBase, System.Drawing, System.Windows.Forms
$bytes = [System.IO.File]::ReadAllBytes($exePath)
$asm = [System.Reflection.Assembly]::Load($bytes)
$appType = $asm.GetType("AnnoyingCat.App")
$mainMethod = $appType.GetMethod("Main", [System.Reflection.BindingFlags]"Public,Static")
$mainMethod.Invoke($null, @())
