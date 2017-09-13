using System;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;

namespace MvxForms.Core.ViewModels
{
	public class NextFormsViewModel : MvxViewModel<object>
	{
		private readonly IMvxNavigationService _navigationService;
		public NextFormsViewModel(IMvxNavigationService navServcie)
		{
			_navigationService = navServcie;

		}

		public string WelcomeStr { get; set; } = "Next Page ParamIn:";

		public override Task Initialize(object parameter)
		{
			WelcomeStr += parameter.ToString();

			return base.Initialize();
			//throw new NotImplementedException();
		}
	}
}
