using store.Dtos;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<GameDto> games = [
    new (
        1,
        "Street Fighters",
        "Fighting", 
        56.87M,
        new DateOnly(2010, 10, 4)
    ),
    new (
        2,
        "Street Fighters II",
        "Fighting", 
        45.81M,
        new DateOnly(2015, 12, 4)
    )
];

app.MapGet("games", ()=>games);

app.MapGet("games/{id}", (int id)=>games.Find(game=>game.Id==id)).WithName("GetGame");

app.MapPost("games", (CreateGameDto newGame)=> {
    GameDto game = new (
        games.Count + 1,
        newGame.Name,
        newGame.Genre,
        newGame.Price,
        newGame.ReleasedDate
    );
    games.Add(game);
    return Results.CreatedAtRoute("GetGame", new {id = game.Id}, game);
});

app.Run();
