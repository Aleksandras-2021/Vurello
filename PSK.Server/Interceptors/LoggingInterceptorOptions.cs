namespace PSK.Server.Interceptors;

public class LoggingInterceptorOptions
{
    public bool Enabled { get; set; } = false;
    public string LogFilePath { get; set; } = "PSK.log";
}
