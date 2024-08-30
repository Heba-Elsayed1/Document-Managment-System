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
       
        Task<IEnumerable<FolderDto>> GelAllFolders();
        Task<IEnumerable<FolderDto>> GetPublicFolders(int userId);
        Task<FolderDto> GetFolderById (int id , int userId);
        Task<IEnumerable<FolderDto>> GetFoldersByWorkspace (int workspaceId, int userId);
        Task<bool> CreateFolder(FolderDto folderDto, int userId);
        Task<bool> UpdateFolder(FolderDto folderDto, int userId);
        Task<bool> DeleteFolder(int id, int userId);
      


    }
}
