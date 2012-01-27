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
        public void SetSmooth()
        {
            for (int i = 0; i < this.Keys.Count; i++)
            {
                this.SetSmooth(i);
            }
        }

        public void SetSmooth(int i)
        {
            CurveKey prev;
            CurveKey current;
            CurveKey next;
            int prevIndex;
            int nextIndex;

            prevIndex = i - 1;
            if (prevIndex < 0) prevIndex = i;

            nextIndex = i + 1;
            if (nextIndex == this.Keys.Count) nextIndex = i;

            prev = this.Keys[prevIndex];
            next = this.Keys[nextIndex];
            current = this.Keys[i];
            SetCurveKeyTangent(ref prev, ref current, ref next);
            this.Keys[i] = current;
        }

        private static void SetCurveKeyTangent(ref CurveKey prev, ref CurveKey cur, ref CurveKey next)
        {
            float dt = next.Position - prev.Position;
            float dv = next.Value - prev.Value;
            if (Math.Abs(dv) < float.Epsilon)
            {
                cur.TangentIn = 0;
                cur.TangentOut = 0;
            }
            else
            {
                // The in and out tangents should be equal to the slope between the adjacent keys.
                cur.TangentIn = dv * (cur.Position - prev.Position) / dt;
                cur.TangentOut = dv * (next.Position - cur.Position) / dt;
            }
        }


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
            float position;
            float num2;
            float num = num2 = (position = curveKey.Position);
            float value;
            float num4;
            float num3 = num4 = (value = curveKey.Value);
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
                float num5 = position - num2;
                float num6 = value - num4;
                if (Math.Abs(num6) < 1.1920929E-07f)
                {
                    curveKey.TangentIn = 0f;
                }
                else
                {
                    curveKey.TangentIn = num6 * Math.Abs(num2 - num) / num5;
                }
            }
            else
            {
                if (tangentInType == CurveTangent.Linear)
                {
                    curveKey.TangentIn = num3 - num4;
                }
                else
                {
                    curveKey.TangentIn = 0f;
                }
            }
            if (tangentOutType == CurveTangent.Smooth)
            {
                float num7 = position - num2;
                float num8 = value - num4;
                if (Math.Abs(num8) < 1.1920929E-07f)
                {
                    curveKey.TangentOut = 0f;
                    return;
                }
                curveKey.TangentOut = num8 * Math.Abs(position - num) / num7;
                return;
            }
            else
            {
                if (tangentOutType == CurveTangent.Linear)
                {
                    curveKey.TangentOut = value - num3;
                    return;
                }
                curveKey.TangentOut = 0f;
                return;
            }
        }
    }
}
