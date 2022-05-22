using Rewards.Command;
using Rewards.Data;
using Rewards.Middleware;
using Rewards.Services;

var builder = WebApplication.CreateBuilder(args);

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
    services.AddControllers(options => options.Filters.Add<HttpResponseExceptionFilter>());
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        options.CustomSchemaIds(type => type.ToString());
    });

    RegisterCommands(services);
    services.AddScoped<ICommandExecutor, CommandExecutor>();
    services.AddScoped<IDateTimeService, DateTimeService>();

    if (configuration.GetValue<string>("PersistentStorage") == "FileStorage")
    {
        services.AddSingleton<IStorageProvider, FileStorageProvider>();
        if (!Directory.Exists("users"))
            Directory.CreateDirectory("users");
    }
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