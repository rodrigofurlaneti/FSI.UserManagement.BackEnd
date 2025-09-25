using MediatR;

namespace Application.Commands
{
    public class AuthUserCommand : IRequest<string>
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}