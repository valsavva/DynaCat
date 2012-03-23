using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

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

            var files = Directory.GetFiles(path, pattern);
            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outPath);

            try
            {
                foreach (var file in files)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file);
                    var comments = doc.SelectNodes(@"//comment()");

                    foreach (XmlNode c in comments)
                        c.ParentNode.RemoveChild(c);

                    using (XmlWriter w = XmlWriter.Create(Path.Combine(outPath, Path.GetFileName(file)), new XmlWriterSettings() { Indent = false, NewLineHandling = NewLineHandling.None }))
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
