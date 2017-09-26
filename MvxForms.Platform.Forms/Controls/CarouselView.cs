using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using MvxForms.Platform.Forms.Controls;
using Xamarin.Forms;

namespace MvxForms.Platform.Forms
{
	public class CarouselView : View
	{
		readonly RelativeLayout _relativeLayout;

		public CarouselView()
		{
			_relativeLayout = new RelativeLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
		}


		public CarouselLayout.IndicatorStyleEnum IndicatorStyle
		{
			get
			{
				return (CarouselLayout.IndicatorStyleEnum)GetValue(IndicatorStyleProperty);
			}
			set
			{
				SetValue(IndicatorStyleProperty, value);
			}
		}

		/// <summary>
		/// The indicator style property.
		/// </summary>
		public static readonly BindableProperty IndicatorStyleProperty =
			BindableProperty.Create(
				   nameof(IndicatorStyle),
				typeof(CarouselLayout.IndicatorStyleEnum),
				typeof(CarouselView),
				0,
				BindingMode.TwoWay,
				propertyChanged: async (bindable, oldValue, newValue) =>
				{
					await ((CarouselView)bindable).SetCarouseMainContent();
				}
		);

		/// <summary>
		/// Sets the content of the carouse main.
		/// </summary>
		/// <returns>The carouse main content.</returns>
		async Task SetCarouseMainContent()
		{
			var layout = CreateCarouselLayout();
			switch (IndicatorStyle)
			{
				case CarouselLayout.IndicatorStyleEnum.Dots:
					//创建点模式容器
					var dots = CreatePagerIndicatorContainer();
					_relativeLayout.Children.Add(layout,
						Constraint.RelativeToParent((parent) => { return parent.X; }),
						Constraint.RelativeToParent((parent) => { return parent.Y; }),
						Constraint.RelativeToParent((parent) => { return parent.Width; }),
						Constraint.RelativeToParent((parent) => { return parent.Height; })
						);
					//添加点布局
					_relativeLayout.Children.Add(dots,
						Constraint.Constant(0),
						Constraint.RelativeToView(layout, (parent, sibling) => { return sibling.Height - 18; }),
						Constraint.RelativeToParent(parent => parent.Width),
						Constraint.Constant(18)
						);
					break;
				case CarouselLayout.IndicatorStyleEnum.Tabs:

					break;
				default:
					break;
			}

		}

		/// <summary>
		/// Creates the pages carousel.
		/// </summary>
		/// <returns>The pages carousel.</returns>
		CarouselLayout CreateCarouselLayout()
		{
			var carousel = new CarouselLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				IndicatorStyle = IndicatorStyle,
				ItemTemplate = new DataTemplate(typeof(Nullable))
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


	}

	public interface ITabProvider
	{
		string ImageSource { get; set; }
	}


	/// <summary>
	/// 引导页点模式布局
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

}
