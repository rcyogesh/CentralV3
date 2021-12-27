using CentralV3.Controllers;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.WebHost.UseUrls("http://*:5001");
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

ThreadPool.QueueUserWorkItem(x =>
{
    while (true)
    {
        DateTime dateTime = DateTime.Now;
        //SwitchController.state = "Turning ON at {"
        int msTimeout = 10 * 60 * 1000;
        SwitchController.NextStateChangeAt=DateTime.Now+TimeSpan.FromMilliseconds(msTimeout);
        Thread.Sleep(msTimeout);
        SwitchController._SwitchOn();
        msTimeout = 30 * 1000;
        SwitchController.NextStateChangeAt=DateTime.Now+TimeSpan.FromMilliseconds(msTimeout);
        Thread.Sleep(msTimeout); //ON for 30 seconds
        SwitchController._SwitchOff();
    }
});

app.Run();
