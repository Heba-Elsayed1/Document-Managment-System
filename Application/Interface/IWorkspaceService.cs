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
        Task<IEnumerable<WorkspaceDto>> GetAllWorkspaces();
        Task<WorkspaceDto> GetWorkspaceById(int id, int userId);
        Task<WorkspaceDto> GetWorkspaceByUser(int userId);

        //Task<bool> CreateWorkspace(Workspace workspace);
        Task<bool> UpdateWorkspace (WorkspaceDto workspaceDto, int userId);
        Task<bool> WorkspaceExists (string name);

    }
}
