using DocMgmt.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Configuration;
using DocMgmt.Utilities;

namespace DocMgmt.Services
{
    public class DocsRepositoryService : IDocsRepositoryService
    {
        DocumentsEntities dbContext;
        public DocsRepositoryService()
        {
            dbContext = new DocumentsEntities();
        }

        public DbSet<Document> GetDocuments()
        {
            return dbContext.Documents;
        }

        public IQueryable<DocStatusView> GetDocsStatus()
        {
            return dbContext.DocStatusViews;
        }

        public void AddMissingDocs(ISet<string> searchPropIds, string fileName)
        {
            if (searchPropIds != null)
            {
                IEnumerable<DocStatusView> status = this.GetDocsStatus().ToList().Where(doc => searchPropIds.Contains(doc.PropertyId.ToString()));
                DbSet<Document> docs = this.GetDocuments();
                foreach (DocStatusView docStt in status)
                {
                    foreach(DocTypeEnum type in Enum.GetValues(typeof(DocTypeEnum)).Cast<DocTypeEnum>()){
                        if(!IsExisting(type, docStt))
                        {
                            var doc = docs.Add(new Document {Id = Guid.NewGuid(), PropertyId = docStt.PropertyId, DocType = type.ToString(),
                                                FileName = DocMgmtHelper.GetFilePathAndName(fileName)[1] });
                            string password = DocMgmtHelper.GetPassword(doc.Id.ToString(), doc.PropertyId.ToString());
                            doc.DocBlob = ZipHelper.ZipFileWithPassword(password, fileName);
                        }
                    }
                }

                dbContext.SaveChanges();
            }
        }

        private bool IsExisting(DocTypeEnum docType, DocStatusView stt)
        {
            bool result = true;
            switch (docType)
            {
                case DocTypeEnum.Agreement:
                    result = stt.Agreement ?? false;
                    break;
                case DocTypeEnum.Appraisal:
                    result = stt.Appraisal ?? false;
                    break;
                case DocTypeEnum.SiteMap:
                    result = stt.SiteMap ?? false;
                    break;
                case DocTypeEnum.Resume:
                    result = stt.Resume ?? false;
                    break;
                case DocTypeEnum.Paperwork:
                    result = stt.Paperwork ?? false;
                    break;
            }

            return result;
        }


    }
}