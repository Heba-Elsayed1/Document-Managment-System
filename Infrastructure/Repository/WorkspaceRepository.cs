using Application.Extensions;
using Domain.Interface;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class WorkspaceRepository : GenericRepository<Workspace>, IWorkspaceRepository
    {
        public readonly DataContext context;
        private bool isAdmin = false;
        public WorkspaceRepository(DataContext context , IHttpContextAccessor httpContext) : base(context)
        {
            this.context = context;
            isAdmin = httpContext.HttpContext.User.IsAdmin();
        }

        public async Task<Workspace> GetWorkspaceByUser(int userId)
        {
            return await _context.Workspaces.Where(w => w.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<Workspace> GetWorkspaceByName(string name)
        {
            return await _context.Workspaces.Where(w => w.Name == name).FirstOrDefaultAsync();
                
            
        }

        public async Task<Workspace> GetWorkspaceByIdAndUserId(int workspaceId, int userId)
        {
            return await _context.Workspaces.FirstOrDefaultAsync(w => (w.Id == workspaceId && ((w.UserId == userId) ||isAdmin)) );
        }
    }
}
