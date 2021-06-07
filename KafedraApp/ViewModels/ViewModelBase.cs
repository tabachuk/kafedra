using KafedraApp.Helpers;

namespace KafedraApp.ViewModels
{
	public abstract class ViewModelBase : BindableBase
    {
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            protected set => SetProperty(ref _isBusy, value);
        }
    }
}
