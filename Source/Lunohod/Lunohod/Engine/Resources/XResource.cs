using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace Lunohod.Objects
{
	public abstract class XResource : XObject
	{
		public XResource()
		{
		}

        [XmlAttribute]
        public string Source;

        protected T LoadResource<T>(ContentManager content, string buildProcessor, string defaultInExtension, string defaultOutExtension, string importer = "")
        {
#if DEBUG
			Console.WriteLine("==> Loading resource {0} Source: {1}", this.Id, this.Source);
#endif

            XResourceBundle r = (XResourceBundle)this.Parent;

            string fileName = Path.Combine(r.RootFolder.Replace('/', Path.DirectorySeparatorChar), this.Source);

#if WINDOWS
            var inFile = fileName;
            if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
                inFile = Path.ChangeExtension(inFile, defaultInExtension);
            inFile = Path.Combine(Directory.GetCurrentDirectory(), content.RootDirectory, inFile);

            var outFile = Path.ChangeExtension(Path.Combine(Directory.GetCurrentDirectory(), content.RootDirectory, fileName), defaultOutExtension);
            
            var outputPath = Path.GetDirectoryName(inFile);

            if (File.Exists(inFile) && !File.Exists(outFile))
            {
                using (Lunohod.ContentLoading.ContentBuilder b = new ContentLoading.ContentBuilder(outputPath))
                {
                    b.Add(inFile, this.Source, importer, buildProcessor);
                    string error = b.Build();

                    if (!string.IsNullOrEmpty(error))
                        throw new InvalidOperationException(string.Format("Could not compile resource: {0}", this.Source));
                }
            }
#endif
			PerfMon.Start("LoadResource");

			var result = content.Load<T>(fileName);

			PerfMon.Stop("LoadResource");

			return result;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.Source = reader["Source"];

            base.ReadXml(reader);
        }
	}
}

