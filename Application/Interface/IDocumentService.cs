using Application.Common;
using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IDocumentService
    {   Task<GenericResult<IEnumerable<DocumentDto>>> GetDocumentsOfWorkspace
            (int userId, int workspaceId, string documentName = null, string documentType = null, DateTime? creationDate = null);
        Task<GenericResult<IEnumerable<DocumentDto>>> GetDocumentsByFolder(int FolderId, int userId, int pageNumber, int pageSize);
        Task<Result> IsUserDocument(int FolderId, int userId);
        Task<GenericResult<DocumentDto>> GetDocumentMetadata(int id, int userId);
        Task<Result> CreateDocument(DocumentUploadDto document, int userId);
        Task<Result> DeleteDocument(int id,int userId);
        Task<Result> UpdateDocumentName(int documentId, string name, int userId);
        Task<GenericResult<(byte[] fileBytes, string mimiType, string documentName)>> DownloadDocument(int id, int userId);
        Task<GenericResult<string>> GetDocumentPath(int id, int userId);
        string GetMimeType(string fileExtension);
        

    }
}
