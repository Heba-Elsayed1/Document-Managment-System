using Application.Common;
using Application.Dto;
using Application.Interface;
using AutoMapper;
using Domain.Interface;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;

namespace Application.Service
{
    public class WorkspaceService : BaseService, IWorkspaceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WorkspaceService(IUnitOfWork unitOfWork, IMapper mapper ,IWebHostEnvironment environment):base(environment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<GenericResult<IEnumerable<WorkspaceDto>>> GetAllWorkspaces()
        {
            var workspaces = await _unitOfWork.WorkspaceRepository.GetAll();
            if(workspaces == null)
            {
                return GenericResult<IEnumerable < WorkspaceDto >>.Failure("Workspaces not Found");
            }
            var workspacesDto = _mapper.Map<List<WorkspaceDto>>(workspaces);
            return GenericResult<IEnumerable<WorkspaceDto>>.Success(workspacesDto);
        }

        public async Task<GenericResult<WorkspaceDto>> GetWorkspaceById(int id ,int userId)
        {
            if (id < 0)
                return GenericResult<WorkspaceDto>.Failure("Invalid Id");

            var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAndUserId(id, userId);
            if(workspace == null)
            {
                return GenericResult<WorkspaceDto>.Failure("Not Authorize to this workspace or workspace not found");
            }
            var workspaceDto = _mapper.Map<WorkspaceDto>(workspace);
            return GenericResult<WorkspaceDto>.Success(workspaceDto);
        }

        public async Task<GenericResult<WorkspaceDto>> GetWorkspaceByUser(int userId)
        {
            var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByUser(userId);
            if(workspace == null)
            {
                return GenericResult<WorkspaceDto>.Failure("Not Authorize to this workspace or workspace not found");

            }
            var workspaceDto = _mapper.Map<WorkspaceDto>(workspace);
            return GenericResult<WorkspaceDto>.Success(workspaceDto);

        }

        public async Task<Result> UpdateWorkspace(WorkspaceDto workspaceDto, int userId)
        {
            if (workspaceDto == null)
            {
                return Result.Failure("Invalid workspace data.");
            }
            if (workspaceDto.Id < 0)
            {
                return Result.Failure("Invalid Id");
            }

            bool workspaceExists = await WorkspaceExists(workspaceDto.Name);
            if (workspaceExists)
            {
                return Result.Failure("Invalid workspace Name.");
            }

            var Updatedworkspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAndUserId(workspaceDto.Id, userId);
            
            if (Updatedworkspace == null)
            {
                return Result.Failure("workspace not found");
            }

            string oldPath = GetWorkspacePath(Updatedworkspace.Name);
            string newPath = GetWorkspacePath(workspaceDto.Name);

            if (!UpdatePhysicalFolder(oldPath, newPath))
            {
                return Result.Failure("Failed to Update physical folder");
            }

            _mapper.Map(workspaceDto, Updatedworkspace);
            _unitOfWork.WorkspaceRepository.Update(Updatedworkspace);
            var result = _unitOfWork.Save() > 0;
            if (result)
                return Result.Success();
            else
                return Result.Failure("Failed to Update logical folder");
                   
        }
            

        public async Task<bool> WorkspaceExists(string name)
        {
            var workspaceExists = await _unitOfWork.WorkspaceRepository.GetWorkspaceByName(name);
            if (workspaceExists != null)
                return true;
            else
                return false;
        }

    }
}

        //public async Task<bool> CreateWorkspace(Workspace workspace)
        //{
        //    if(workspace!= null)
        //    {

        //        //var workspaceExists = (await _unitOfWork.WorkspaceRepository.GetAll()).FirstOrDefault(w => w.Name == workspace.Name); 
        //        var workspaceExists = _unitOfWork.WorkspaceRepository.GetWorkspaceByName(workspace.Name);

        //        if (workspaceExists == null)
        //        {
        //            await _unitOfWork.WorkspaceRepository.Add(workspace);
        //            return  _unitOfWork.Save() > 0;
        //        }

        //    }
        //    return false;
        //}

        //    public async Task<bool> DeleteWorkspace(int id)
        //    {

        //        var workspace = await _unitOfWork.WorkspaceRepository.GetById(id);
        //        if (workspace != null)
        //             _unitOfWork.WorkspaceRepository.Delete(workspace);

        //        return _unitOfWork.Save() > 0 ? true : false;
        //    }


        //    public async Task<IEnumerable<WorkspaceDto>> GetAllWorkspaces();
        //    {
        //        var workspace = await 

        //    }

        //    public async Task<WorkspaceDto> GetWorkspaceById(int id);
        //    {
        //        return await _unitOfWork.WorkspaceRepository.GetById(id);
        //    }

        //    public async UpdateWorkspace(WorkspaceDto workspaceDto);
        //    {
        //        if (workspace != null)
        //        {
        //            var workspace1 = await _unitOfWork.WorkspaceRepository.GetById(workspace.Id);
        //            if (workspace1 != null)
        //            {
        //                workspace1.Name = workspace.Name;
        //                _unitOfWork.WorkspaceRepository.Update(workspace1);
        //                return  _unitOfWork.Save() > 0 ;
        //            }
        //        }
        //        return false;
        //    }

        //    public async Task<bool> WorkspaceExists(string name)
        //    {
        //        var workspaceExists = await _unitOfWork.WorkspaceRepository.GetWorkspaceByName(name);
        //        if (workspaceExists != null)
        //            return true;
        //        else
        //            return false;
        //    }
        //}
    
