using Application.Extensions;
using Domain.Interface;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        public readonly DataContext context;
        private bool isAdmin = false;

        public DocumentRepository(DataContext Context, IHttpContextAccessor httpContext) : base(Context)
        {
            context = Context;
            isAdmin = httpContext.HttpContext.User.IsAdmin();

        }

        public async Task<IEnumerable<Document>> GetDocumentsByWorkspace
             (int userId, int workspaceId, string documentName = null, string documentType = null, DateTime? creationDate = null)
        {
            var query = context.Documents.AsQueryable();
            if (workspaceId > 0)
            {
                query = query.Where(d => d.Folder.WorkspaceId == workspaceId
                                    && d.IsDeleted == false
                                    && (d.Folder.Workspace.UserId == userId || isAdmin));
            }

            if (!string.IsNullOrEmpty(documentName))
                query = query.Where(d => d.Name == documentName);

            if (!string.IsNullOrEmpty(documentType))
                query = query.Where(d => d.Type == documentType);

            if (creationDate.HasValue)
                query = query.Where(d => d.CreationDate == creationDate);

            return await query.ToListAsync();

        }

        public override void Delete(Document document)
        {
            document.IsDeleted = true;
            Update(document);
        }

        public async Task<Document> GetDocumentByUser(int userId, int documentId)
        {
            var document = await _context.Documents
           .Include(d => d.Folder)
           .ThenInclude(f => f.Workspace)
           .Where(d => d.Id == documentId
               && (d.Folder.Workspace.UserId == userId || isAdmin || d.Folder.IsPublic == true)
               && d.Folder.IsDeleted == false
               && d.IsDeleted == false
               )
           .FirstOrDefaultAsync();

            return document;
        }

        public async Task<Document> GetDocumentToDownload(int id, int userId)
        {
            var document = await _context.Documents
            .Include(d => d.Folder)
            .ThenInclude(f => f.Workspace)
            .Where(d => d.Id == id
                && (d.Folder.Workspace.UserId == userId || d.Folder.IsPublic == true || isAdmin)
                && d.Folder.IsDeleted == false
                && d.IsDeleted == false
                )
            .FirstOrDefaultAsync();

            return document;
        }

        public async Task<IEnumerable<Document>> GetDocumentsByFolder(int FolderId, int userId)
        {
            var documents = await _context.Documents
            .Include(d => d.Folder)
            .ThenInclude(d => d.Workspace)
            .Where(d => d.IsDeleted == false
                && (d.FolderId == FolderId)
                && (d.Folder.Workspace.UserId == userId || d.Folder.IsPublic == true || isAdmin)
                && d.Folder.IsDeleted == false
                ).ToListAsync();

            if (!documents.Any())
            {
                var folder = await _context.Folders
                    .Include(f => f.Workspace)
                    .FirstOrDefaultAsync(f => f.Id == FolderId && !f.IsDeleted);

                if (folder == null || (folder.Workspace.UserId != userId && !folder.IsPublic && !isAdmin))
                {
                    return null;
                }
            }

            return documents;
        }
    
        public async Task<Document> GetDocumentByUserAndFolder(int FolderId, int userId)
        {
            var document = await _context.Documents
            .Include(d => d.Folder)
            .ThenInclude(d => d.Workspace)
            .Where(d => d.IsDeleted == false
                && (d.FolderId == FolderId)
                && (d.Folder.Workspace.UserId == userId)
                && d.Folder.IsDeleted == false
                ).FirstOrDefaultAsync();

            return document;
        }
        
    }
}
