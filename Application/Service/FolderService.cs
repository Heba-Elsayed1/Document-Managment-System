using Application.Dto;
using Application.Interface;
using AutoMapper;
using Domain.Interface;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;

namespace Application.Service
{
    public class FolderService : BaseService, IFolderService 
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FolderService(IUnitOfWork unitOfWork, IMapper mapper , IWebHostEnvironment environment):base(environment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            

        }
        public async Task<bool> CreateFolder(FolderDto folderDto, int userId)
        {
            if (folderDto == null)
            {
                return false;

            }

            var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByUser(userId);
            if (workspace == null)
            {
                return false;
            }

            var existingFolder = await _unitOfWork.FolderRepository.GetFolderByWorkspace(workspace.Id, folderDto.Name);
            if (existingFolder != null)
            {
                return false;
            }

            string folderPath = GetFolderPath(workspace.Name, folderDto.Name);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var newFolder = _mapper.Map<Folder>(folderDto);
            newFolder.WorkspaceId = workspace.Id;
            newFolder.IsDeleted = false;
            await _unitOfWork.FolderRepository.Add(newFolder);
            return _unitOfWork.Save() > 0;
        }

        public async Task<bool> DeleteFolder(int id, int userId)
        {
            var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByUser(userId);
            var folder = await _unitOfWork.FolderRepository.GetFolderByWorkspace(workspace.Id);

            if (folder != null )
                _unitOfWork.FolderRepository.Delete(folder);

            return _unitOfWork.Save() > 0 ? true : false;
        }

        public async Task<IEnumerable<FolderDto>> GelAllFolders()
        {

            var folders = await _unitOfWork.FolderRepository.GetAll();
            return _mapper.Map<List<FolderDto>>(folders);

        }

        public async Task<FolderDto> GetFolderById(int id, int userId)
        {
            var folder = await _unitOfWork.FolderRepository.GetFolderById(id, userId);
            return _mapper.Map<FolderDto>(folder);
        }

        public async Task<IEnumerable<FolderDto>> GetFoldersByWorkspace(int workspaceId, int userId)
        {
                var folders = await _unitOfWork.FolderRepository.GetFoldersByWorkspace(workspaceId,userId);
                return _mapper.Map<List<FolderDto>>(folders);

        }

        public async Task<IEnumerable<FolderDto>> GetPublicFolders(int userId)
        {
            var folders = await _unitOfWork.FolderRepository.GetPublicFolders(userId);
            return _mapper.Map<List<FolderDto>>(folders);
        }

        public async Task<bool> UpdateFolder(FolderDto folderDto, int userId)
        {
            var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByUser(userId);
            var folderUpdated = await _unitOfWork.FolderRepository.GetFolderByWorkspace(workspace.Id);

            if (folderUpdated != null )
            {

                string FolderOldPath = GetFolderPath(workspace.Name, folderUpdated.Name);
                string FolderNewPath = GetFolderPath(workspace.Name, folderDto.Name);

                if (! updatePhysicalFolder(FolderOldPath, FolderNewPath))
                {
                    return false;
                }
                _mapper.Map(folderDto, folderUpdated);
                _unitOfWork.FolderRepository.Update(folderUpdated);
                return _unitOfWork.Save() > 0;

            }

            return false;

        }




    }


    
}
