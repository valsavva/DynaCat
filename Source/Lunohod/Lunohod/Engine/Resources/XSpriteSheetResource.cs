﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Lunohod.Objects
{
    public class XSpriteSheetResource : XTextureResource
    {
        public Dictionary<string, System.Drawing.RectangleF> Map = new Dictionary<string,System.Drawing.RectangleF>(StringComparer.InvariantCultureIgnoreCase);

        public override void InitializeMainThread(InitializeParameters p)
        {
            base.InitializeMainThread(p);

            ParseMapFile(p);
        }

        private void ParseMapFile(InitializeParameters p)
        {
            // read file
            XResourceBundle r = (XResourceBundle)this.Parent;
            string mapFilePath = Path.ChangeExtension(this.Source, "xml");
            mapFilePath = Path.Combine(Directory.GetCurrentDirectory(), p.Game.Content.RootDirectory, r.RootFolder.Replace('/', Path.DirectorySeparatorChar), mapFilePath);

            if (!File.Exists(mapFilePath))
                throw new InvalidOperationException(string.Format("Could not find spritesheet map file: {0}", mapFilePath));

            XmlDocument doc = new XmlDocument();
            doc.Load(mapFilePath);

            foreach (XmlElement spriteNode in doc.DocumentElement.ChildNodes)
            {
                this.Map.Add(
                    spriteNode.GetAttribute("n"),
                    new System.Drawing.RectangleF(
                        float.Parse(spriteNode.GetAttribute("x")),
                        float.Parse(spriteNode.GetAttribute("y")),
                        float.Parse(spriteNode.GetAttribute("w")),
                        float.Parse(spriteNode.GetAttribute("h"))
                    )
                );
            }
        }
    }
}