using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IUnitOfWork
    {
        IWorkspaceRepository WorkspaceRepository { get; }
        IFolderRepository FolderRepository { get; }
        IDocumentRepository DocumentRepository { get; }
        IUserRepository UserRepository { get; }
        int Save();
    }
}
