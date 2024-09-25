using Application.Common;
using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IFolderService
    {

        Task<GenericResult<IEnumerable<FolderDto>>> GelAllFolders();
        Task<GenericResult<IEnumerable<FolderDto>>> GetPublicFolders(int userId, int pageNumber, int pageSize);
        Task<GenericResult<FolderDto>> GetFolderById (int id , int userId);
        Task<GenericResult<IEnumerable<FolderDto>>> GetFoldersByWorkspace (int workspaceId, int userId, int pageNumber, int pageSize);
        Task<Result> CreateFolder(string folderName, bool isPublic, int userId);
        Task<Result> UpdateFolder(FolderDto folderDto, int userId);
        Task<Result> DeleteFolder(int id, int userId);
      
    }
}
