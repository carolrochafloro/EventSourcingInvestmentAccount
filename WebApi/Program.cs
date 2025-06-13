using Domain.Business;
using Domain.EventHandlers;
using Domain.Interfaces;
using Infra.Data;
using Infra.Queue;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IQueue>(provider =>
{
    var queue = Queue.CreateAsync("rabbitmq").GetAwaiter().GetResult();
    return queue;
});

builder.Services.AddScoped<IBusiness, Business>();
builder.Services.AddScoped<IData, Data>();
builder.Services.AddScoped<ICapitalContributionHandler, CapitalContributionHandler>();
builder.Services.AddScoped<IWithdrawalHandler, WithdrawalHandler>();

builder.Services.AddDbContext<EventSourcingDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}  
app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EventSourcingDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
