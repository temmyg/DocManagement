using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using Ionic.Zip;
using System.Reflection;

namespace DocMgmt.Utilities
{
    public static class ZipHelper
    {
        public static byte[] ZipFileWithPassword(string password, string filePath)
        {
            FileStream fs = File.OpenRead(filePath);
            string wkdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string outputFile = wkdir + @"\tmp.zip";
            using (FileStream outputStrm = File.OpenWrite(outputFile))
            using (ZipOutputStream zipStream = new ZipOutputStream(outputStrm))
            {
                zipStream.Password = password;
                zipStream.PutNextEntry(filePath.Substring(filePath.LastIndexOf("\\") + 1));

                int lenToRead = (int)fs.Length;
                byte[] buffer = new byte[lenToRead];
                int start = 0;
                while (lenToRead > 0)
                {
                    int cnt = fs.Read(buffer, start, lenToRead);
                    if (cnt == 0)
                        break;
                    zipStream.Write(buffer, start, lenToRead);
                    start += lenToRead;
                    lenToRead -= cnt;
                }
            }

            byte[] res = File.ReadAllBytes(outputFile);
            return res;
        }

        public static byte[] ExtractFileFromZip(byte[] content, string password, string fileName)
        {   
            string wkdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string outputFile = wkdir + @"\tmp.zip";

            using (FileStream fs = File.OpenWrite(outputFile))
            {
                for (int i = 0; i < content.Length; i++)
                {
                    fs.WriteByte(content[i]);
                }
                fs.Seek(0, SeekOrigin.Begin);
            }
            using (ZipFile zf = new ZipFile(outputFile))
            {
                zf.Password = password;
                zf.ExtractAll(wkdir);
            }
            return File.ReadAllBytes(wkdir + "\\" + fileName);
        }

        public static void SaveToFile(byte[] unzipped)
        {
            string wkdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string outputFile = wkdir + @"\test-unzip.zip";
            File.WriteAllBytes(outputFile, unzipped);
        }
    }
}