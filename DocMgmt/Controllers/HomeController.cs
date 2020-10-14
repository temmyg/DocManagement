using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Policy;
using System.Configuration;
using DocMgmt.Models;
using DocMgmt.Services;
using Newtonsoft.Json;
using DocMgmt.Models.EFModels;
using Ionic.Zip;
using DocMgmt.Utilities;

namespace DocMgmt.Controllers
{
    public class HomeController : Controller
    { 

        IDocsRepositoryService repoService;
        List<Document> docs;

        public HomeController(IDocsRepositoryService repoService)
        {
            this.repoService = repoService;
        }

        public ActionResult Index()
        {
            docs = new List<Document>()
            {
                
            };
            return View(docs);
        }

        [HttpPost]
        public ActionResult AppendDocs(HttpPostedFileBase file)
        {
            string[] propIds = null;
            string fileDir = ConfigurationManager.AppSettings["DocLocation"];
            string fileName = Server.MapPath(fileDir);

            using (StreamReader sr = new StreamReader(file.InputStream))
            {
                dynamic jsonContent = JsonConvert.DeserializeObject(sr.ReadToEnd());
                if (jsonContent["data"] != null)
                {
                    propIds = jsonContent["data"].ToObject<string[]>();
                }
            }
            string docLoc = ConfigurationManager.AppSettings["DocLocation"];                
            repoService.AddMissingDocs(new HashSet<string>(propIds), Server.MapPath(docLoc));

            return View("index", repoService.GetDocuments().ToList());
        }

        public ActionResult DownloadDoc(Guid? docId, string docType)
        {
            string docLoc = ConfigurationManager.AppSettings["DocLocation"];
            string localPath = Server.MapPath(docLoc);
            string fileName = DocMgmtHelper.GetFilePathAndName(localPath)[1];
            Document doc = repoService.GetDocuments().First(d => d.Id==docId && d.DocType.ToString().Equals(docType));

            string pswd = DocMgmtHelper.GetPassword(doc.Id.ToString(), doc.PropertyId.ToString());
            byte[] unzipped = Utilities.ZipHelper.ExtractFileFromZip(doc.DocBlob, pswd, fileName);
            return File(unzipped, "application/octet-stream", fileName);
        }

        public ActionResult ZipFile()
        {
            string docLoc = ConfigurationManager.AppSettings["DocLocation"];
            string localPath = Server.MapPath(docLoc);
            string fileName = DocMgmtHelper.GetFilePathAndName(localPath)[1];

            byte[] zipped = Utilities.ZipHelper.ZipFileWithPassword("123", localPath);
            byte[] unzipped = Utilities.ZipHelper.ExtractFileFromZip(zipped, "123", fileName);

            return File(unzipped, "application/octet-stream", "test.docx");
        }
    }
}