using Asp.Versioning;
using DotNetEnv.Configuration;
using Microsoft.OpenApi.Models;
using PadEbankETLService.Data.Account;
using PadEbankETLService.Data.DataWarehouse;
using PadEbankETLService.Data.Transaction;
using PadEbankETLService.Services;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel();

builder.Configuration.AddDotNetEnv(".env.local");

builder.Services.AddDbContext<AccountDbContext>();
builder.Services.AddDbContext<TransactionDbContext>();
builder.Services.AddDbContext<DataWarehouseDbContext>();
builder.Services.AddControllers();
builder.Services.AddHttpLogging();
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();
builder.Services.AddDirectoryBrowser();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSerilog(options => { options.WriteTo.Console(); });
builder.Services.AddSwaggerGen(options => {
   options.SwaggerDoc("v1", new OpenApiInfo {
      Title = "PAD ETL",
      Description = "ETL microservice",
      Version = "v1",
   });
   options.EnableAnnotations();

   var filePath = Path.Combine(AppContext.BaseDirectory, "PadEbankETLService.xml");
   options.IncludeXmlComments(filePath);
});
builder.Services.AddApiVersioning(options => {
   options.DefaultApiVersion = new ApiVersion(1);
   options.ApiVersionReader = new UrlSegmentApiVersionReader();
   options.ReportApiVersions = true;
   options.AssumeDefaultVersionWhenUnspecified = true;
   options.UnsupportedApiVersionStatusCode = StatusCodes.Status501NotImplemented;
}).AddApiExplorer(options => {
   options.GroupNameFormat = "'v'V";
   options.SubstituteApiVersionInUrl = true;
});

WebApplication app = builder.Build();

// using (IServiceScope scope = app.Services.CreateScope()) {
//    DataWarehouseDbContext dbContext = scope.ServiceProvider.GetRequiredService<DataWarehouseDbContext>();
//    dbContext.Database.Migrate();
// }

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseSwagger(options => { options.RouteTemplate = "Api/Docs/{documentName}/swagger.json"; });
app.UseSwaggerUI(options => {
   options.SwaggerEndpoint("/Api/Docs/v1/swagger.json", "ETL v1");
   options.DocumentTitle = "ETL docs";
   options.RoutePrefix = "Api/Docs";
});
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHealthChecks("/Healthz");
app.MapControllers();

_ = ETLService.Init();

app.Run("http://0.0.0.0:3000");
