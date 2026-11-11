using MCPTranslator.Services;

namespace MCPTranslator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpClient<IJsonRpcClient, JsonRpcClient>();

            var app = builder.Build();

            await app.RunAsync();
        }
    }
}
