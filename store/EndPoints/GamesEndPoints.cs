using System;
using store.Dtos;

namespace store.EndPoints;

public static class GamesEndPoints
{
    private static List<GameDto> games = [
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

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {

        var group = app.MapGroup("games").WithParameterValidation();
        const string GetGameEndpointName = "GetGame";

        group.MapGet("/", ()=>games);

        group.MapGet("/{id}", (int id) =>
        {
            GameDto? game = games.Find(game => game.Id == id);
            return game is null ? Results.NotFound() : Results.Ok(game);
        })
        .WithName(GetGameEndpointName);


        // we can add validations seperatly like this or we can it to the group
        group.MapPost("/", (CreateGameDto newGame)=> {
            GameDto game = new (
                games.Count + 1,
                newGame.Name,
                newGame.Genre,
                newGame.Price,
                newGame.ReleasedDate
            );
            games.Add(game);
            return Results.CreatedAtRoute(GetGameEndpointName, new {id = game.Id}, game);
        })
        .WithParameterValidation();



        group.MapPut("/{id}",(int id, UpdateGameDto updatedGame)=>{
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



        group.MapDelete("/{id}", (int id)=> {
            games.RemoveAll(game=> game.Id==id);
            return Results.NoContent();
        });

        return group;
    }
}
