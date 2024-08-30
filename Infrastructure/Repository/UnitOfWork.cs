using Domain.Interface;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        public IWorkspaceRepository WorkspaceRepository { get; }
        public IFolderRepository FolderRepository { get; }
        public IDocumentRepository DocumentRepository { get; }
        public IUserRepository UserRepository { get; }

        public UnitOfWork( DataContext context ,IWorkspaceRepository workspaceRepository , IFolderRepository folderRepository , IDocumentRepository documentRepository , IUserRepository userRepository)
        {
            _context = context;
            WorkspaceRepository = workspaceRepository;
            FolderRepository = folderRepository;
            DocumentRepository = documentRepository;
            UserRepository = userRepository;

        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}
