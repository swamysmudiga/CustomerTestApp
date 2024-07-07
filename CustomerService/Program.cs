using Serilog;

namespace CustomerService;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        builder.Host.UseSerilog();
        builder.Services.AddGrpc();

        WebApplication app = builder.Build();
        app.MapGrpcService<Services.CustomerService>();
        app.MapGet("/", () => "Communication with gRPC endpooints must be made through a gRPC client.");
        app.Run();
    }
}