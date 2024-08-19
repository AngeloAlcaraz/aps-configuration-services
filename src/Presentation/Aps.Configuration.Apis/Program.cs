var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddValidatorsFromAssemblyContaining<AccountsRequestDtoValidator>();

builder.Services.RegisterServices(builder.Configuration);

//builder.Configuration.AddJsonFile("appsettings.json");
var swaggerSettings = builder.Configuration.GetSection("SwaggerSettings");
var termsOfServiceUrl = swaggerSettings.GetValue<string>("TermsOfServiceUrl");

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "APS Configuration API Services",
        Version = "v1",
        Description = "Retrieves essential configuration data for APS operations.",
        TermsOfService = new Uri(termsOfServiceUrl)
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        swagger.Servers = new List<OpenApiServer> { new() { Url = $"https://{httpReq.Host.Value}" } };
    });
});

app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();