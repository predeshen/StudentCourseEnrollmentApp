using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StudentCourseEnrollmentApp.Infrastructure.Data;
using StudentCourseEnrollmentApp.UI;
using StudentCourseEnrollmentApp.UI.Services;
using StudentCourseEnrollmentApp.UI.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7026") });

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>(); 
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthenticationService, StudentCourseEnrollmentApp.UI.Services.AuthenticationService>();
builder.Services.AddScoped<ICourseService, StudentCourseEnrollmentApp.UI.Services.CourseService>();
builder.Services.AddScoped<IEnrollmentService, StudentCourseEnrollmentApp.UI.Services.EnrollmentService>();
await builder.Build().RunAsync();
