using System;
using Xamarin.Forms;
using MvxForms.Platform.Forms.Controls;
using System.Linq;
using System.Collections;
using System.Reflection;
using MvvmCross.Core.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace MvxForms.Platform.Forms.ExPages
{
	public class CarouselExPage : ContentPage
	{
		CarouselLayout.IndicatorStyleEnum _indicatorStyle;

		public CarouselExPage()
		{
			_indicatorStyle = CarouselLayout.IndicatorStyleEnum.Dots;

			BindingContext = new DefaultCarouselItemSource();

			var relativeLayout = new RelativeLayout()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			//创建Carousel样式布局
			var pagesCarousel = CreatePagesCarousel(); `
			//创建点模式容器
			var dots = CreatePagerIndicatorContainer();
			//创建Tab容器
			var _tabs = CreateTabs();

			//根据引导页类型初始化引导页布局
			switch (pagesCarousel.IndicatorStyle)
			{
				case CarouselLayout.IndicatorStyleEnum.Dots:
					//添加Carousel布局
					relativeLayout.Children.Add(pagesCarousel,
						Constraint.RelativeToParent((parent) => { return parent.X; }),
						Constraint.RelativeToParent((parent) => { return parent.Y; }),
						Constraint.RelativeToParent((parent) => { return parent.Width; }),
						Constraint.RelativeToParent((parent) => { return parent.Height; })
					);
					//添加点布局
					relativeLayout.Children.Add(dots,
						Constraint.Constant(0),
						Constraint.RelativeToView(pagesCarousel,(parent, sibling) => { return sibling.Height - 18; }),
						Constraint.RelativeToParent(parent => parent.Width),
						Constraint.Constant(18)
					);
					break;
				case CarouselLayout.IndicatorStyleEnum.Tabs:
					var tabsHeight = 50;
					//添加tab布局
					relativeLayout.Children.Add(_tabs,
						Constraint.Constant(0),
						Constraint.RelativeToParent((parent) => { return parent.Height - tabsHeight; }),
						Constraint.RelativeToParent(parent => parent.Width),
						Constraint.Constant(tabsHeight)
					);
					//添加Carousel布局
					relativeLayout.Children.Add(pagesCarousel,
						Constraint.RelativeToParent((parent) => { return parent.X; }),
						Constraint.RelativeToParent((parent) => { return parent.Y; }),
						Constraint.RelativeToParent((parent) => { return parent.Width; }),
						Constraint.RelativeToView(_tabs, (parent, sibling) => { return parent.Height - (sibling.Height); })
					);
					break;
				default:
					relativeLayout.Children.Add(pagesCarousel,
						Constraint.RelativeToParent((parent) => { return parent.X; }),
						Constraint.RelativeToParent((parent) => { return parent.Y; }),
						Constraint.RelativeToParent((parent) => { return parent.Width; }),
						Constraint.RelativeToParent((parent) => { return parent.Height; })
					);
					break;
			}

			Content = relativeLayout;
		}

		CarouselLayout CreatePagesCarousel()
		{
			var carousel = new CarouselLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				IndicatorStyle = _indicatorStyle,
				ItemTemplate = new DataTemplate(typeof(CarouselBaseTemplate))
			};
			carousel.SetBinding(CarouselLayout.ItemsSourceProperty, "Pages");
			carousel.SetBinding(CarouselLayout.SelectedItemProperty, "CurrentPage", BindingMode.TwoWay);
			return carousel;
		}

		View CreatePagerIndicatorContainer()
		{
			return new StackLayout
			{
				Children = { CreatePagerIndicators() }
			};
		}

		View CreatePagerIndicators()
		{
			var pagerIndicator = new PagerIndicatorDots() { DotSize = 5, DotColor = Color.Black };
			pagerIndicator.SetBinding(PagerIndicatorDots.ItemsSourceProperty, "Pages");
			pagerIndicator.SetBinding(PagerIndicatorDots.SelectedItemProperty, "CurrentPage");
			return pagerIndicator;
		}

		View CreateTabsContainer()
		{
			return new StackLayout
			{
				Children = { CreateTabs() }
			};
		}

		View CreateTabs()
		{
			var pagerIndicator = new PagerIndicatorTabs() { HorizontalOptions = LayoutOptions.CenterAndExpand };
			pagerIndicator.RowDefinitions.Add(new RowDefinition() { Height = 50 });
			pagerIndicator.SetBinding(PagerIndicatorTabs.ItemsSourceProperty, "Pages");
			pagerIndicator.SetBinding(PagerIndicatorTabs.SelectedItemProperty, "CurrentPage");

			return pagerIndicator;
		}
	}

	public interface ITabProvider
	{
		string ImageSource { get; set; }
	}




	/// <summary>
	/// 引导页显示模板
	/// </summary>
	public class CarouselBaseTemplate : ContentView
	{
		public CarouselBaseTemplate()
		{
			BackgroundColor = Color.White;

			var label = new Label
			{
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.Black
			};

			label.SetBinding(Label.TextProperty, "Title");
			this.SetBinding(ContentView.BackgroundColorProperty, "Background");

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children = { label }
			};
		}
	}

	/// <summary>
	/// 引导页viewmodel
	/// </summary>
	public abstract class CarouselItemSource : INotifyPropertyChanged
	{
		public INavigation Navigation { get; set; }

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged == null) return;
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		protected void SetObservableProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return;
			field = value;
			OnPropertyChanged(propertyName);
		}


		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion
	}

	/// <summary>
	/// 引导页元数据
	/// </summary>
	public class CarouselMetaSource : CarouselItemSource, ITabProvider
	{
		public string Title { get; set; }
		public Color Background { get; set; }
		public string ImageSource { get; set; }
	}

	/// <summary>
	/// 引导页默认数据源
	/// </summary>
	public class DefaultCarouselItemSource : CarouselItemSource
	{
		public DefaultCarouselItemSource()
		{
			Pages = new List<CarouselMetaSource>() {
				new CarouselMetaSource { Title = "1", Background = Color.White, ImageSource = "icon.png" },
				new CarouselMetaSource { Title = "2", Background = Color.Red, ImageSource = "icon.png" },
				new CarouselMetaSource { Title = "3", Background = Color.Blue, ImageSource = "icon.png" },
				new CarouselMetaSource { Title = "4", Background = Color.Yellow, ImageSource = "icon.png" },
			};

			CurrentPage = Pages.First();
		}

		IEnumerable<CarouselMetaSource> _pages;
		public IEnumerable<CarouselMetaSource> Pages
		{
			get
			{
				return _pages;
			}
			set
			{
				SetObservableProperty(ref _pages, value);
				CurrentPage = Pages.FirstOrDefault();
			}
		}

		CarouselMetaSource _currentPage;
		public CarouselMetaSource CurrentPage
		{
			get
			{
				return _currentPage;
			}
			set
			{
				SetObservableProperty(ref _currentPage, value);
			}
		}
	}
}
