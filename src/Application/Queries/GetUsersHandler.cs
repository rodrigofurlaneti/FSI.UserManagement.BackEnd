using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using Application.DTOs;
using Domain.Repositories;
using System.Linq;

namespace Application.Queries
{
    public class GetUsersHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IUserRepository _repo;

        public GetUsersHandler(IUserRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var users = _repo.GetAll();
            var result = users.Select(u => new UserDto(u.Id, u.Name, u.Email, u.CreatedAt));
            return Task.FromResult(result);
        }
    }
}
