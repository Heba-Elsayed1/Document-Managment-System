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
            var result = await _workspaceService.GetAllWorkspaces();

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.ErrorMessage); 
        }

        [HttpGet("{workspaceid}")]
        [ProducesResponseType(typeof(WorkspaceDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetWorkspacesById (int workspaceid)
        {
            int userId = GetUserId();

            var result = await _workspaceService.GetWorkspaceById(workspaceid , userId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return BadRequest(result.ErrorMessage);
        }

        [Authorize(Roles = "User")]
        [HttpGet("User")]
        [ProducesResponseType(typeof(WorkspaceDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetWorkspacesOfUser()
        {
            int userId = GetUserId();

            var result = await _workspaceService.GetWorkspaceByUser(userId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return BadRequest(result.ErrorMessage);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Admin")]
        public async Task<IActionResult> GetWorkspacesByUser(int userId)
        {

            var result = await _workspaceService.GetWorkspaceByUser(userId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return BadRequest(result.ErrorMessage);
        }

        [Authorize(Roles = "User")]
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateWorkspace( WorkspaceDto workspaceDto)
        {
            int userId = GetUserId();

            var updated = await _workspaceService.UpdateWorkspace(workspaceDto, userId);

            if (updated.IsSuccess)
                    return Ok();
            else
                return NotFound(updated.ErrorMessage);
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
