using Application.Extensions;
using Domain.Enums;
using Domain.Interface;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;


namespace Infrastructure.Repository
{
    public class FolderRepository : GenericRepository<Folder>, IFolderRepository
    {
        private readonly DataContext context;
        private bool isAdmin = false; 
        public FolderRepository(DataContext Context , IHttpContextAccessor httpContext) : base(Context)
        {
            this.context = Context;
            isAdmin = httpContext.HttpContext.User.IsAdmin();
        }

        public override async Task<IEnumerable<Folder>> GetAll()
        {
            return await _context.Folders.Where(f => f.IsDeleted == false).ToListAsync();
        }

        public async Task<Folder> GetFolderById(int id , int userId)
        {
            return await _context.Folders.Where(f => (f.Id == id && f.IsDeleted == false )&& (f.Workspace.UserId== userId || isAdmin)).FirstOrDefaultAsync();
        }



        public async Task<IEnumerable<Folder>> GetPublicFolders(int userId)
        {
            return await _context.Folders.Include(f=> f.Workspace)
                .Where(f => f.IsPublic == true && f.IsDeleted == false && f.Workspace.UserId != userId).ToListAsync();
        }

        public override void Delete(Folder folder)
        {

            folder.IsDeleted = true;
            Update(folder);
        }

        public async Task<IEnumerable<Folder>> GetFoldersByWorkspace(int workspaceId, int userId)
        {
            return await _context.Folders.Where(f => f.WorkspaceId == workspaceId && f.IsDeleted == false &&((f.Workspace.UserId==userId) || isAdmin) ).ToListAsync();
        }

        public async Task<Folder> GetFolderByWorkspace(int workspaceId, string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
                return null;

            return await _context.Folders.Where(f => f.WorkspaceId == workspaceId && f.Name == folderName && f.IsDeleted == false).FirstOrDefaultAsync();
        }

        public async Task<Folder> GetFolderByWorkspace(int workspaceId)
        {
            return await _context.Folders.Where(f => f.WorkspaceId == workspaceId && f.IsDeleted == false).FirstOrDefaultAsync();
        }

        
    }
}
