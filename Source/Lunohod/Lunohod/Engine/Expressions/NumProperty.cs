using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;
using System.Diagnostics;

namespace Lunohod.Xge
{
    public class NumProperty : Property<float>, INumExpression
    {
        private XElement element;
        private XSprite sprite;
        private IHasVolume audio;
        private XHero hero;

        public NumProperty(XObject currentObject, string objectId, string propertyId)
            : base(currentObject, objectId, propertyId)
        {
            InitializeAccessor();
        }

        private void InitializeAccessor()
        {
            this.element = target as XElement;
            this.sprite = target as XSprite;
            this.audio = target as IHasVolume;
            this.hero = target as XHero;

            switch (propertyId)
            {
                case "X": getter = GetX; setter = SetX; break;
                case "Y": getter = GetY; setter = SetY; break;
                case "Height": getter = GetHeight; setter = SetHeight; break;
                case "Width": getter = GetWidth; setter = SetWidth; break;
                case "Rotation": getter = GetRotation; setter = SetRotation; break;
                case "Scale": getter = GetScale; setter = SetScale; break;
                case "Opacity": getter = GetOpacity; setter = SetOpacity; break;
                case "CurrentFrame": getter = GetCurrentFrame; setter = SetCurrentFrame; break;
                case "Volume": getter = GetVolume; setter = SetVolume; break;
                case "Health": getter = GetHealth; setter = SetHealth; break;
                case "Speed": getter = GetSpeed; setter = SetSpeed; break;
                case "DefaultSpeed": getter = GetDefaultSpeed; setter = SetDefaultSpeed; break;
                case "Deceleration": getter = GetDeceleration; setter = SetDeceleration; break;
                default:
                {
                    InitializeGenericAccessor();

                    if (propertyInfo.PropertyType != typeof(float)
                        && propertyInfo.PropertyType != typeof(double)
                        && propertyInfo.PropertyType != typeof(int)
                    )
                    {
                        throw new InvalidOperationException(
                            string.Format("Property [{0}.{1}] has type [{2}] which is not numeric. Numeric types are float, double and int",
                                target.GetType().FullName, propertyId, propertyInfo.PropertyType)
                        );
                    }
                }; break;
            }

        }

        #region known numeric Getters/Setters
        private float GetX()
        {
            return element.X;
        }
        private void SetX(float v)
        {
            element.X = v;
        }
        private float GetY()
        {
            return element.Y;
        }
        private void SetY(float v)
        {
            element.Y = v;
        }
        private float GetWidth()
        {
            return element.Width;
        }
        private void SetWidth(float v)
        {
            element.Width = v;
        }
        private float GetHeight()
        {
            return element.Height;
        }
        private void SetHeight(float v)
        {
            element.Height = v;
        }
        private float GetRotation()
        {
            return element.Rotation;
        }
        private void SetRotation(float v)
        {
            element.Rotation = v;
        }
        private float GetCurrentFrame()
        {
            return sprite.CurrentFrame;
        }
        private void SetCurrentFrame(float v)
        {
            sprite.CurrentFrame = (int)Math.Round(v);
        }
        private float GetOpacity()
        {
            return element.Opacity;
        }
        private void SetOpacity(float v)
        {
            element.Opacity = v;
        }
        private float GetScale()
        {
            return element.Scale;
        }
        private void SetScale(float v)
        {
            element.Scale = v;
        }
        private float GetVolume()
        {
            return audio.Volume;
        }
        private void SetVolume(float v)
        {
            audio.Volume = v;
        }
        private float GetHealth()
        {
            return hero.Health;
        }
        private void SetHealth(float v)
        {
            // noop
        }
        private float GetSpeed()
        {
            return hero.Speed;
        }
        private void SetSpeed(float v)
        {
            hero.Speed = v;
        }
        private float GetDefaultSpeed()
        {
            return hero.DefaultSpeed;
        }
        private void SetDefaultSpeed(float v)
        {
            hero.DefaultSpeed = v;
        }
        private float GetDeceleration()
        {
            return hero.Deceleration;
        }
        private void SetDeceleration(float v)
        {
            hero.Deceleration = v;
        }
        #endregion
    }
}
