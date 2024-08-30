using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IFolderRepository :IGenericRepository<Folder>
    {
        Task<IEnumerable<Folder>> GetPublicFolders(int userId);
        Task<IEnumerable<Folder>> GetAll();
        Task<Folder> GetFolderById(int id, int userId);
        void Delete(Folder folder);
        Task<IEnumerable<Folder>> GetFoldersByWorkspace(int workspaceId , int userId);
        Task<Folder> GetFolderByWorkspace(int workspaceId, string folderName);
        Task<Folder> GetFolderByWorkspace(int workspaceId);


    }
}
