using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SchoolScheduler.Infrastructure.Persistence;
using System;
using System.IO;

namespace SchoolScheduler.Infrastructure.Persistence;

public class SchoolSchedulerDbContextFactory : IDesignTimeDbContextFactory<SchoolSchedulerDbContext>
{
    public SchoolSchedulerDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<SchoolSchedulerDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new SchoolSchedulerDbContext(optionsBuilder.Options, null!);
    }
}
