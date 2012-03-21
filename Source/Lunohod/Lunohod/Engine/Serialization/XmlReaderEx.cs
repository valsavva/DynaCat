﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace Lunohod.Objects
{
    internal static class XmlReaderEx
    {
        public static bool ReadAttrAsString(this XmlReader reader, string name, ref string s)
        {
            string v = reader[name];
            if (v == null)
                return false;

            s = v;

            return true;
        }
        public static bool ReadAttrAsBoolean(this XmlReader reader, string name, ref bool b)
        {
            string v = reader[name];
            if (v == null)
                return false;

            b = bool.Parse(v);

            return true;
        }
        public static bool ReadAttrAsBoolean(this XmlReader reader, string name, bool def = false)
        {
            string v = reader[name];
            return v == null ? def : bool.Parse(v);
        }
        public static float ReadAttrAsFloat(this XmlReader reader, string name, float def = 0)
        {
            string v = reader[name];
            return v == null ? def : float.Parse(v, CultureInfo.InvariantCulture);
        }
        public static bool ReadAttrAsFloat(this XmlReader reader, string name, ref float f)
        {
            string v = reader[name];
            if (v == null)
                return false;

            f = float.Parse(v, CultureInfo.InvariantCulture);

            return true;
        }
        public static bool ReadAttrAsInt(this XmlReader reader, string name, ref int i)
        {
            string v = reader[name];
            if (v == null)
                return false;

            i = int.Parse(v, CultureInfo.InvariantCulture);

            return true;
        }
        public static bool ReadAttrAsColor(this XmlReader reader, string name, ref Color c)
        {
            string v = reader[name];
            if (v == null)
                return false;

            c = v.ToColor();
            return true;
        }
        public static bool ReadAttrAsEnum<T>(this XmlReader reader, string name, ref T e) where T : struct
        {
            string v = reader[name];
            if (v == null)
                return false;

            if (!Enum.TryParse<T>(v, out e))
                throw new ArgumentException(string.Format("{0} is not a member of enum {1}", v, typeof(T).Name));

            return true;
        }
        public static bool ReadAttrAsRect(this XmlReader reader, string name, ref Rectangle rect)
        {
            string v = reader[name];
            if (v == null)
                return false;

            rect = v.ToRect();
            return true;
        }
        public static bool ReadAttrAsRectF(this XmlReader reader, string name, ref System.Drawing.RectangleF rectF)
        {
            string v = reader[name];
            if (v == null)
                return false;

            rectF = v.ToRectF();
            return true;
        }
        public static bool ReadAttrAsVector2(this XmlReader reader, string name, ref Vector2 v2)
        {
            string v = reader[name];
            if (v == null)
                return false;

            v2 = v.ToVector2();
            return true;
        }
    }
}
