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


        public async Task<IEnumerable<WorkspaceDto>> GetAllWorkspaces()
        {
            var workspaces = await _unitOfWork.WorkspaceRepository.GetAll();
            return _mapper.Map<List<WorkspaceDto>>(workspaces);
        }

        public async Task<WorkspaceDto> GetWorkspaceById(int id ,int userId)
        {
            var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAndUserId(id, userId);
            return _mapper.Map<WorkspaceDto>(workspace);
            
        }

        public async Task<WorkspaceDto> GetWorkspaceByUser(int userId)
        {
            var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByUser(userId);
            return _mapper.Map<WorkspaceDto>(workspace);
           
        }

        public async Task<bool> UpdateWorkspace(WorkspaceDto workspaceDto, int userId)
        {
            var Updatedworkspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAndUserId(workspaceDto.Id, userId);
            string oldPath = GetWorkspacePath(Updatedworkspace.Name);
            string newPath = GetWorkspacePath(workspaceDto.Name);

            if (Updatedworkspace != null)
            {
               
                if (!updatePhysicalFolder(oldPath, newPath))
                {
                    return false;
                }

                _mapper.Map(workspaceDto, Updatedworkspace);
                _unitOfWork.WorkspaceRepository.Update(Updatedworkspace);
                return _unitOfWork.Save() > 0;

            }
            else
            {
                updatePhysicalFolder(newPath, oldPath);
                return false;
            }
                
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
    
