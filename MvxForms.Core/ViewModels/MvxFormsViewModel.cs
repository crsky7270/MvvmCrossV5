using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;

namespace MvxForms.Core.ViewModels
{
    public class MvxFormsViewModel : MvxViewModel
    {
		private readonly IMvxNavigationService _navigationService;
		public MvxFormsViewModel(IMvxNavigationService navService)
        {
			_navigationService = navService;
        }
        
        public override Task Initialize()
        {
            //TODO: Add starting logic here
		    
            return base.Initialize();
        }
        
        public IMvxCommand ResetTextCommand => new MvxCommand(ResetText);

		public IMvxCommand JumpToNextCommand => new MvxCommand(JumpToNext);

		private void JumpToNext() {
			_navigationService.Navigate<NextFormsViewModel,object>("123");
		}

        private void ResetText()
        {
            Text = "Hello MvvmCross";
        }

        private string _text = "Hello MvvmCross";
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
    }
}