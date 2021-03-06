﻿using System;
using System.Collections.Generic;
using MvvmCross.Forms.Core;
using MvxForms.Core.ViewModels;
using Xamarin.Forms;

namespace MvxForms.Core.Pages
{
	public partial class BaiduMapPage : MvxContentPage<BaiduMapViewModel>
	{
		public BaiduMapPage()
		{
			InitializeComponent();
			//NavigationPage.SetHasNavigationBar(this, false);
			NavigationPage.SetHasBackButton(this, true);
			NavigationPage.SetBackButtonTitle(this, "title");
		}
	}
}
