using System;
using Microsoft.EntityFrameworkCore;
using store.Data;
using store.Dtos;
using store.Entities;
using store.Mapping;

namespace store.EndPoints;

public static class GamesEndPoints
{
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {

        var group = app.MapGroup("games").WithParameterValidation();
        const string GetGameEndpointName = "GetGame";

         group.MapGet("/", async (GameStoreContext dbContext) => 
            await dbContext.Games
                     .Include(game => game.Genre)
                     .Select(game => game.ToGameSummaryDto())
                     .AsNoTracking()
                     .ToListAsync());

        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(id);
            return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        })
        .WithName(GetGameEndpointName);


        // we can add validations seperatly like this or we can it to the group
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbCOntext) =>
        {
            Game game = newGame.ToEntity();
            // instead of sending gameDto we can also send game object back to user
            
            dbCOntext.Games.Add(game);
            await dbCOntext.SaveChangesAsync();
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game.ToGameDetailsDto());
        })
        .WithParameterValidation();



        group.MapPut("/{id}", async(int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);
            if (existingGame is null)
            {
                return Results.NotFound();
            }
            dbContext.Entry(existingGame)
            .CurrentValues
            .SetValues(updatedGame.ToEntity(id));
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });



        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games
            .Where(game=> game.Id==id)
            .ExecuteDeleteAsync();
            return Results.NoContent();
        });

        return group;
    }
}
