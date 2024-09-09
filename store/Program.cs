using store.Data;
using store.EndPoints;

var builder = WebApplication.CreateBuilder(args);

// getting connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connectionString);
// dotnet ef migrations add InitialCreate --output-dir Data\Migrations
// dotnet ef database update

var app = builder.Build();

app.MapGamesEndpoints();

app.MigrateDb();

app.Run();
