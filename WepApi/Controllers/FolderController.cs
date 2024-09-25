using Application.Dto;
using Application.Extensions;
using Application.Interface;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WepApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FolderController : BaseController
    {
        private readonly IFolderService _folderService;
        
        public FolderController(IFolderService folderService  )
        {
            _folderService = folderService;
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> GetFolders()
        {
            var folders = await _folderService.GelAllFolders();

            if (folders.IsSuccess)
                return Ok(folders.Value);
            else
                return BadRequest(folders.ErrorMessage);
        }

        
        [HttpGet("isPublic")]
        public async Task<IActionResult> GetPublicFolders(int pageNumber = 1, int pageSize = 10)
        {
            
            int userId = GetUserId();
            var folders = await _folderService.GetPublicFolders(userId, pageNumber, pageSize);

            if (folders.IsSuccess)
                return Ok(folders.Value);
            else
                return BadRequest(folders.ErrorMessage);
        }


        [HttpGet("{folderId}")]
        public async Task<IActionResult> GetFolderById(int folderId)
        {
            int userId = GetUserId();
            var folder = await _folderService.GetFolderById(folderId , userId);

            if (folder.IsSuccess)
                return Ok(folder.Value);
            else
                return BadRequest(folder.ErrorMessage);
        }


        [HttpGet("workspace/{workspaceId:int}")]
        public async Task<IActionResult> GetFoldersByWorkspace(int workspaceId, int pageNumber = 1, int pageSize = 10)
        {
            int userId = GetUserId();
            var folders = await _folderService.GetFoldersByWorkspace(workspaceId , userId , pageNumber, pageSize);

            if (folders.IsSuccess)
                return Ok(folders.Value);
            else
                return BadRequest(folders.ErrorMessage);
        }


        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateFolder(string folderName, bool isPublic)
        {

            int userId = GetUserId();
            var Created = await _folderService.CreateFolder(folderName, isPublic, userId);
            
            if (Created.IsSuccess) 
                return Ok();
            else
                return BadRequest(Created.ErrorMessage);
        }


        [Authorize(Roles = "User")]
        [HttpPut]
        public async Task<IActionResult> UpdateFolder([FromBody] FolderDto folderDto)
        {
                int userId = GetUserId();
                var updated = await _folderService.UpdateFolder(folderDto, userId);

                if (updated.IsSuccess)
                    return Ok();
                else
                  return NotFound(updated.ErrorMessage);
        }

        [Authorize(Roles = "User")]
        [HttpDelete("{folderId}")]
        public async Task<IActionResult> DeleteFolder(int folderId)
        {
            int userId = GetUserId();
            var deleted = await _folderService.DeleteFolder(folderId, userId);

            if (deleted.IsSuccess)
                return Ok();
            else
                return NotFound(deleted.ErrorMessage);
        }

    }
}
