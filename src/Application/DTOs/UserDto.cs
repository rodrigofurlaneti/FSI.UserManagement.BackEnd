using System;

namespace Application.DTOs
{
    public record UserDto(Guid Id, string Name, string Email, DateTime CreatedAt);
}
