# SSE Server (Test.SseServer)

This small ASP.NET Core app exposes Server-Sent Events (SSE) endpoints used by the simulation tool in `tools/simulate`.

Quick start:

```bash
# From repository root (recommended):
dotnet run --project tools/sse-server -- --urls http://localhost:5001

# Or from the server folder:
cd tools/sse-server
dotnet run -- --urls http://localhost:5001
```

Endpoints:

- `/quotes/random?interval=100&batchSize=1000&quoteInterval=1h` — continuous random quote stream
- `/quotes/longest?interval=100&batchSize=1000&quoteInterval=5m` — deterministic stream using the longest quote dataset

Query parameters:

| Parameter | Type | Default | Description |
| --------- | ---- | ------- | ----------- |
| `interval` | int | `100` | Delivery rate in milliseconds (how fast quotes are sent) |
| `batchSize` | int | none (unlimited) | Maximum number of quotes to send before closing stream |
| `quoteInterval` | string | `1m` | Time warp: timestamp spacing between quotes (e.g., `1s`, `5m`, `1h`, `1d`) |

**Time warp feature**: The `quoteInterval` parameter enables fast testing of strategies that use longer timeframes. For example, `quoteInterval=1h` with `interval=100` delivers hourly-spaced quotes every 100ms, allowing you to test a full day (24 hours) of hourly data in just 2.4 seconds.

Stopping the server:

- Press `Ctrl+C` in the terminal where `dotnet run` is running to request a graceful shutdown.
- If the process does not exit, you can use the VS Code tasks added to the repository (see `.vscode/tasks.json`) to stop stray hosts.

Manual PowerShell kill (if needed):

```powershell
Get-CimInstance Win32_Process | Where-Object { ($_.Name -eq 'dotnet.exe' -or $_.Name -match 'Test.SseServer') -and ($_.CommandLine -match 'tools\\sse-server' -or $_.CommandLine -match 'Test.SseServer') } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force }
```

Notes:

- The server prints start/stop and delivery logs to the console; use the output to confirm it is running and which port is bound.
- `ServerManager` in `tools/simulate` attempts to start the built executable (`bin/.../Test.SseServer.exe`) and falls back to `dotnet run` when the exe is not present.
