using Application.Dto;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class BaseService
    {
        private readonly IWebHostEnvironment _environment;

        public BaseService(IWebHostEnvironment environment)
        {

            _environment = environment;
        }
        protected string GetWorkspacePath(string workspaceName)
        {
            string wwwrootPath = _environment.WebRootPath;
            string workspacePath = Path.Combine(wwwrootPath, "Workspaces", workspaceName);
            return workspacePath;

        }
        protected string GetFolderPath(string workspaceName , string FolderName )
        {
            string workspacePath = GetWorkspacePath(workspaceName);
            string FolderPath = Path.Combine(workspacePath, FolderName);
            return FolderPath;

        }


        protected bool UpdatePhysicalFolder(string oldPath, string newPath)
        {
            // Check if the old path exists
            if (Directory.Exists(oldPath))
            {
                if (!Directory.Exists(newPath))
                {
                    Directory.Move(oldPath, newPath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        protected bool renameFile(string oldFileName, string newFileName, string fileType)
        {
            if (File.Exists(oldFileName))
            {
                File.Move(oldFileName, newFileName + $".{fileType}");
                return true;
            }
            else
                return false;
        }

    }
}
