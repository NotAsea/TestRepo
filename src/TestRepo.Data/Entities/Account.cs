// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace TestRepo.Data.Entities;

internal class Account
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(100)] public string UserName { get; set; } = string.Empty;

    [MaxLength(100)] public string Password { get; set; } = string.Empty;

    public int PersonId { get; set; }
}