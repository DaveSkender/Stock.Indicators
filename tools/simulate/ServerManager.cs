using System.Diagnostics;

namespace Test.Simulation;

internal static class ServerManager
{
    internal static Process? StartServer(int port)
    {
        try
        {
            // Find the server DLL - try multiple locations
            string[] possiblePaths = [
                // Running from bin directory
                Path.Combine("..", "..", "..", "..", "..", "..", "server", "bin", "Debug", "net10.0", "Test.SseServer.dll"),
                // Running from project root
                Path.Combine("tools", "server", "bin", "Debug", "net10.0", "Test.SseServer.dll"),
                // Absolute path based on current directory
                Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "..", "server", "bin", "Debug", "net10.0", "Test.SseServer.dll"))
            ];

            string? serverPath = null;
            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    serverPath = Path.GetFullPath(path);
                    break;
                }
            }

            if (serverPath is null)
            {
                Console.WriteLine($"[ServerManager] Server not found. Skipping server start.");
                Console.WriteLine($"[ServerManager] Current directory: {Directory.GetCurrentDirectory()}");
                return null;
            }

            ProcessStartInfo startInfo = new()
            {
                FileName = "dotnet",
                Arguments = $"\"{serverPath}\" --urls http://localhost:{port}",
                UseShellExecute = false,
                CreateNoWindow = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };

            Process? process = Process.Start(startInfo);
            Console.WriteLine($"[ServerManager] SSE server started on port {port} (PID: {process?.Id})");
            return process;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[ServerManager] I/O error starting server: {ex.Message}");
            return null;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"[ServerManager] Invalid operation: {ex.Message}");
            return null;
        }
    }

    internal static void StopServer(Process? process)
    {
        if (process is not null && !process.HasExited)
        {
            try
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit(3000);
                Console.WriteLine("[ServerManager] SSE server stopped");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[ServerManager] Error stopping server: {ex.Message}");
            }
        }
    }
}
