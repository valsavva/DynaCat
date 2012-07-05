using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections;

namespace XmlWhiteSpaceRemover
{
    class Program
    {
        const string xmlns = "http://schemas.microsoft.com/developer/msbuild/2003";

        static void Main(string[] args)
        {
            string path = args[0];
            string pattern = args[1];
            string outPath = args[2];

            path = Path.GetFullPath(path);
            outPath = Path.GetFullPath(outPath);

            var files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
            Array.Sort(files);

            try
            {
                foreach (var file in files)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file);
                    var comments = doc.SelectNodes(@"//comment()");

                    List<XmlNode> toDelete = new List<XmlNode>();

                    foreach (XmlNode c in comments)
                        toDelete.Add(c);

                    foreach (var c in toDelete)
                        c.ParentNode.RemoveChild(c);


                    string relativeDir = Path.GetDirectoryName(file.Substring(path.Length+1));
                    string newFileDir = Path.Combine(outPath, relativeDir);

                    if (!Directory.Exists(newFileDir))
                        Directory.CreateDirectory(newFileDir);
                    
                    string newFilePath = Path.Combine(newFileDir, Path.GetFileName(file));

                    using (XmlWriter w = XmlWriter.Create(newFilePath, new XmlWriterSettings() { Indent = false, NewLineHandling = NewLineHandling.None }))
                    {
                        doc.WriteTo(w);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
