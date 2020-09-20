using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DotNet2020.TypeConverters;
using Xamarin.Forms;

namespace DotNet2020.Controls
{
    public class TemplatedCheckBox : TemplatedView
    {
        const string ElementBackground = "PART_Background";
        const string ElementGlyph = "PART_Glyph";
        const string ElementContent = "PART_Content";

        TapGestureRecognizer _tapGestureRecognizer;
        Xamarin.Forms.Shapes.Shape _background;
        Xamarin.Forms.Shapes.Shape _glyph;

        public TemplatedCheckBox()
        {
            Initialize();
        }

        void Initialize()
        {
            _tapGestureRecognizer = new TapGestureRecognizer();

            UpdateIsEnabled();
        }

        public static readonly BindableProperty ColorProperty =
            BindableProperty.Create(nameof(Color), typeof(Brush), typeof(TemplatedCheckBox), Brush.DeepPink);

        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly BindableProperty IsCheckedProperty =
            BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(TemplatedCheckBox), false,
                propertyChanged: OnIsCheckedChanged);

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        static async void OnIsCheckedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is TemplatedCheckBox checkbox)) return;
            checkbox.CheckedChanged?.Invoke(checkbox, new CheckedChangedEventArgs((bool)newValue));
            await checkbox.AnimateCheckedChanged();
        }

        public static readonly BindableProperty ContentProperty =
            BindableProperty.Create(nameof(Content), typeof(View), typeof(TemplatedCheckBox), null);

        [TypeConverter(typeof(ContentTypeConverter))]
        public View Content
        {
            get => (View)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public event EventHandler<CheckedChangedEventArgs> CheckedChanged;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _background = GetTemplateChild(ElementBackground) as Xamarin.Forms.Shapes.Shape;
            _glyph = GetTemplateChild(ElementGlyph) as Xamarin.Forms.Shapes.Shape;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == IsEnabledProperty.PropertyName)
                UpdateIsEnabled();
        }

        void UpdateIsEnabled()
        {
            if (IsEnabled)
            {
                _tapGestureRecognizer.Tapped += OnCheckBoxTapped;
                GestureRecognizers.Add(_tapGestureRecognizer);
            }
            else
            {
                _tapGestureRecognizer.Tapped -= OnCheckBoxTapped;
                GestureRecognizers.Remove(_tapGestureRecognizer);
            }
        }

        void OnCheckBoxTapped(object sender, EventArgs e)
        {
            IsChecked = !IsChecked;

            if (IsChecked)
            {
                if (_background != null)
                    _background.Fill = Color;

                if (_glyph != null)
                    _glyph.Opacity = 1;
            }
            else
            {
                if (_background != null)
                    _background.Fill = Brush.Default;

                if (_glyph != null)
                    _glyph.Opacity = 0;
            }

            RaiseCheckedChanged();
        }

        async Task AnimateCheckedChanged()
        {
            if (_background.Parent is View parent)
            {
                await parent.ScaleTo(0.85, 100);
                await parent.ScaleTo(1, 100, Easing.BounceOut);
            }
        }

        void RaiseCheckedChanged()
        {
            var checkedChangedEventArgs = new CheckedChangedEventArgs(IsChecked);
            CheckedChanged?.Invoke(this, checkedChangedEventArgs);
        }
    }
}