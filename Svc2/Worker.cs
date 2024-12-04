using amLogger;

namespace Svc2;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Service started
        Logger.Instance.Init(log => OnLog(log));

        var msg = "Телебот запущен";
        await amTelebot.Worker.Start(msg);
        Log.Info(1, "Svc2.Worker.ExecuteAsync", msg);
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // Service stoped
        var msg = "Телебот выключен";
        await amTelebot.Worker.Stop(msg);
        Log.Info(1, "Svc2.Worker.StopAsync", msg);

        await base.StopAsync(cancellationToken);
    }
    void OnLog(Log log)
    {
        // Писать лог для внешнего просмотра будем тут
        // удобнее всего писать в журнал событий windows (Windows Event Log)
        switch (log.lvl)
        {
            case Level.Info:
                _logger.LogInformation(log.id, log.msg, log.src);
                break;
            case Level.Error:
                _logger.LogError(log.id, log.msg, log.src);
                break;
            case Level.Debug:
                _logger.LogDebug(log.msg);
                break;
        }
    }
}
