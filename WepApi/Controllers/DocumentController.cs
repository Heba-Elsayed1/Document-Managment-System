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
            var documents = await _documentService.GetDocumentsOfWorkspace(userId, workspaceId, name, type, creationDate);
            if (documents.IsSuccess)
                return Ok(documents.Value);
            else
                return BadRequest(documents.ErrorMessage);
        }


        [HttpGet("Folder/{folderId}")]
        public async Task<IActionResult> GetDocumentsByFolder (int folderId)
        {
            int userId = GetUserId();
            var documents = await _documentService.GetDocumentsByFolder(folderId, userId);
            if (documents.IsSuccess)
                return Ok(documents.Value);
            else
                return BadRequest(documents.ErrorMessage);
        }


        [HttpGet("Metadata/{documentId}")]
        public async Task<IActionResult> GetDocumentMetadata(int documentId)
        {
            int userId = GetUserId();
            var documents = await _documentService.GetDocumentMetadata(documentId,userId);
            if (documents.IsSuccess)
                return Ok(documents.Value);
            else
                return BadRequest(documents.ErrorMessage); ;

        }


        [Authorize(Roles = "User")]
        [HttpGet("Preview/{documentId}")]
        public async Task<IActionResult> PreviewDocument(int documentId)
        {
            int userId = GetUserId();
            var result = await _documentService.DownloadDocument(documentId, userId); 

            if (result.IsSuccess)
            {
                var (fileBytes, mimeType, documentName) = result.Value;
                return File(fileBytes, mimeType, documentName);
            }

            return BadRequest(result.ErrorMessage);
        }


        [Authorize(Roles ="User")]
        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteDocument([FromRoute] int documentId)
        {
            int userId = GetUserId();
            var deleted = await _documentService.DeleteDocument(documentId,userId);
            if (deleted.IsSuccess)
                return Ok();
            else
                return BadRequest(deleted.ErrorMessage);
        }


        [Authorize(Roles = "User")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] DocumentUploadDto document)
        {
            int userId = GetUserId();
            var Created = await _documentService.CreateDocument(document, userId);
            if (Created.IsSuccess)
                return Ok();
            else
                return BadRequest(Created.ErrorMessage);
        }


        [Authorize(Roles = "User")]
        [HttpGet("download/{documentId}")]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            int userId = GetUserId();
            var download = await _documentService.DownloadDocument(documentId, userId);
            if (download.IsSuccess)
            {
                return File(download.Value.fileBytes, download.Value.mimiType, download.Value.documentName);
                
            }
            return NotFound(download.ErrorMessage);
        }


        [Authorize(Roles = "User")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDocument (int documentId ,string documentName)
        {
            int userId = GetUserId();
            var updated = await _documentService.UpdateDocumentName(documentId, documentName, userId);
            if (updated.IsSuccess)
                return Ok();
            else
                return BadRequest(updated.ErrorMessage);
        }

    }
}
