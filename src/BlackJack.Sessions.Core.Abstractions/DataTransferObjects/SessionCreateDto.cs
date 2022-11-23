using System.ComponentModel.DataAnnotations;

namespace BlackJack.Sessions.Core.Abstractions.DataTransferObjects;

public class SessionCreateDto
{
    [Required]
    public Guid UserId { get; set; } = Guid.Empty;

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; set; } = null!;
}