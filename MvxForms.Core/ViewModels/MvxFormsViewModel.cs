using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvxForms.IBLL;

namespace MvxForms.Core.ViewModels
{
	public class MvxFormsViewModel : MvxViewModel
	{
		private readonly IMvxNavigationService _navigationService;
		private readonly IUserBLL _userBLL;
		public MvxFormsViewModel(IMvxNavigationService navService, IUserBLL userBLL)
		{
			_navigationService = navService;
			_userBLL = userBLL;
		}

		public override Task Initialize()
		{
			//TODO: Add starting logic here

			return base.Initialize();
		}

		public IMvxCommand ResetTextCommand => new MvxCommand(ResetText);

		public IMvxCommand JumpToNextCommand => new MvxCommand(JumpToNext);

		public IMvxCommand JumpToBaiduMapCommand => new MvxCommand(JumpToBaiduMapPage);

		public IMvxCommand JumpToCarouselExPageCommand => new MvxCommand(JumpToCarouselPage);

		private void JumpToNext()
		{
			_navigationService.Navigate<NextFormsViewModel, object>(_userBLL.GetUserNameById(1));
		}

		private void ResetText()
		{
			Text = "Hello MvvmCross";
		}

		private void JumpToBaiduMapPage()
		{
			_navigationService.Navigate<BaiduMapViewModel>();
		}

		private void JumpToCarouselPage()
		{
			//_navigationService.Navigate<TestCarouselExViewModel>();
		}

		private string _text = "Hello MvvmCross";
		public string Text
		{
			get { return _text; }
			set { SetProperty(ref _text, value); }
		}

		public object ss { get; set; }
	}
}