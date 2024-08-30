using Application.Dto;
using Application.Extensions;
using Application.Interface;
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
        private readonly IWebHostEnvironment _environment;
        private readonly IWorkspaceService _workspaceService;

        public FolderController(IFolderService folderService , IWebHostEnvironment environment ,IWorkspaceService workspaceService )
        {
            _folderService = folderService;
            _environment = environment;
            _workspaceService = workspaceService;
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> GetFolders()
        {
            var folders = await _folderService.GelAllFolders();

            if (folders != null)
                return Ok(folders);
            else
                return BadRequest("Not Authorized");
        }

        
        [HttpGet("isPublic")]
        public async Task<IActionResult> GetPublicFolders()
        {
            
            int userId = GetUserId();
            var folders = await _folderService.GetPublicFolders(userId);

            if (folders != null)
                return Ok(folders);
            else
                return BadRequest();
        }


        [HttpGet("{folderId}")]
        public async Task<IActionResult> GetFolderById(int folderId)
        {
            int userId = GetUserId();

            var folder = await _folderService.GetFolderById(folderId , userId);

            if (folder != null)
                return Ok(folder);
            else
                return BadRequest("Folder not found");
        }


        [HttpGet("workspace/{workspaceId:int}")]
        public async Task<IActionResult> GetFoldersByWorkspace(int workspaceId)
        {
            int userId = GetUserId();

            var folders = await _folderService.GetFoldersByWorkspace(workspaceId , userId);

            if (folders != null)
                return Ok(folders);
            else
                return BadRequest("Invalid Workspace");
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateFolder(string folderName, bool isPublic)
        {

            int userId = GetUserId();

            var folderDto = new FolderDto
            {
                Name = folderName,
                IsPublic = isPublic
            };


            bool isCreated = false;
            isCreated = await _folderService.CreateFolder(folderDto, userId);
            
            if (isCreated) 
            {

                return Ok("Folder is created successfully");
            }
            else
                return BadRequest("Failed to create folder");
        }

        [Authorize(Roles = "User")]
        [HttpPut]
        public async Task<IActionResult> UpdateFolder([FromBody] FolderDto folderDto)
        {
            if (folderDto != null)
            {
                int userId = GetUserId();

                bool isUpdated = false;
                isUpdated = await _folderService.UpdateFolder(folderDto, userId);

                if (isUpdated)
                    return Ok("Folder is Updated successfully");

            }
            return NotFound();
            

        }

        [Authorize(Roles = "User")]
        [HttpDelete("{folderId}")]
        public async Task<IActionResult> DeleteFolder(int folderId)
        {
            int userId = GetUserId();

            bool isDeleted = await _folderService.DeleteFolder(folderId, userId);

            if (isDeleted)
                return Ok("Folder is Deleted successfully");
            else
                return NotFound("Deleted is Failed");
        }




    }
}
