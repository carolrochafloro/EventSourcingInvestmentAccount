using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Data;
public class EventSourcingDbContextFactory :  IDesignTimeDbContextFactory<EventSourcingDbContext>
{
    public EventSourcingDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../WebApi"))
            .AddJsonFile("appsettings.Development.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("Postgres");

        var optionsBuilder = new DbContextOptionsBuilder<EventSourcingDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new EventSourcingDbContext(optionsBuilder.Options);
    }
}
