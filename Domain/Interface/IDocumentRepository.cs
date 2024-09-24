using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        Task<IEnumerable<Document>> GetDocumentsByWorkspace
             (int userId, int workspaceId, string documentName = null, string documentType = null, DateTime? creationDate = null);
        Task<Document> GetDocumentByUser(int userId, int documentId);
        Task<Document> GetDocumentToDownload(int id, int userId);
        Task<IEnumerable<Document>> GetDocumentsByFolder(int FolderId, int userId);
        Task<Document> GetDocumentByUserAndFolder(int FolderId, int userId);
        void Delete(Document document);
      

    }
}
