using MediatR;
using Application.DTOs;
using System.Collections.Generic;

namespace Application.Queries
{
    public class GetUsersQuery : IRequest<IEnumerable<UserDto>>
    {
    }
}
