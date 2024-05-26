using Serilog;

namespace MiddleWaring;
public static class Log2FileExtension
{
  public static ILoggingBuilder Log2File(this ILoggingBuilder loggingBuilder, string logFileName = "Log2File.txt")
  {
    var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", logFileName);
    var retainedFileCountLimit = 90;

    var logger = new LoggerConfiguration().WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: retainedFileCountLimit).CreateLogger();

    loggingBuilder.AddSerilog(logger);

    return loggingBuilder;
  }
}