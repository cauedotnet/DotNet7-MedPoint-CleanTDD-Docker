using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedPoint.Domain.Entities;

namespace MedPoint.Domain.Interfaces;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User> GetByUsernameAsync(string username);
    Task<User> GetByIdAsync(Guid id);
    Task<bool> SetUserRoleAsync(Guid userId, string role);
    Task<(IEnumerable<User>, long)> ListAsync(int pageNumber, int pageSize);

    // We also could have another methods like:
    //Task UpdateAsync(User user);
    //Task DeleteAsync(Guid id);
}
