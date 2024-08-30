using Application.Dto;
using AutoMapper;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public class MappingProfiles :Profile
    {
        public MappingProfiles()
        {
            CreateMap<Workspace, WorkspaceDto>();
            CreateMap<WorkspaceDto, Workspace>();
            CreateMap<Folder, FolderDto>();
            CreateMap<FolderDto, Folder>();
            CreateMap<User,UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Document, DocumentDto>();
            CreateMap<DocumentDto, Document>();
        }
    }
}
