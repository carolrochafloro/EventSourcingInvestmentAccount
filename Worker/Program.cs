using Domain.Business;
using Domain.EventHandlers;
using Domain.Interfaces;
using Infra.Data;
using Infra.Queue;
using Microsoft.EntityFrameworkCore;
using WorkerApp;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddDbContext<EventSourcingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IQueue>(provider =>
{
    var queue = Queue.CreateAsync("rabbitmq").GetAwaiter().GetResult();
    return queue;
});

builder.Services.AddScoped<IHandler, Handler>();
builder.Services.AddScoped<IData, Data>();
builder.Services.AddScoped<ICapitalContributionHandler, CapitalContributionHandler>();
builder.Services.AddScoped<IWithdrawalHandler, WithdrawalHandler>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
