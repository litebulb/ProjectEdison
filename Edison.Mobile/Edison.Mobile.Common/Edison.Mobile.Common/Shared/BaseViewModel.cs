namespace Edison.Mobile.Common.Shared
{
    public abstract class BaseViewModel
    {
        public string Id => GetType().Name;

        public virtual void ViewCreated() { }

        public virtual void ViewAppearing()
        {
            BindEventHandlers();
        }

        public virtual void ViewAppeared() { }

        public virtual void ViewDisappearing()
        {
            UnBindEventHandlers();
        }

        public virtual void ViewDisappeared() { }

        public virtual void ViewDestroyed() { }

        public virtual void BindEventHandlers()
        {
            UnBindEventHandlers();
        }

        public virtual void UnBindEventHandlers() { }
    }
}
