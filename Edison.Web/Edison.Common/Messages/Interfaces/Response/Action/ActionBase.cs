using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class ActionBase
    {
        public ActionBase(ActionModel model)
        {
            Action = model;
        }
        public Guid ActionId { get { return Action.ActionId; } }
        private ActionModel Action { get; set; }

        public T GetProperty<T>(string key)
        {
            object value = Action.Parameters[key];
            return (T)Convert.ChangeType(value, typeof(T));
        }
        public void SetProperty(string key, string value)
        {
            Action.Parameters[key] = value;
        }
    }
}
