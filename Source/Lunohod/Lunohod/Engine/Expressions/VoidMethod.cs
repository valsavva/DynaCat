using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;

namespace Lunohod.Xge
{
    public class VoidMethod : Method<object>
    {
        private IRunnable runnable;
        private XHero hero;
        private XExplosion explosion;
        private XEnemy enemy;

        public VoidMethod(XObject currentObject, string objectId, string propertyId, List<Expression> parameters)
            : base(currentObject, objectId, propertyId, parameters)
        {
            InitializeAction();
        }

        private void InitializeAction()
        {
            runnable = this.target as IRunnable;
            hero = this.target as XHero;
            explosion = this.target as XExplosion;
            enemy = this.target as XEnemy;

            switch (actionId)
            {
                case "Enable": action = this.ActionEnable; break;
                case "Disable": action = this.ActionDisable; break;

                case "Start": action = this.ActionStart; break;
                case "Stop": action = this.ActionStop; break;
                case "Pause": action = this.ActionPause; break;
                case "Resume": action = this.ActionResume; break;

                case "StartLevel": action = this.ActionStartLevel; break;
                case "StartScreen": action = this.ActionStartScreen; break;
                case "CloseCurrentScreen": action = this.ActionCloseCurrentScreen; break;
                case "EndCurrentLevel": action = this.ActionEndCurrentLevel; break;

                case "StartTransaction": action = this.ActionStartTransaction; break;
                case "EndTransaction": action = this.ActionEndTransaction; break;
                case "SetDirection": action = this.ActionSetDirection; break;

                case "Attack": action = this.ActionAttack; break;

                case "Explode": action = this.ActionExplode; break;
                default:
                    throw new InvalidOperationException(
                        string.Format("Unknown method: {0}.{1}", this.target.GetType().FullName, this.actionId)
                    );
            }
        }
        #region Well known actions

        // Common
        public object ActionEnable()
        {
            this.target.Enable();
            return null;
        }
        public object ActionDisable()
        {
            this.target.Disable();
            return null;
        }

        // Runnables
        public object ActionStart()
        {
            this.runnable.Start();
            return null;
        }
        public object ActionStop()
        {
            this.runnable.Stop();
            return null;
        }
        public object ActionResume()
        {
            this.runnable.Resume();
            return null;
        }
        public object ActionPause()
        {
            this.runnable.Pause();
            return null;
        }

        // System
        public object ActionStartLevel()
        {
            this.system.StartLevel((int)((IExpression<float>)parameters[0]).GetValue());
            return null;
        }
        public object ActionStartScreen()
        {
            ((XSystem)this.target).StartScreen(((IExpression<string>)parameters[0]).GetValue());
            return null;
        }
        public object ActionCloseCurrentScreen()
        {
            this.system.CloseCurrentScreen();
            return null;
        }
        public object ActionEndCurrentLevel()
        {
            this.system.EndCurrentLevel();
            return null;
        }

        // Hero
        public object ActionStartTransaction()
        {
            this.hero.StartTransaction();
            return null;
        }
        public object ActionEndTransaction()
        {
            this.hero.EndTransaction();
            return null;
        }
        public object ActionSetDirection()
        {
            this.hero.SetDirection(((IExpression<float>)parameters[0]).GetValue(), ((IExpression<float>)parameters[1]).GetValue());
            return null;
        }

        // Enemy
        public object ActionAttack()
        {
            this.enemy.Attack();
            return null;
        }

        // Explosion
        public object ActionExplode()
        {
            this.explosion.Explode();
            return null;
        }
        #endregion
    }
}
