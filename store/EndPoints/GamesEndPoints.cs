using System;
using Microsoft.EntityFrameworkCore;
using store.Data;
using store.Dtos;
using store.Entities;
using store.Mapping;

namespace store.EndPoints;

public static class GamesEndPoints
{
    private static List<GameSummaryDto> games = [
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

         group.MapGet("/", (GameStoreContext dbContext) => 
            dbContext.Games
                     .Include(game => game.Genre)
                     .Select(game => game.ToGameSummaryDto())
                     .AsNoTracking());

        group.MapGet("/{id}", (int id, GameStoreContext dbContext) =>
        {
            Game? game = dbContext.Games.Find(id);
            return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        })
        .WithName(GetGameEndpointName);


        // we can add validations seperatly like this or we can it to the group
        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbCOntext) =>
        {
            Game game = newGame.ToEntity();
            // instead of sending gameDto we can also send game object back to user
            
            dbCOntext.Games.Add(game);
            dbCOntext.SaveChanges();
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game.ToGameDetailsDto());
        })
        .WithParameterValidation();



        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = dbContext.Games.Find(id);
            if (existingGame is null)
            {
                return Results.NotFound();
            }
            dbContext.Entry(existingGame)
            .CurrentValues
            .SetValues(updatedGame.ToEntity(id));
            dbContext.SaveChanges();
            return Results.NoContent();
        });



        group.MapDelete("/{id}", (int id, GameStoreContext dbContext) =>
        {
            dbContext.Games
            .Where(game=> game.Id==id)
            .ExecuteDelete();
            return Results.NoContent();
        });

        return group;
    }
}
