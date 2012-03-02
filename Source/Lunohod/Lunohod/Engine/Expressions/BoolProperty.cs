using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;
using System.Reflection;
using System.Diagnostics;

namespace Lunohod.Xge
{
    public class BoolProperty : Property<bool>, IBoolExpression
    {
        private IRunnable runnable;
        private XHero hero;

        public BoolProperty(XObject currentObject, string objectId, string propertyId)
            : base(currentObject, objectId, propertyId)
        {
            InitializeAccessor();
        }

        private void InitializeAccessor()
        {
            runnable = target as IRunnable;
            hero = target as XHero;

            switch (propertyId)
            {
                case "Enabled": getter = GetEnabled; setter = SetEnabled; break;
                case "InProgress": getter = GetInProgress; setter = SetInProgress; break;
                case "IsPaused": getter = GetIsPaused; setter = SetIsPaused; break;
                case "IsDead": getter = GetIsDead; setter = SetIsDead; break;
                default:
                {
                    InitializeGenericAccessor();
                } break;
            }
        }

        #region known Boolean getters/setters
        private bool GetEnabled()
        {
            return target.Enabled;
        }
        private void SetEnabled(bool v)
        {
            target.Enabled = v;
        }
        private bool GetInProgress()
        {
            return runnable.InProgress;
        }
        private void SetInProgress(bool v)
        {
            // noop
        }
        private bool GetIsPaused()
        {
            return runnable.IsPaused;
        }
        private void SetIsPaused(bool v)
        {
            // noop
        }
        private bool GetIsDead()
        {
            return hero.IsDead;
        }
        private void SetIsDead(bool v)
        {
            // noop
        }
        #endregion
    }
}
