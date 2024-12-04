using Svc2;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddWindowsService(options =>
    options.ServiceName = "AM Telebot"
);

builder.Logging.AddEventLog(c => {
    c.LogName = "AM Telebot LN";
    c.SourceName = "AM Telebot SRC";
});

var host = builder.Build();
host.Run();
