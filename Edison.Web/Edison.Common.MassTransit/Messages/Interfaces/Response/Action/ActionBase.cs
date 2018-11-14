using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class ActionBase : IActionBaseEvent
    {
        public ActionBase(ActionModel _model, bool _isCloseAction = false)
        {
            Action = _model;
            IsCloseAction = _isCloseAction;
        }
        public Guid ActionId { get { return Action.ActionId; } }
        public Guid ResponseId { get; set; }
        public bool IsCloseAction { get; set; }
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
