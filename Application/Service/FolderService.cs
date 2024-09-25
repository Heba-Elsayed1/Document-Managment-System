using Application.Common;
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
        public async Task<Result> CreateFolder(string folderName, bool isPublic, int userId)
        {
            if (string.IsNullOrEmpty(folderName))
                return Result.Failure("Invalid empty name");

            var folderDto = new FolderDto
            {
                Name = folderName,
                IsPublic = isPublic
            };

            if (folderDto == null)
            {
                return Result.Failure("Invalid empty data");

            }

            var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByUser(userId);
            if (workspace == null)
            {
                return Result.Failure("Workspace not found");
            }

            var existingFolder = await _unitOfWork.FolderRepository.GetFolderByWorkspace(workspace.Id, folderDto.Name);
            if (existingFolder != null)
            {
                return Result.Failure("Folder not found");
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
            var result = _unitOfWork.Save() > 0;
            if (result)
                return Result.Success();
            else
                return Result.Failure("Falied to create folder");
        }

        public async Task<Result> DeleteFolder(int id, int userId)
        {
            if (id < 0)
                return Result.Failure("Invalid Id");

            var folder = await _unitOfWork.FolderRepository.GetFolderById(id,userId);

            if (folder != null )
                _unitOfWork.FolderRepository.Delete(folder);

            var result = _unitOfWork.Save() > 0;
            if (result)
                return Result.Success();
            else
                return Result.Failure("Falied to delete folder");
        }

        public async Task<GenericResult<IEnumerable<FolderDto>>> GelAllFolders()
        {
            var folders = await _unitOfWork.FolderRepository.GetAll();
            if (folders == null)
                return GenericResult<IEnumerable<FolderDto>>.Failure("Folders not found");

            var foldersDto = _mapper.Map<List<FolderDto>>(folders);
            return GenericResult<IEnumerable<FolderDto>>.Success(foldersDto);
        }

        public async Task<GenericResult<FolderDto>> GetFolderById(int id, int userId)
        {
            if (id < 0)
                return GenericResult<FolderDto>.Failure("Invalid Id");

            var folder = await _unitOfWork.FolderRepository.GetFolderById(id,userId);
            if (folder == null)
                return GenericResult<FolderDto>.Failure("Folder not found");

            var folderDto = _mapper.Map<FolderDto>(folder);
            return GenericResult<FolderDto>.Success(folderDto);
        }

        public async Task<GenericResult<IEnumerable<FolderDto>>> GetFoldersByWorkspace(int workspaceId, int userId, int pageNumber, int pageSize)
        {
            if (workspaceId < 0)
                return GenericResult<IEnumerable<FolderDto>>.Failure("Invalid Id");

            var folders = await _unitOfWork.FolderRepository.GetFoldersByWorkspace(workspaceId,userId);
            if (folders == null)
                return GenericResult<IEnumerable<FolderDto>>.Failure("Folders not found");

            var paginatedFolders = folders.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var foldersDto = _mapper.Map<List<FolderDto>>(paginatedFolders);
            return GenericResult<IEnumerable<FolderDto>>.Success(foldersDto);

        }

        public async Task<GenericResult<IEnumerable<FolderDto>>> GetPublicFolders(int userId, int pageNumber, int pageSize)
        {
            var folders = await _unitOfWork.FolderRepository.GetPublicFolders(userId);
            if (folders == null)
                return GenericResult<IEnumerable<FolderDto>>.Failure("Folders not found");

            var paginatedFolders = folders.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var foldersDto = _mapper.Map<List<FolderDto>>(paginatedFolders);
            return GenericResult<IEnumerable<FolderDto>>.Success(foldersDto);
        }

        public async Task<Result> UpdateFolder(FolderDto folderDto, int userId)
        {
            if (folderDto == null)
              return Result.Failure("Invalid empty folder");

            if(folderDto.Id < 0)
             return Result.Failure("Invalid Id");

            var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByUser(userId);
            var folderUpdated = await _unitOfWork.FolderRepository.GetFolderById(folderDto.Id,userId);

            if (folderUpdated == null)
            {
                return Result.Failure("Falied to update folder");
            }
            string FolderOldPath = GetFolderPath(workspace.Name, folderUpdated.Name);
            string FolderNewPath = GetFolderPath(workspace.Name, folderDto.Name);

            if (! UpdatePhysicalFolder(FolderOldPath, FolderNewPath))
            {
                return Result.Failure("Falied to update folder");
            }
            _mapper.Map(folderDto, folderUpdated);
            _unitOfWork.FolderRepository.Update(folderUpdated);
            var result = _unitOfWork.Save() > 0;
            if (result)
                return Result.Success();
            else
                return Result.Failure("Falied to update folder");
            
        }

    }   
}
