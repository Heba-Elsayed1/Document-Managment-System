using Application.Common;
using Application.Dto;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IWorkspaceService
    {
        Task<GenericResult<IEnumerable<WorkspaceDto>>> GetAllWorkspaces();
        Task<GenericResult<WorkspaceDto>> GetWorkspaceById(int id, int userId);
        Task<GenericResult<WorkspaceDto>> GetWorkspaceByUser(int userId);
        Task<Result> UpdateWorkspace (WorkspaceDto workspaceDto, int userId);
        Task<bool> WorkspaceExists (string name);

    }
}
