namespace TestRepo.Data.Dtos;

public record PersonDto
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? Email { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
    public string UserName { get; init; } = string.Empty;
    public const string Password = "********************************";
}
