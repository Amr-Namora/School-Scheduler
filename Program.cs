using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Infrastructure.Identity;
using SchoolScheduler.Infrastructure.Persistence;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Business DbContext
builder.Services.AddDbContext<SchoolSchedulerDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<SchoolSchedulerDbContext>());

// Identity DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity Setup
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Application Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentSchoolContext, CurrentSchoolContext>();
builder.Services.AddScoped<IUserLookupService, UserLookupService>();
builder.Services.AddScoped<ITimetableGenerationService, TimetableGenerationService>();

// MediatR - Scan for handlers in the Application assembly
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IApplicationDbContext).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
