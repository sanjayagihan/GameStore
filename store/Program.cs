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



app.MapGet("games/{id}", (int id) =>
{
    GameDto? game = games.Find(game => game.Id == id);
    return game is null ? Results.NotFound() : Results.Ok(game);
})
.WithName("GetGame");



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



app.MapPut("games/{id}",(int id, UpdateGameDto updatedGame)=>{
    int index = games.FindIndex(game => game.Id==id);
    if(index==-1){
        return Results.NotFound();
    }
    games[index] = new GameDto(
        id, 
        updatedGame.Name,
        updatedGame.Genre,
        updatedGame.Price,
        updatedGame.ReleasedDate
    );
    return Results.NoContent();
});



app.MapDelete("games/{id}", (int id)=> {
    games.RemoveAll(game=> game.Id==id);
    return Results.NoContent();
});



app.Run();
