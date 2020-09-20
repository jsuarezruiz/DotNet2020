using System;
using System.ComponentModel;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.FastRenderers;
using DotNet2020.Controls;
using CheckBoxRenderer = DotNet2020.Droid.Renderers.CheckBoxRenderer;
using Color = Xamarin.Forms.Color;
using AAttribute = Android.Resource.Attribute;
using AColor = Android.Graphics.Color;
using AView = Android.Views.View;

[assembly: ExportRenderer(typeof(RendererCheckBox), typeof(CheckBoxRenderer))]
namespace DotNet2020.Droid.Renderers
{
    public class CheckBoxRenderer :	AppCompatCheckBox,
		IVisualElementRenderer,
		AView.IOnFocusChangeListener,
		CompoundButton.IOnCheckedChangeListener,
		ITabStop
	{
		bool _disposed;
		int? _defaultLabelFor;
		VisualElementTracker _tracker;
		VisualElementRenderer _visualElementRenderer;
        static int[][] _checkedStates = new int[][]
		{
            new int[] { AAttribute.StateEnabled, AAttribute.StateChecked },
			new int[] { AAttribute.StateEnabled, -AAttribute.StateChecked },
			new int[] { -AAttribute.StateEnabled, AAttribute.StateChecked },
			new int[] { -AAttribute.StateEnabled, -AAttribute.StatePressed },
		};

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		public CheckBoxRenderer(Context context) : base(context) => Init();

		public CheckBoxRenderer(Context context, int defStyleAttr) : base(context, null, defStyleAttr) => Init();

		void Init()
		{
			SoundEffectsEnabled = false;
			SetOnCheckedChangeListener(this);
			Tag = this;
			OnFocusChangeListener = this;

			this.SetClipToOutline(true);
		}

		void IVisualElementRenderer.SetElement(VisualElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (!(element is RendererCheckBox checkBox))
			{
				throw new ArgumentException("Element is not of type " + typeof(RendererCheckBox), nameof(element));
			}

			RendererCheckBox oldElement = Element;
			Element = checkBox;

			if (oldElement != null)
			{
				oldElement.PropertyChanged -= OnElementPropertyChanged;
			}

			element.PropertyChanged += OnElementPropertyChanged;

			if (_tracker == null)
			{
				_tracker = new VisualElementTracker(this);
			}

			if (_visualElementRenderer == null)
			{
				_visualElementRenderer = new VisualElementRenderer(this);
			}

			OnElementChanged(new ElementChangedEventArgs<RendererCheckBox>(oldElement as RendererCheckBox, Element));
		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<RendererCheckBox> e)
		{
			if (e.NewElement != null && !_disposed)
			{
				this.EnsureId();

				UpdateOnColor();
				UpdateIsChecked();
				UpdateBackgroundColor();
				UpdateBackground();
			}

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
		}

		protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == RendererCheckBox.ColorProperty.PropertyName)
			{
				UpdateOnColor();
			}
			else if (e.PropertyName == RendererCheckBox.IsCheckedProperty.PropertyName)
			{
				UpdateIsChecked();
			}
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
			{
				UpdateBackgroundColor();
			}
			else if (e.PropertyName == VisualElement.BackgroundProperty.PropertyName)
			{
				UpdateBackground();
			}

			ElementPropertyChanged?.Invoke(this, e);
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				_disposed = true;
				_tracker?.Dispose();
				_tracker = null;
				SetOnCheckedChangeListener(null);
				OnFocusChangeListener = null;

				if (Element != null)
				{
					Element.PropertyChanged -= OnElementPropertyChanged;

					Element = null;
				}
			}

			base.Dispose(disposing);
		}

		Size MinimumSize()
		{
			return Size.Zero;
		}

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			if (_disposed)
			{
				return new SizeRequest();
			}

			Measure(widthConstraint, heightConstraint);
			return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), MinimumSize());
		}

		void IOnCheckedChangeListener.OnCheckedChanged(CompoundButton buttonView, bool isChecked)
		{
			((IElementController)Element).SetValueFromRenderer(RendererCheckBox.IsCheckedProperty, isChecked);
		}

		void UpdateIsChecked()
		{
			if (Element == null || Control == null)
				return;

			Checked = Element.IsChecked;
		}

		protected virtual ColorStateList GetColorStateList()
		{
			var tintColor = Element.Color == Color.Default ? Color.Accent.ToAndroid() : Element.Color.ToAndroid();

			var list = new ColorStateList(
					_checkedStates,
					new int[]
					{
						tintColor,
						tintColor,
						tintColor,
						tintColor
					});

			return list;
		}

		void UpdateBackgroundColor()
		{
			if (Element.BackgroundColor == Color.Default)
				SetBackgroundColor(AColor.Transparent);
			else
				SetBackgroundColor(Element.BackgroundColor.ToAndroid());
		}

		void UpdateBackground()
		{
			Brush background = Element.Background;

			this.UpdateBackground(background);
		}

		void UpdateOnColor()
		{
			if (Element == null || Control == null)
				return;

			var mode = PorterDuff.Mode.SrcIn;

			CompoundButtonCompat.SetButtonTintList(Control, GetColorStateList());
			CompoundButtonCompat.SetButtonTintMode(Control, mode);
		}

		void IOnFocusChangeListener.OnFocusChange(AView v, bool hasFocus)
		{
			((IElementController)Element).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, hasFocus);
		}

		public void SetLabelFor(int? id)
		{
			if (_defaultLabelFor == null)
				_defaultLabelFor = LabelFor;

			LabelFor = (int)(id ?? _defaultLabelFor);
		}

		void IVisualElementRenderer.UpdateLayout() => _tracker?.UpdateLayout();
		VisualElement IVisualElementRenderer.Element => Element;
		AView IVisualElementRenderer.View => this;
		ViewGroup IVisualElementRenderer.ViewGroup => null;
		VisualElementTracker IVisualElementRenderer.Tracker => _tracker;

        protected RendererCheckBox Element { get; private set; }

        protected AppCompatCheckBox Control => this;

		AView ITabStop.TabStop => this;
	}
}