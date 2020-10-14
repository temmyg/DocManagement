using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocMgmt.Models.EFModels;

namespace DocMgmt.Services
{
    public interface IDocsRepositoryService
    {
        DbSet<Document> GetDocuments();
        IQueryable<DocStatusView> GetDocsStatus();
        void AddMissingDocs(ISet<string> searchIds, string docLocation);
    }
}
