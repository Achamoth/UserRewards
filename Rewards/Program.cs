using Rewards.Command;
using Rewards.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
{
    RegisterCommands(services);
    services.AddScoped<ICommandExecutor, CommandExecutor>();

    if (configuration.GetValue<string>("PersistentStorage") == "FileStorage")
    {
        services.AddSingleton<IStorageProvider, FileStorageProvider>();
    }
    // Otherwise I would use a DB
}

static void RegisterCommands(IServiceCollection services)
{
    var allTypesInAssembly = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
    var commands = allTypesInAssembly
        .Where(t => t
            .GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>)));
    foreach (var command in commands)
    {
        services.AddScoped(command);
    }
}