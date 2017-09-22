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
	public class CarouselExViewModel : MvxViewModel
	{
	}

	public class CarouselExPage : MvvmCross.Forms.Core.MvxContentPage<CarouselExViewModel>
	{
		View _tabs;
		RelativeLayout relativeLayout;
		CarouselLayout.IndicatorStyleEnum _indicatorStyle;
		DefaultCarouselItemSource defaultCarouseItemSource;

		public CarouselExPage()//CarouselLayout.IndicatorStyleEnum style
		{
			_indicatorStyle = CarouselLayout.IndicatorStyleEnum.Dots;

			defaultCarouseItemSource = new DefaultCarouselItemSource();
			BindingContext = defaultCarouseItemSource;

			relativeLayout = new RelativeLayout()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			var pagesCarousel = CreatePagesCarousel();
			var dots = CreatePagerIndicatorContainer();
			_tabs = CreateTabs();

			switch (pagesCarousel.IndicatorStyle)
			{
				case CarouselLayout.IndicatorStyleEnum.Dots:
					relativeLayout.Children.Add(pagesCarousel,
						Constraint.RelativeToParent((parent) => { return parent.X; }),
						Constraint.RelativeToParent((parent) => { return parent.Y; }),
						Constraint.RelativeToParent((parent) => { return parent.Width; }),
						Constraint.RelativeToParent((parent) => { return parent.Height; })
					);

					relativeLayout.Children.Add(dots,
						Constraint.Constant(0),
						Constraint.RelativeToView(pagesCarousel,
							(parent, sibling) => { return sibling.Height - 18; }),
						Constraint.RelativeToParent(parent => parent.Width),
						Constraint.Constant(18)
					);
					break;
				case CarouselLayout.IndicatorStyleEnum.Tabs:
					var tabsHeight = 50;
					relativeLayout.Children.Add(_tabs,
						Constraint.Constant(0),
						Constraint.RelativeToParent((parent) => { return parent.Height - tabsHeight; }),
						Constraint.RelativeToParent(parent => parent.Width),
						Constraint.Constant(tabsHeight)
					);

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
	/// Pager indicator dots.
	/// </summary>
	public class PagerIndicatorDots : StackLayout
	{
		int _selectedIndex;

		public Color DotColor { get; set; }
		public double DotSize { get; set; }

		public PagerIndicatorDots()
		{
			HorizontalOptions = LayoutOptions.CenterAndExpand;
			VerticalOptions = LayoutOptions.Center;
			Orientation = StackOrientation.Horizontal;
			DotColor = Color.Black;
		}

		void CreateDot()
		{
			//Make one button and add it to the dotLayout
			var dot = new Button
			{
				BorderRadius = Convert.ToInt32(DotSize / 2),
				HeightRequest = DotSize,
				WidthRequest = DotSize,
				BackgroundColor = DotColor
			};
			Children.Add(dot);
		}

		void CreateTabs()
		{
			foreach (var item in ItemsSource)
			{
				var tab = item as ITabProvider;
				var image = new Image
				{
					HeightRequest = 42,
					WidthRequest = 42,
					BackgroundColor = DotColor,
					Source = tab.ImageSource,
				};
				Children.Add(image);
			}
		}

		public static BindableProperty ItemsSourceProperty =
			BindableProperty.Create(
				nameof(ItemsSource),
				typeof(IList),
				typeof(PagerIndicatorDots),
				null,
				BindingMode.OneWay,
				propertyChanging: (bindable, oldValue, newValue) =>
				{
					((PagerIndicatorDots)bindable).ItemsSourceChanging();
				},
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					((PagerIndicatorDots)bindable).ItemsSourceChanged();
				}
		);

		public IList ItemsSource
		{
			get
			{
				return (IList)GetValue(ItemsSourceProperty);
			}
			set
			{
				SetValue(ItemsSourceProperty, value);
			}
		}

		public static BindableProperty SelectedItemProperty =
			BindableProperty.Create(
				nameof(SelectedItem),
				typeof(object),
				typeof(PagerIndicatorDots),
				null,
				BindingMode.TwoWay,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					((PagerIndicatorDots)bindable).SelectedItemChanged();
				}
		);

		public object SelectedItem
		{
			get
			{
				return GetValue(SelectedItemProperty);
			}
			set
			{
				SetValue(SelectedItemProperty, value);
			}
		}

		void ItemsSourceChanging()
		{
			if (ItemsSource != null)
				_selectedIndex = ItemsSource.IndexOf(SelectedItem);
		}

		void ItemsSourceChanged()
		{
			if (ItemsSource == null) return;

			// Dots *************************************
			var countDelta = ItemsSource.Count - Children.Count;

			if (countDelta > 0)
			{
				for (var i = 0; i < countDelta; i++)
				{
					CreateDot();
				}
			}
			else if (countDelta < 0)
			{
				for (var i = 0; i < -countDelta; i++)
				{
					Children.RemoveAt(0);
				}
			}
			//*******************************************
		}

		void SelectedItemChanged()
		{

			var selectedIndex = ItemsSource.IndexOf(SelectedItem);
			var pagerIndicators = Children.Cast<Button>().ToList();

			foreach (var pi in pagerIndicators)
			{
				UnselectDot(pi);
			}

			if (selectedIndex > -1)
			{
				SelectDot(pagerIndicators[selectedIndex]);
			}
		}

		static void UnselectDot(Button dot)
		{
			dot.Opacity = 0.5;
		}

		static void SelectDot(Button dot)
		{
			dot.Opacity = 1.0;
		}
	}

	public class PagerIndicatorTabs : Grid
	{
		int _selectedIndex;

		public Color DotColor { get; set; }
		public double DotSize { get; set; }

		public PagerIndicatorTabs()
		{
			HorizontalOptions = LayoutOptions.CenterAndExpand;
			VerticalOptions = LayoutOptions.Center;
			DotColor = Color.Black;
			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					BackgroundColor = Color.Gray;
					break;
			}

			var assembly = typeof(PagerIndicatorTabs).GetTypeInfo().Assembly;
			foreach (var res in assembly.GetManifestResourceNames())
				System.Diagnostics.Debug.WriteLine("found resource: " + res);
		}

		void CreateTabs()
		{
			if (Children != null && Children.Count > 0) Children.Clear();

			foreach (var item in ItemsSource)
			{

				var index = Children.Count;
				var tab = new StackLayout
				{
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					Padding = new Thickness(7),
				};
				switch (Device.RuntimePlatform)
				{
					case Device.iOS:
						tab.Children.Add(new Image { Source = "pin.png", HeightRequest = 20 });
						tab.Children.Add(new Label { Text = "Tab " + (index + 1), FontSize = 11 });
						break;

					case Device.Android:
						tab.Children.Add(new Image { Source = "pin.png", HeightRequest = 25 });
						break;
				}

				var tgr = new TapGestureRecognizer();
				tgr.Command = new Command(() =>
				{
					SelectedItem = ItemsSource[index];
				});
				tab.GestureRecognizers.Add(tgr);
				Children.Add(tab, index, 0);
			}
		}

		public static BindableProperty ItemsSourceProperty =
			BindableProperty.Create(
				nameof(ItemsSource),
				typeof(IList),
				typeof(PagerIndicatorTabs),
				null,
				BindingMode.OneWay,
				propertyChanging: (bindable, oldValue, newValue) =>
				{
					((PagerIndicatorTabs)bindable).ItemsSourceChanging();
				},
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					((PagerIndicatorTabs)bindable).ItemsSourceChanged();
				}
		);

		public IList ItemsSource
		{
			get
			{
				return (IList)GetValue(ItemsSourceProperty);
			}
			set
			{
				SetValue(ItemsSourceProperty, value);
			}
		}

		public static BindableProperty SelectedItemProperty =
			BindableProperty.Create(
				nameof(SelectedItem),
				typeof(object),
				typeof(PagerIndicatorTabs),
				null,
				BindingMode.TwoWay,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					((PagerIndicatorTabs)bindable).SelectedItemChanged();
				}
		);

		public object SelectedItem
		{
			get
			{
				return GetValue(SelectedItemProperty);
			}
			set
			{
				SetValue(SelectedItemProperty, value);
			}
		}

		void ItemsSourceChanging()
		{
			if (ItemsSource != null)
				_selectedIndex = ItemsSource.IndexOf(SelectedItem);
		}

		void ItemsSourceChanged()
		{
			if (ItemsSource == null) return;

			this.ColumnDefinitions.Clear();
			foreach (var item in ItemsSource)
			{
				this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			}

			CreateTabs();
		}

		void SelectedItemChanged()
		{

			var selectedIndex = ItemsSource.IndexOf(SelectedItem);
			var pagerIndicators = Children.Cast<StackLayout>().ToList();

			foreach (var pi in pagerIndicators)
			{
				UnselectTab(pi);
			}

			if (selectedIndex > -1)
			{
				SelectTab(pagerIndicators[selectedIndex]);
			}
		}

		static void UnselectTab(StackLayout tab)
		{
			tab.Opacity = 0.5;
		}

		static void SelectTab(StackLayout tab)
		{
			tab.Opacity = 1.0;
		}
	}

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

	public class CarouselMetaSource : CarouselItemSource, ITabProvider
	{
		public string Title { get; set; }
		public Color Background { get; set; }
		public string ImageSource { get; set; }
	}

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
