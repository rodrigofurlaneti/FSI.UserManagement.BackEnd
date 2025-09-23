using MediatR;

namespace Application.Commands
{
    public class AuthUserCommand : IRequest<string>
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}
