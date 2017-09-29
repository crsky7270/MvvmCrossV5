using System;
using System.Collections.Generic;
using MvvmCross.Forms.Core;
using MvxForms.Core.ViewModels;
using Xamarin.Forms;
using MvxForms.Platform.Forms.Controls;
namespace MvxForms.Core.Pages
{
	public partial class NextFormsPage : MvxContentPage<NextFormsViewModel>
	{
		public NextFormsPage()
		{
			try
			{
				InitializeComponent();
				this.Content = new CarouselContainer() { IndicatorStyle = CarouselLayout.IndicatorStyleEnum.Dots };

				//NavigationPage.SetHasNavigationBar(this, false);
			}
			catch (Exception ex)
			{
				throw ex;
			}



		}
	}
}
