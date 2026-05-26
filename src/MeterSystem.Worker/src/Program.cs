using MeterSystem.Worker.src;

await Host.CreateApplicationBuilder(args)
    .ConfigureServices()
    .BuildApplication()
    .RunAsync();

