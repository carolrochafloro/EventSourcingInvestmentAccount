using Domain.Business;
using Domain.EventHandlers;
using Domain.Interfaces;
using Infra.Data;
using Infra.Queue;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IBusiness, Business>();
builder.Services.AddSingleton<IQueue, Queue>();
builder.Services.AddScoped<IData, Data>();
builder.Services.AddScoped<ICapitalContributionHandler, CapitalContributionHandler>();
builder.Services.AddScoped<IWithdrawalHandler, WithdrawalHandler>();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}  
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
