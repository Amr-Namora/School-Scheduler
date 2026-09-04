using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolScheduler.Infrastructure.Persistence;
using SchoolScheduler.Application.Common.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SchoolScheduler.IntegrationTests;

public class SchoolSchedulerFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DB contexts
            var descriptor1 = services.SingleOrDefault(d => d.ServiceType == typeof(SchoolSchedulerDbContext));
            if (descriptor1 != null) services.Remove(descriptor1);

            var descriptor2 = services.SingleOrDefault(d => d.ServiceType == typeof(ApplicationDbContext));
            if (descriptor2 != null) services.Remove(descriptor2);

            var descriptor3 = services.SingleOrDefault(d => d.ServiceType == typeof(IApplicationDbContext));
            if (descriptor3 != null) services.Remove(descriptor3);

            // Add InMemory DB
            services.AddDbContext<SchoolSchedulerDbContext>(options =>
                options.UseInMemoryDatabase("TestSchoolDb"));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TestApplicationDb"));

            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<SchoolSchedulerDbContext>());
        });
    }
}
