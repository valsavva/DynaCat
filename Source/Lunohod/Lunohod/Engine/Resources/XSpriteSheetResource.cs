using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
            string mapFilePath = Path.ChangeExtension(this.Source, "txt");
            mapFilePath = Path.Combine(Directory.GetCurrentDirectory(), p.Game.Content.RootDirectory, r.RootFolder.Replace('/', Path.DirectorySeparatorChar), mapFilePath);

            if (!File.Exists(mapFilePath))
                throw new InvalidOperationException(string.Format("Could not find spritesheet map file: {0}", mapFilePath));

            var content = File.ReadAllLines(mapFilePath);

            // parse texture names and coordinates
            foreach (var line in content)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split('=');

                var boundsParts = parts[1].Trim().Split(' ');

                this.Map.Add(
                    parts[0].Trim(),
                    new System.Drawing.RectangleF(
                        float.Parse(boundsParts[0]),
                        float.Parse(boundsParts[1]),
                        float.Parse(boundsParts[2]),
                        float.Parse(boundsParts[3])
                    )
                );
            }
        }
    }
}
