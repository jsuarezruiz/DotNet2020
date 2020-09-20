﻿using System;
using System.ComponentModel;
using CoreGraphics;
using DotNet2020.Controls;
using DotNet2020.iOS.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CheckBoxRenderer = DotNet2020.iOS.Renderers.CheckBoxRenderer;

[assembly: ExportRenderer(typeof(RendererCheckBox), typeof(CheckBoxRenderer))]
namespace DotNet2020.iOS.Renderers
{
	public class CheckBoxRenderer : ViewRenderer<RendererCheckBox, NativeCheckBox>
	{
		protected virtual float MinimumSize => 44f; // Apple docs
		bool _disposed;

		protected override void OnElementChanged(ElementChangedEventArgs<RendererCheckBox> e)
		{
			if (e.OldElement != null)
				e.OldElement.CheckedChanged -= OnElementCheckedChanged;

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(CreateNativeControl());
					Control.CheckedChanged += OnControlCheckedChanged;
				}

				Control.MinimumViewSize = MinimumSize;
				Control.IsChecked = Element.IsChecked;
				Control.IsEnabled = Element.IsEnabled;

				e.NewElement.CheckedChanged += OnElementCheckedChanged;
				UpdateTintColor();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == CheckBox.ColorProperty.PropertyName)
				UpdateTintColor();
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing && Control != null)
			{
				Control.CheckedChanged -= OnControlCheckedChanged;
			}

			base.Dispose(disposing);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			Control.UpdateDisplay();
		}

		protected override void SetAccessibilityLabel()
		{
			// If we have not specified an AccessibilityLabel and the AccessibiltyLabel is current bound to the Title,
			// exit this method so we don't set the AccessibilityLabel value and break the binding.
			// This may pose a problem for users who want to explicitly set the AccessibilityLabel to null, but this
			// will prevent us from inadvertently breaking UI Tests that are using Query.Marked to get the dynamic Title 
			// of the ImageButton.

			var elemValue = (string)Element?.GetValue(AutomationProperties.NameProperty);
			if (string.IsNullOrWhiteSpace(elemValue) && Control?.AccessibilityLabel == Control?.Title(UIControlState.Normal))
				return;

			base.SetAccessibilityLabel();
		}

		public override CGSize SizeThatFits(CGSize size)
		{
			var result = base.SizeThatFits(size);
			var height = Math.Max(MinimumSize, result.Height);
			var width = Math.Max(MinimumSize, result.Width);
			var final = Math.Min(width, height);
			return new CGSize(final, final);
		}

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			var sizeConstraint = base.GetDesiredSize(widthConstraint, heightConstraint);

			var set = false;

			var width = widthConstraint;
			var height = heightConstraint;
			if (sizeConstraint.Request.Width == 0)
			{
				if (widthConstraint <= 0 || double.IsInfinity(widthConstraint))
				{
					width = MinimumSize;
					set = true;
				}
			}

			if (sizeConstraint.Request.Height == 0)
			{
				if (heightConstraint <= 0 || double.IsInfinity(heightConstraint))
				{
					height = MinimumSize;
					set = true;
				}
			}

			if (set)
			{
				sizeConstraint = new SizeRequest(new Size(width, height), new Size(MinimumSize, MinimumSize));
			}

			return sizeConstraint;
		}

		protected virtual void UpdateTintColor()
		{
			if (Element == null)
				return;

			Control.CheckBoxTintColor = Element.Color;
		}

		void OnControlCheckedChanged(object sender, EventArgs e)
		{
			Element.IsChecked = Control.IsChecked;
		}

		void OnElementCheckedChanged(object sender, EventArgs e)
		{
			Control.IsChecked = Element.IsChecked;
		}
	}
}