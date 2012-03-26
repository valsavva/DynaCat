using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;
using System.Diagnostics;

namespace Lunohod.Xge
{
    public class NumProperty : Property<double>, IExpression<double>
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
                case "DefaultHealth": getter = GetDefaultHealth; setter = SetDefaultHealth; break;
                case "Speed": getter = GetSpeed; setter = SetSpeed; break;
                case "DefaultSpeed": getter = GetDefaultSpeed; setter = SetDefaultSpeed; break;
                case "BombCount": getter = GetBombCount; setter = SetBombCount; break;
                case "DefaultBombCount": getter = GetDefaultBombCount; setter = SetDefaultBombCount; break;
                case "Deceleration": getter = GetDeceleration; setter = SetDeceleration; break;
                default:
                {
                    InitializeGenericAccessor();

                    if (propertyInfo.PropertyType != typeof(double)
                        && propertyInfo.PropertyType != typeof(float)
                        && propertyInfo.PropertyType != typeof(int)
                    )
                    {
                        throw new InvalidOperationException(
                            string.Format("Property [{0}.{1}] has type [{2}] which is not numeric. Numeric types are double, double and int",
                                target.GetType().FullName, propertyId, propertyInfo.PropertyType)
                        );
                    }
                }; break;
            }

        }

        #region known numeric Getters/Setters
        private double GetX()
        {
            return element.X;
        }
        private void SetX(double v)
        {
            element.X = (float)v;
        }
        private double GetY()
        {
            return element.Y;
        }
        private void SetY(double v)
        {
            element.Y = (float)v;
        }
        private double GetWidth()
        {
            return element.Width;
        }
        private void SetWidth(double v)
        {
            element.Width = (float)v;
        }
        private double GetHeight()
        {
            return element.Height;
        }
        private void SetHeight(double v)
        {
            element.Height = (float)v;
        }
        private double GetRotation()
        {
            return element.Rotation;
        }
        private void SetRotation(double v)
        {
            element.Rotation = v;
        }
        private double GetCurrentFrame()
        {
            return sprite.CurrentFrame;
        }
        private void SetCurrentFrame(double v)
        {
            sprite.CurrentFrame = (int)Math.Round(v);
        }
        private double GetOpacity()
        {
            return element.Opacity;
        }
        private void SetOpacity(double v)
        {
            element.Opacity = v;
        }
        private double GetScale()
        {
            return element.Scale;
        }
        private void SetScale(double v)
        {
            element.Scale = v;
        }
        private double GetVolume()
        {
            return audio.Volume;
        }
        private void SetVolume(double v)
        {
            audio.Volume = v;
        }
        private double GetHealth()
        {
            return hero.Health;
        }
        private void SetHealth(double v)
        {
            // noop
        }
        private double GetDefaultHealth()
        {
            return hero.DefaultHealth;
        }
        private void SetDefaultHealth(double v)
        {
            // noop
        }
        private double GetSpeed()
        {
            return hero.Speed;
        }
        private void SetSpeed(double v)
        {
            hero.Speed = v;
        }
        private double GetDefaultSpeed()
        {
            return hero.DefaultSpeed;
        }
        private void SetDefaultSpeed(double v)
        {
            hero.DefaultSpeed = v;
        }
        private double GetBombCount()
        {
            return hero.BombCount;
        }
        private void SetBombCount(double v)
        {
            hero.BombCount = v;
        }
        private double GetDefaultBombCount()
        {
            return hero.DefaultBombCount;
        }
        private void SetDefaultBombCount(double v)
        {
            // noop
        }
        private double GetDeceleration()
        {
            return hero.Deceleration;
        }
        private void SetDeceleration(double v)
        {
            hero.Deceleration = v;
        }
        #endregion
    }
}
