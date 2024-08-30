using Application.Dto;
using Application.Interface;
using Application.Service;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WepApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentController : BaseController
    {
        private readonly IDocumentService _documentService;


        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }


        [HttpGet("workspace/{workspaceId}")]
        public async Task<IActionResult> GetDocumentOfWorkspace(int workspaceId,
        [FromQuery] string name = null,
        [FromQuery] string type = null,
        [FromQuery] DateTime? creationDate = null)
        {
            int userId = GetUserId();
            var documents = await _documentService.GetDocumentOfWorkspace(userId, workspaceId, name, type, creationDate);
            if (documents != null)
                return Ok(documents);
            else
                return BadRequest("No documents found for the specified data");
        }


        [HttpGet("Folder/{folderId}")]
        public async Task<IActionResult> GetDocumentsByFolder (int folderId)
        {
            int userId = GetUserId();
            var documents = await _documentService.GetDocumentsByFolder(folderId, userId);
            if (documents != null)
                return Ok(documents);
            else
                return BadRequest("No documents found.");
        }


        [HttpGet("Metadata/{documentId}")]
        public async Task<IActionResult> GetDocumentMetadata(int documentId)
        {
            int userId = GetUserId();
            var documents = await _documentService.GetDocumentMetadata(documentId,userId);
            if (documents != null)
                return Ok(documents);
            else
                return BadRequest("No documents found.");

        }

        [Authorize(Roles = "User")]
        [HttpGet ("Preview/{documentId}")]
        public async Task<IActionResult> GetDocumentPath(int documentId)
        {
            int userId = GetUserId();
            var documentPath = await _documentService.GetDocumentPath(documentId , userId);
            if (!string.IsNullOrEmpty(documentPath))
               return Ok(documentPath);
            else
                return BadRequest("Not Authorized");
        }


        [Authorize(Roles ="User")]
        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteDocument([FromRoute] int documentId)
        {
            int userId = GetUserId();
            var isDeleted = await _documentService.DeleteDocument(documentId,userId);
            if (isDeleted)
                return Ok("Document is deleted successfully");
            else
                return BadRequest("Deletion failed");
        }


        [Authorize(Roles = "User")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] DocumentUploadDto document)
        {
            int userId = GetUserId();

            if (document.File == null || document.File.Length == 0)
            {
                return BadRequest("File not provided");
            }

            bool isCreated = await _documentService.CreateDocument(document, userId);
            if (isCreated)
                return Ok("Document uploaded successfully");
            else
                return BadRequest("Failed to upload document");
        }


        [Authorize(Roles = "User")]
        [HttpGet("download")]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            int userId = GetUserId();
            var (isDownloaded, message, fileBytes, mimeType, documentName) = await _documentService.DownloadDocument(documentId, userId);

            if (!isDownloaded)
            {
                return NotFound(message);
            }
               
             return File(fileBytes, mimeType, documentName);
        }


        [Authorize(Roles = "User")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDocument (int documentId ,string documentName)
        {
            int userId = GetUserId();
            if (string.IsNullOrEmpty(documentName))
                return BadRequest("Name not Entered");
            var isUpdated = await _documentService.UpdateDocumentName(documentId, documentName, userId);
            if (isUpdated)
                return Ok("Document name updated sucessfully");
            else
                return BadRequest("Failed to Update Document");
        }


        //[HttpPut("Restore/{documentId}")]
        //public async Task<IActionResult> RestoreDocument([FromRoute] int documentId)
        //{
        //    string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    //int userIdInt = int.Parse(userId);

        //    var isDeleted = await _documentService.RestoreDocument(documentId);
        //    if (isDeleted)
        //        return Ok("Document is Restored successfully");
        //    else
        //        return BadRequest("Restoring failed");
        //}

        //[HttpGet("SharedDocuments")]
        //public async Task<IActionResult> GetSharedDocuments()
        //{
        //    int userId = GetUserId();
        //    var documents = await _documentService.GetSharedDocuments(userId);
        //    if (documents != null)
        //        return Ok(documents);
        //    else
        //        return BadRequest("No shared documents found.");

        //}

    }
}
