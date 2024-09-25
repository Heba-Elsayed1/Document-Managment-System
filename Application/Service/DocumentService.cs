using Application.Common;
using Application.Dto;
using Application.Interface;
using AutoMapper;
using Domain.Interface;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Xml.Linq;

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

        public async Task<Result> CreateDocument(DocumentUploadDto document, int userId)
        {
            if (document.File == null || document.File.Length == 0)
            {
                return Result.Failure("File not provided");
            }

            if (document == null)
                return Result.Failure("document not found");

            var folder = await _unitOfWork.FolderRepository.GetFolderById(document.FolderId, userId);

            if (folder == null)
                return Result.Failure("folder not found");

            var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByUser(userId);
            if (workspace == null)
            {
                return Result.Failure("workspace not found");
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
                Type = extension.TrimStart('.'),
                CreationDate = DateTime.UtcNow
            };

            var newDocument = _mapper.Map<Domain.Models.Document>(documentDto);
            await _unitOfWork.DocumentRepository.Add(newDocument);
            var result = _unitOfWork.Save() > 0;
            if (result)
                return Result.Success();
            else
                return Result.Failure("Falied to create document");
        }

        public async Task<Result> DeleteDocument(int id, int userId)
        {
            if (id < 0)
                return Result.Failure("Invalid Id");

            var document = await _unitOfWork.DocumentRepository.GetDocumentByUser(userId, id);
            if (document != null)
                _unitOfWork.DocumentRepository.Delete(document);

            var result = _unitOfWork.Save() > 0;
            if (result)
                return Result.Success();
            else
                return Result.Failure("Falied to delete document");
        }

        public async Task<GenericResult<DocumentDto>> GetDocumentMetadata(int id, int userId)
        {
            if (id < 0)
                return GenericResult<DocumentDto>.Failure("Invalid Id");

            var document = await _unitOfWork.DocumentRepository.GetDocumentByUser(userId, id);
            if (document == null)
            {
                return GenericResult<DocumentDto>.Failure("Document not found");
            }
            var documentDto = _mapper.Map<DocumentDto>(document);
            return GenericResult<DocumentDto>.Success(documentDto);
        }

        public async Task<GenericResult<IEnumerable<DocumentDto>>> GetDocumentsOfWorkspace
            (int userId, int workspaceId, string documentName = null, string documentType = null, DateTime? creationDate = null)
        {
            if (workspaceId < 0)
                return GenericResult<IEnumerable<DocumentDto>>.Failure("Invalid Workspace Id");

            var documents = await _unitOfWork.DocumentRepository.GetDocumentsByWorkspace(userId, workspaceId, documentName, documentType, creationDate);
            if(documents == null)
            {
                return GenericResult<IEnumerable<DocumentDto>>.Failure("Documents not found");
            }
            var documentDto = _mapper.Map<List<DocumentDto>>(documents);
            return GenericResult<IEnumerable<DocumentDto>>.Success(documentDto);
        }

        public async Task<GenericResult<string>> GetDocumentPath(int id, int userId)
        {
            if (id < 0)
                return GenericResult<string>.Failure("Invalid Id");

            var docuemnt = await _unitOfWork.DocumentRepository.GetDocumentByUser(userId, id);
            if (docuemnt == null)
            {

                return GenericResult<string>.Failure("Can't find path");
            }

            else
            {
                var folderPath = GetFolderPath(docuemnt.Folder.Workspace.Name, docuemnt.Folder.Name);
                var path = Path.Combine(folderPath, docuemnt.Name);
                return GenericResult<string>.Success(path);
            }

        }

        public async Task<GenericResult<IEnumerable<DocumentDto>>> GetDocumentsByFolder(int FolderId, int userId, int pageNumber, int pageSize)
        {
            if (FolderId < 0)
                return GenericResult<IEnumerable<DocumentDto>>.Failure("Invalid Folder Id");

            var documents = await _unitOfWork.DocumentRepository.GetDocumentsByFolder(FolderId, userId);
            if (documents == null)
            {
                return GenericResult<IEnumerable<DocumentDto>>.Failure("Unauthorized access or invalid folder");
            }
            if (!documents.Any())
            {
                return GenericResult<IEnumerable<DocumentDto>>.Success(Enumerable.Empty<DocumentDto>());
            }

            var paginatedDocuments = documents.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var documentDto = _mapper.Map<List<DocumentDto>>(paginatedDocuments);
            return GenericResult<IEnumerable<DocumentDto>>.Success(documentDto);
        }


        public async Task<GenericResult<(byte[] fileBytes, string mimiType, string documentName)>> DownloadDocument(int id, int userId)
        {
            var document = await _unitOfWork.DocumentRepository.GetDocumentToDownload(id, userId);

            if (document == null)
            {
                return GenericResult<(byte[] fileBytes, string mimiType, string documentName)>.Failure("Document Not Found")  ;
            }
            var path = await GetDocumentPath(id, userId);
            string filePath = path.Value;

            if (!System.IO.File.Exists(filePath))
            {
                return GenericResult<(byte[] fileBytes, string mimiType, string documentName)>.Failure("File Not Found");
            }
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var mimiType = GetMimeType(document.Type);

            return GenericResult<(byte[] fileBytes, string mimiType, string documentName)>.Success((fileBytes, mimiType, document.Name));
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

        public async Task<Result> UpdateDocumentName(int documentId, string name, int userId)
        {
            if (documentId < 0)
                return Result.Failure("Invalid Id");

            if (string.IsNullOrEmpty(name))
                return Result.Failure("Name not Entered");

            var document = await _unitOfWork.DocumentRepository.GetDocumentByUser(userId, documentId);
            if (document == null)
                return Result.Failure("document not found");
            
            var folderPath = GetFolderPath(document.Folder.Workspace.Name, document.Folder.Name);
            var oldPath = Path.Combine(folderPath, document.Name);
            var newPath = Path.Combine(folderPath, name);

            document.Name = $"{name}.{document.Type}";
            //document.Path = $"{newPath}.{document.Type}";

            if (!renameFile(oldPath, newPath,document.Type))
                return Result.Failure("Falied to update document");

            _unitOfWork.DocumentRepository.Update(document);
            var result = _unitOfWork.Save() > 0;
            if (result)
                return Result.Success();
            else
                return Result.Failure("Falied to update document");
        }

        public async Task<Result> IsUserDocument(int FolderId, int userId)
        {
            if (FolderId < 0)
                return Result.Failure("");

            var document = await _unitOfWork.DocumentRepository.GetDocumentByUserAndFolder(FolderId, userId);
            if (document == null)
            {
                return Result.Failure("");
            }

            return Result.Success();
        }
    }
}
