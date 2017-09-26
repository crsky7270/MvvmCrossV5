using System;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using MvxForms.BLL;
using MvxForms.IBLL;

namespace MvxForms.Core
{
	public class CoreApp : MvvmCross.Core.ViewModels.MvxApplication
	{
		public override void Initialize()
		{
			CreatableTypes()
				.EndingWith("Service")
				.AsInterfaces()
				.RegisterAsLazySingleton();

			Mvx.RegisterType<IUserBLL, UserBLL>();
			RegisterAppStart<ViewModels.MvxFormsViewModel>();
		}
	}
}
