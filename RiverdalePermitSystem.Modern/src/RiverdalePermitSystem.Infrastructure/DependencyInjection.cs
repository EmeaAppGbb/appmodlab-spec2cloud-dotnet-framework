using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverdalePermitSystem.Application.Interfaces;
using RiverdalePermitSystem.Domain.Services;
using RiverdalePermitSystem.Infrastructure.Data;
using RiverdalePermitSystem.Infrastructure.Services;

namespace RiverdalePermitSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PermitDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("PermitDB")));

        services.AddScoped<IPermitService, PermitService>();
        services.AddScoped<IPlanReviewService, PlanReviewService>();
        services.AddScoped<IInspectionService, InspectionService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IEmailNotificationService, EmailNotificationService>();

        services.AddTransient<PermitFeeCalculator>();
        services.AddTransient<PermitStatusMachine>();

        return services;
    }
}
