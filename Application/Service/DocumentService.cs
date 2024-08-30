using Application.Dto;
using Application.Interface;
using AutoMapper;
using Domain.Interface;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using System.Reflection.Metadata;

namespace Application.Service
{
    public class DocumentService : BaseService, IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DocumentService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment environment) : base(environment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateDocument(DocumentUploadDto document, int userId)
        {
            if (document == null)
                return false;

            var folder = await _unitOfWork.FolderRepository.GetFolderById(document.FolderId, userId);

            if (folder == null)
                return false;

            var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByUser(userId);
            if (workspace == null)
            {
                return false;
            }


            var folderPath = GetFolderPath(workspace.Name, folder.Name);
            var originalFileName = Path.GetFileNameWithoutExtension(document.File.FileName);
            var extension = Path.GetExtension(document.File.FileName);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var uniqueName = $"{originalFileName}_{timestamp}{extension}";
            var filePath = Path.Combine(folderPath, uniqueName);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await document.File.CopyToAsync(stream);
            }


            var documentDto = new DocumentDto
            {
                Name = uniqueName,
                FolderId = document.FolderId,
                Path = filePath,
                Type = extension.TrimStart('.'),
                CreationDate = DateTime.UtcNow
            };

            var newDocument = _mapper.Map<Domain.Models.Document>(documentDto);
            await _unitOfWork.DocumentRepository.Add(newDocument);
            return _unitOfWork.Save() > 0;


        }

        public async Task<bool> DeleteDocument(int id, int userId)
        {
            var document = await _unitOfWork.DocumentRepository.GetDocumentByUser(userId, id);
            if (document != null)
                _unitOfWork.DocumentRepository.Delete(document);

            return _unitOfWork.Save() > 0 ? true : false;
        }

        public async Task<DocumentDto> GetDocumentMetadata(int id, int userId)
        {
            var document = await _unitOfWork.DocumentRepository.GetDocumentByUser(userId, id);
            return _mapper.Map<DocumentDto>(document);
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentOfWorkspace
            (int userId, int workspaceId, string documentName = null, string documentType = null, DateTime? creationDate = null)
        {
            var documents = await _unitOfWork.DocumentRepository.GetDocumentsByWorkspace(userId, workspaceId, documentName, documentType, creationDate);
            return _mapper.Map<List<DocumentDto>>(documents);

        }

        public async Task<string> GetDocumentPath(int id, int userId)
        {
            return await _unitOfWork.DocumentRepository.GetDocumentPath(id, userId);
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentsByFolder(int FolderId, int userId)
        {
            var documents = await _unitOfWork.DocumentRepository.GetDocumentsByFolder(FolderId, userId);
            return _mapper.Map<List<DocumentDto>>(documents);
        }

        public async Task<(bool isDownloaded , string message , byte[] fileBytes , string mimiType , string documentName)> DownloadDocument(int id, int userId)
        {
            var document = await _unitOfWork.DocumentRepository.GetDocumentToDownload(id, userId);

            if (document == null)
            {
                return (false ,"Document Not Found", Array.Empty<byte>(), "","");
            }
            string filePath = await GetDocumentPath(id, userId);

            if (!System.IO.File.Exists(filePath))
            {
                return (false,"File Not Found", Array.Empty<byte>(), "", "");
            }
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var mimiType = GetMimeType(document.Type);

            return (true, "Document downloaded sucessfully",fileBytes,mimiType,document.Name);




        }
        public string GetMimeType(string fileExtension)
        {
            return fileExtension switch
            {
                "jpeg" or "jpg" => "image/jpeg",
                "png" => "image/png",
                "pdf" => "application/pdf",
                "txt" => "text/plain",
                _ => "application/octet-stream",
            };
        }

        public async Task<bool> UpdateDocumentName(int documentId, string name, int userId)
        {
            var document = await _unitOfWork.DocumentRepository.GetDocumentByUser(userId, documentId);
            if (document == null)
                return false;
            
            var folderPath = GetFolderPath(document.Folder.Workspace.Name, document.Folder.Name);
            var oldPath = Path.Combine(folderPath, document.Name);
            var newPath = Path.Combine(folderPath, name);

            document.Name = $"{name}.{document.Type}";

            if (!renameFile(oldPath, newPath,document.Type))
                return false;

            _unitOfWork.DocumentRepository.Update(document);
            return _unitOfWork.Save() > 0;

        }

    }
}
