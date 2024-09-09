namespace store.Dtos;

public record class UpdateGameDto(string Name, string Genre, decimal Price, DateOnly ReleasedDate);
