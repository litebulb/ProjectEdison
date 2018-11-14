namespace Edison.Mobile.Common.Shared
{
    public class BaseCollectionItemViewModel
    {
        public virtual void ViewAppearing()
        {
            BindEventHandlers();
        }

        public virtual void ViewAppeared()
        {

        }

        public virtual void ViewDisappearing()
        {

        }

        public virtual void ViewDisappeared()
        {
            UnBindEventHandlers();
        }

        public virtual void BindEventHandlers()
        {
            UnBindEventHandlers();
        }

        public virtual void UnBindEventHandlers() 
        {

        }
    }
}
