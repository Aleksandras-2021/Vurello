namespace PSK.Server.Interceptors;

using Castle.DynamicProxy;
using System;
using System.IO;
using System.Security.Claims;
using System.Text;

public class LoggingInterceptor : IInterceptor
{
    private readonly string _logFilePath;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly bool _enabled;

    public LoggingInterceptor(string logFilePath, IHttpContextAccessor httpContextAccessor, bool enabled)
    {
        _logFilePath = logFilePath;
        _httpContextAccessor = httpContextAccessor;
        _enabled = enabled;
    }

    public void Intercept(IInvocation invocation)
    {

        if (!_enabled)
        {
            invocation.Proceed();
            return;
        }

        var logEntry = new StringBuilder();

        logEntry.AppendLine($"Timestamp: {DateTime.UtcNow:o}");
        logEntry.AppendLine($"Action: {invocation.TargetType.Name}.{invocation.Method.Name}");

        var user = _httpContextAccessor.HttpContext?.User;

        if (user != null)
        {
            var username = user.Identity?.Name ?? "Unknown";
            var roles = string.Join(", ", user.FindAll(ClaimTypes.Role).Select(c => c.Value));
            logEntry.AppendLine($"User: {username}");
            logEntry.AppendLine($"Roles: {roles}");
        }
        else
        {
            logEntry.AppendLine("User: Not authenticated");
            logEntry.AppendLine("Roles: None");
        }

        logEntry.AppendLine();

        try
        {
            invocation.Proceed();
        }
        catch (Exception ex)
        {
            logEntry.AppendLine($"Exception: {ex.Message}");
            throw;
        }
        finally
        {
            WriteLogEntry(logEntry.ToString());
        }
    }

    private void WriteLogEntry(string logEntry)
    {
        try
        {
            using (var stream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine(logEntry);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error writing to log file: {ex.Message}");
        }
    }
}
