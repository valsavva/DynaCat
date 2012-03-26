using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;

namespace Lunohod.Objects
{
    public class TimeCurve : Curve
    {
        public void ComputeT(int keyIndex, CurveTangent tangentType)
        {
            this.ComputeT(keyIndex, tangentType, tangentType);
        }

        public void ComputeT(int keyIndex, CurveTangent tangentInType, CurveTangent tangentOutType)
        {
            if (this.Keys.Count <= keyIndex || keyIndex < 0)
            {
                throw new ArgumentOutOfRangeException("keyIndex");
            }
            CurveKey curveKey = this.Keys[keyIndex];
            double position;
            double num2;
            double num = num2 = (position = curveKey.Position);
            double value;
            double num4;
            double num3 = num4 = (value = curveKey.Value);
            if (keyIndex > 0)
            {
                num2 = this.Keys[keyIndex - 1].Position;
                num4 = this.Keys[keyIndex - 1].Value;
            }
            if (keyIndex + 1 < this.Keys.Count)
            {
                position = this.Keys[keyIndex + 1].Position;
                value = this.Keys[keyIndex + 1].Value;
            }
            if (tangentInType == CurveTangent.Smooth)
            {
                double num5 = position - num2;
                double num6 = value - num4;
                if (Math.Abs(num6) < 1.1920929E-07f)
                {
                    curveKey.TangentIn = 0f;
                }
                else
                {
                    curveKey.TangentIn = (float)(num6 * Math.Abs(num2 - num) / num5);
                }
            }
            else
            {
                if (tangentInType == CurveTangent.Linear)
                {
                    curveKey.TangentIn = (float)(num3 - num4);
                }
                else
                {
                    curveKey.TangentIn = 0f;
                }
            }
            if (tangentOutType == CurveTangent.Smooth)
            {
                double num7 = position - num2;
                double num8 = value - num4;
                if (Math.Abs(num8) < 1.1920929E-07f)
                {
                    curveKey.TangentOut = 0f;
                    return;
                }
                curveKey.TangentOut = (float)(num8 * Math.Abs(position - num) / num7);
                return;
            }
            else
            {
                if (tangentOutType == CurveTangent.Linear)
                {
                    curveKey.TangentOut = (float)(value - num3);
                    return;
                }
                curveKey.TangentOut = 0f;
                return;
            }
        }
    }
}
