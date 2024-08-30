using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IDocumentService
    {   Task<IEnumerable<DocumentDto>> GetDocumentOfWorkspace
            (int userId, int workspaceId, string documentName = null, string documentType = null, DateTime? creationDate = null);
        Task<bool> DeleteDocument(int id,int userId);
        Task<bool> CreateDocument (DocumentUploadDto document,int userId);
        Task<DocumentDto> GetDocumentMetadata(int id,int userId);
        Task<(bool isDownloaded, string message, byte[] fileBytes, string mimiType, string documentName)> DownloadDocument(int id, int userId);
        Task<IEnumerable<DocumentDto>> GetDocumentsByFolder(int FolderId, int userId);
        Task<string> GetDocumentPath(int id, int userId);
        string GetMimeType(string fileExtension);
        Task<bool> UpdateDocumentName (int documentId ,string name , int userId);


        //Task<bool> UpdateDocument(DocumentUploadDto document);
        //Task<bool> RestoreDocument(int id);

        //Task<bool> UpdateDocument (Document document);




    }
}
