namespace store.Dtos;

public record class CreateGameDto(string Name, string Genre, decimal Price, DateOnly ReleasedDate);