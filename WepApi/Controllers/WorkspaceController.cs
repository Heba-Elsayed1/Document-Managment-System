using Application.Dto;
using Application.Interface;
using Application.Service;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WepApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkspaceController: BaseController
    {
        private readonly IWorkspaceService _workspaceService;

        public WorkspaceController(IWorkspaceService workspaceService )
        {
            _workspaceService = workspaceService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WorkspaceDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetWorkspaces()
        {
            var workspaces = await _workspaceService.GetAllWorkspaces();

            if (workspaces != null)
                return Ok(workspaces);
            else
                return BadRequest("No workspaces found.");
        }

        [HttpGet("{workspaceid}")]
        [ProducesResponseType(typeof(WorkspaceDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetWorkspacesById (int workspaceid)
        {
            int userId = GetUserId();

            var workspace = await _workspaceService.GetWorkspaceById(workspaceid , userId);
            if (workspace != null)
                return Ok(workspace);
            else
                return BadRequest($"You are not authorized to access this workspace.");
        }

        [Authorize(Roles = "User")]
        [HttpGet("User")]
        [ProducesResponseType(typeof(WorkspaceDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetWorkspacesOfUser()
        {
            int userId = GetUserId();

            var workspace = await _workspaceService.GetWorkspaceByUser(userId);
            if (workspace != null)
                return Ok(workspace);
            else
                return BadRequest($"You are not authorized to access this workspace.");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Admin")]
        public async Task<IActionResult> GetWorkspacesByUser(int userId)
        {

            var workspace = await _workspaceService.GetWorkspaceByUser(userId);
            if (workspace != null)
                return Ok(workspace);
            else
                return BadRequest($"You are not authorized to access this workspace.");
        }


        [Authorize(Roles = "User")]
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateWorkspace( WorkspaceDto workspaceDto)
        {
            if (workspaceDto == null)
            {
                return BadRequest("Invalid workspace data.");
            }

            bool workspaceExists = await _workspaceService.WorkspaceExists(workspaceDto.Name);
            if (workspaceExists)
            {
                return BadRequest("Invalid workspace Name.");
            }

            int userId = GetUserId();

            bool isUpdated = await _workspaceService.UpdateWorkspace(workspaceDto, userId);

            if (isUpdated)
                    return Ok("Workspace name is updated succesfully");
            else
                return NotFound($"Failed to Update Workspace");




        }

       





    }
}



//[HttpPost]
//public async Task<IActionResult> CreateWorkspace([FromBody] WorkspaceDto workspaceDto)
//{
//    bool isCreated = false;
//    var workspace = _mapper.Map<Workspace>(workspaceDto);
//    isCreated = await _workspaceService.CreateWorkspace(workspace);

//    if (isCreated)
//        return Ok(isCreated);
//    else
//        return BadRequest();

//}
