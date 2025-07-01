using Clue_Me_In;
using Data;
using Managers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using ViewModels;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddFluentUIComponents();

// Register the WordsService 
builder.Services.AddScoped<WordsService>();
// Register the GameManager
builder.Services.AddScoped<IGameManager, GameManager>();
// Register the PlayViewModel
builder.Services.AddScoped<IPlayViewModel, PlayViewModel>();

await builder.Build().RunAsync();
