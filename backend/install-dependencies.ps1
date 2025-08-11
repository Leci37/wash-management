# Path where the projects live
$solutionRoot = "C:\Users\llecinana\source\repos\controlmat\src"

# Path to the solution file (outside src)
$solutionFile = "C:\Users\llecinana\source\repos\controlmat\controlmat.sln"

# Define projects and their dependencies
$projects = @{
    "controlmat.Api" = @(
        @{ Name = "Microsoft.EntityFrameworkCore.Design"; Version = "6.0.25" }
        @{ Name = "NSwag.AspNetCore"; Version = "14.1.0" }
    )
    "controlmat.Domain" = @(
        @{ Name = "MediatR"; Version = "12.3.0" }
    )
    "controlmat.Infrastructure" = @(
        @{ Name = "Azure.Identity"; Version = "1.13.2" }
        @{ Name = "DocumentFormat.OpenXml"; Version = "3.3.0" }
        @{ Name = "Microsoft.EntityFrameworkCore"; Version = "6.0.25" }
        @{ Name = "Microsoft.EntityFrameworkCore.SqlServer"; Version = "6.0.25" }
        @{ Name = "Microsoft.EntityFrameworkCore.Tools"; Version = "6.0.25" }
        @{ Name = "Microsoft.Extensions.Configuration.Binder"; Version = "6.0.0" }
        @{ Name = "Microsoft.Graph"; Version = "5.77.0" }
    )
    "controlmat.Application" = @(
        @{ Name = "AutoMapper.Extensions.Microsoft.DependencyInjection"; Version = "12.0.1" }
        @{ Name = "FluentValidation"; Version = "11.9.2" }
        @{ Name = "FluentValidation.AspNetCore"; Version = "11.3.0" }
        @{ Name = "MediatR"; Version = "12.3.0" }
        @{ Name = "Serilog"; Version = "4.0.0" }
        @{ Name = "Serilog.AspNetCore"; Version = "6.1.0" }
    )
}

Write-Host "Starting NuGet package installation..."

foreach ($project in $projects.Keys) {
    $projectPath = Join-Path $solutionRoot $project
    Write-Host "`nInstalling packages for $project..."

    foreach ($package in $projects[$project]) {
        $name = $package.Name
        $version = $package.Version
        Write-Host "  Installing $name v$version..."
        dotnet add "$projectPath\$project.csproj" package $name --version $version
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Failed to install $name for $project"
            exit 1
        }
    }
}

Write-Host "`nRestoring solution packages..."
dotnet restore $solutionFile
if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet restore failed"
    exit 1
}

Write-Host "Building solution to validate installation..."
dotnet build $solutionFile --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet build failed"
    exit 1
}

Write-Host "`nAll dependencies installed and validated successfully."
