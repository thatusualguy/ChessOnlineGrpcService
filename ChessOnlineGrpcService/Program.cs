using ChessOnlineGrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(); 
builder.Services.AddGrpcReflection();

var app = builder.Build();


//app.Urls.Add("https://*:7106"); 
app.Urls.Add("http://*:7105"); 
// Configure the HTTP request pipeline.
app.UseRouting();
//app.UseAuthentication();
//app.UseAuthorization();
app.MapGrpcService<ChessAccountService>();
app.MapGrpcService<ChessGameService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn more about service, visit: https://github.com/thatusualguy/ChessOnlineGrpcService");

// Allows reflection for debugging
IWebHostEnvironment env = app.Environment;
if (env.IsDevelopment())
	app.MapGrpcReflectionService();

app.Run();

