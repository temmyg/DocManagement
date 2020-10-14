using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocMgmt.Utilities
{
    public static class DocMgmtHelper
    {
        public static string[] GetFilePathAndName(string fileName)
        {
            string fileDir = fileName;
            string path = fileDir.Substring(0, fileDir.LastIndexOf("\\"));
            string name = fileDir.Substring(fileDir.LastIndexOf("\\") + 1);
            return new string[] { path, name };
        }

        public static string GetPassword(string Id, string PropId)
        {
            string pswd = Id.Substring(0, 8) + PropId.Substring(PropId.Length - 8, 8);
            return pswd;
        }
    }
}