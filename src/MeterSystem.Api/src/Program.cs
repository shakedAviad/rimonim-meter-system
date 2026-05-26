using MeterSystem.Api.src;

await WebApplication.CreateBuilder(args)
    .ConfigureServices()
    .BuildApplication()
    .RunAsync();



