using MediatR;
using Application.DTOs;

namespace Application.Commands
{
    public class CreateUserCommand : IRequest<UserDto>
    {
        public string Name { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}
