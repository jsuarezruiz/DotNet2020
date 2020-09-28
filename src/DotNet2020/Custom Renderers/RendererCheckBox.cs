using System;
using Xamarin.Forms;

namespace DotNet2020.Controls
{
	public class RendererCheckBox : View
	{
		public static readonly BindableProperty IsCheckedProperty =
			BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(RendererCheckBox), false,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					((RendererCheckBox)bindable).CheckedChanged?.Invoke(bindable, new CheckedChangedEventArgs((bool)newValue));
				}, defaultBindingMode: BindingMode.TwoWay);

		public static readonly BindableProperty ColorProperty =
			BindableProperty.Create(nameof(Color), typeof(Color), typeof(RendererCheckBox), Color.Default);

		public Color Color
		{
			get => (Color)GetValue(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		public bool IsChecked
		{
			get => (bool)GetValue(IsCheckedProperty);
			set => SetValue(IsCheckedProperty, value);
		}

		public event EventHandler<CheckedChangedEventArgs> CheckedChanged;
	}
}