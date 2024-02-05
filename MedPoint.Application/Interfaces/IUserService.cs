using MedPoint.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoint.Application.Interfaces;

public interface IUserService
{
    Task<User> AuthenticateAsync(string username, string password);
    Task<User> RegisterAsync(User user, string password);
    Task<User> GetUserByIdAsync(Guid userId);
    Task<bool> SetUserRoleAsync(Guid userId, string role);
    Task<(IEnumerable<User>, long)> ListUsersAsync(int pageNumber, int pageSize);

}
