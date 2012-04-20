using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
    internal class ClassFactory
    {
        #region TypeInfo
        private static Dictionary<string, Type> typeInfo = new Dictionary<string,Type>()
        {
        { "Resources", typeof(XResourceBundle) },
        { "Dashboard", typeof(XDashboard) },
        { "TapArea", typeof(XTapArea) },
        { "Include", typeof(XInclude) },

        // Media
        { "Music", typeof(XMusic) },
        { "Sound", typeof(XSound) },
		
        // Dashboard
        { "Viewport", typeof(XViewport) },
		
        // Layer
        { "Layer", typeof(XLayer) },
		
		// Triggers
        { "IntersectionTrigger", typeof(XIntersectionTrigger) },
        { "DistanceTrigger", typeof(XDistanceTrigger) },
        { "BoolTrigger", typeof(XBoolTrigger) },
        { "NumTrigger", typeof(XNumTrigger) },
		
		// Classes
        { "Class", typeof(XClass) },

		// Characters
        { "Tower", typeof(XTower) },
        { "Hero", typeof(XHero) },
        { "Enemy", typeof(XEnemy) },
        { "Food", typeof(XFood) },
        { "Explosion", typeof(XExplosion) },

		// Iterator
        { "Iterator", typeof(XIterator) },

		// Sets
        { "SequenceSet", typeof(XSequenceSet) },
        { "RandomSet", typeof(XRandomSet) },
        { "ParallelSet", typeof(XParallelSet) },

        // Actions
        { "Do", typeof(XDo) },
        { "Delay", typeof(XDelay) },

        // Animation
        { "NumAnimation", typeof(XNumAnimation) },
        { "KeyFrame", typeof(XKeyFrame) },

		// Basic elements
        { "Group", typeof(XGroup) },
        { "Image", typeof(XImage) },
        { "Text", typeof(XText) },
        { "Block", typeof(XBlock) },
        { "Sprite", typeof(XSprite) },

        // Resources
        { "Font", typeof(XFontResource) },
        { "Texture", typeof(XTextureResource) },
        { "MusicFile", typeof(XMusicResource) },
        { "SoundFile", typeof(XSoundResource) },
		
		// Screens and levels
        { "Screen", typeof(XScreen) },
        { "Level", typeof(XLevel) },
        { "LevelInfo", typeof(XLevelInfo) },
        { "LevelSeries", typeof(XLevelSeries) },
        { "Screens", typeof(XElement) },
        { "Levels", typeof(XElement) },
		
		// Save score
		{ "LevelScore", typeof(XLevelScore) },
		{ "SaveFile", typeof(XSaveFile) }
        };
        #endregion

        public static XObject CreateObject(string typeName)
        {
            Type type;

            if (typeInfo.TryGetValue(typeName, out type))
                return (XObject)Activator.CreateInstance(type);
            else
            {
                //return new XElement();
                throw new InvalidOperationException("Unknown type " + typeName);
            }
        }
    }
}
