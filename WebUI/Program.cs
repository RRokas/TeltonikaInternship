var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
AddHttpClientIgnoringSsl(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

static void AddHttpClientIgnoringSsl(IServiceCollection services, IConfiguration configuration)
{
    var ignoreSslErrors = configuration.GetValue<bool>("ComparisonHttpClient:IgnoreSslErrors");
    var baseAddress = configuration["ComparisonHttpClient:BaseAddress"];

    services.AddHttpClient("ComparisonClient", client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
            new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = ignoreSslErrors 
                    ? HttpClientHandler.DangerousAcceptAnyServerCertificateValidator 
                    : null
            });
}