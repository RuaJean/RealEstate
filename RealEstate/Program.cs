using System.Diagnostics;

// Lanzador: reenvía `dotnet run` en la raíz a la API
var startInfo = new ProcessStartInfo
{
    FileName = "dotnet",
    Arguments = "run --project src/RealEstate.Api/RealEstate.Api.csproj",
    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    WorkingDirectory = Directory.GetCurrentDirectory(),
};

// Forzar URL conocida para pruebas locales
startInfo.Environment["ASPNETCORE_URLS"] = "http://localhost:5106";
startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";

Console.WriteLine("Iniciando RealEstate.Api en http://localhost:5106 ...\n");
using var proc = Process.Start(startInfo)!;

proc.OutputDataReceived += (_, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
proc.ErrorDataReceived += (_, e) => { if (e.Data != null) Console.Error.WriteLine(e.Data); };
proc.BeginOutputReadLine();
proc.BeginErrorReadLine();

proc.WaitForExit();
