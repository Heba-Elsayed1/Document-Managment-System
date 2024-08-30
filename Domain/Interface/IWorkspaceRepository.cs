using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IWorkspaceRepository :IGenericRepository<Workspace>
    {
        Task<Workspace> GetWorkspaceByName(string name);
        Task<Workspace> GetWorkspaceByUser(int userId);
        Task<Workspace> GetWorkspaceByIdAndUserId(int workspaceId, int userId);

    }
}
