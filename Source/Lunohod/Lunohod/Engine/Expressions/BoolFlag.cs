﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;

namespace Lunohod.Xge
{
    class BoolFlag : Expression<bool>
    {
        private string eventName;

        public BoolFlag(XObject currentObject, string objectId, string flag)
        {
            this.eventName = objectId + ":" + flag;

            if (currentObject.FindGlobal(objectId) == null)
				throw new InvalidOperationException(string.Format("Could not find object with id [{0}]", objectId));
        }

        public override bool GetValue()
        {
            return GameEngine.Instance.ScreenEngine.CurrentEvents.ContainsKey(this.eventName);
        }

        public override string ToString()
        {
            return eventName;
        }
    }
}
