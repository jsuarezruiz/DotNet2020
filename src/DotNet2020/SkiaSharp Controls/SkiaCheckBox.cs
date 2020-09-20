using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Threading.Tasks;

namespace DotNet2020.Controls
{
    public class SkiaCheckBox : ContentView, IDisposable
    {
        const double DefaultSize = 20.0d;
        const float DefaultBorderWidth = 6.0f;

        SKCanvasView _skiaView;
        readonly TapGestureRecognizer _tapGestureRecognizer;

        public SkiaCheckBox()
        {
            InitializeCanvas();

            WidthRequest = HeightRequest = DefaultSize;
            HorizontalOptions = VerticalOptions = new LayoutOptions(LayoutAlignment.Start, false);
            Margin = new Thickness(6);

            Content = _skiaView;

            _tapGestureRecognizer = new TapGestureRecognizer();
            _tapGestureRecognizer.Tapped += OnTapped;
            GestureRecognizers.Add(_tapGestureRecognizer);
        }

        public static BindableProperty ColorProperty =
            BindableProperty.Create(nameof(Color), typeof(Color), typeof(SkiaCheckBox), Color.DeepPink);

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly BindableProperty IsCheckedProperty =
            BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(SkiaCheckBox), false, BindingMode.TwoWay,
                propertyChanged: OnIsCheckedChanged);

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        static async void OnIsCheckedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is SkiaCheckBox checkbox)) return;
            checkbox.CheckedChanged?.Invoke(checkbox, new CheckedChangedEventArgs((bool)newValue));
            await checkbox.AnimateCheckedChanged();
        }

        public event EventHandler<CheckedChangedEventArgs> CheckedChanged;

        void InitializeCanvas()
        {
            _skiaView = new SKCanvasView();
            _skiaView.PaintSurface += OnPaintSurface;
            _skiaView.WidthRequest = _skiaView.HeightRequest = DefaultSize;
        }
   
        void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            e?.Surface?.Canvas?.Clear();

            DrawStroke(e);

            if (IsChecked)
                DrawFill(e);
        }

        void DrawStroke(SKPaintSurfaceEventArgs e)
        {
            var imageInfo = e.Info;
            var canvas = e?.Surface?.Canvas;

            using (var stroke = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.ToSKColor(),
                StrokeWidth = DefaultBorderWidth,
                StrokeJoin = SKStrokeJoin.Round,
                IsAntialias = true
            })
            {
                var cornerRadius = Device.RuntimePlatform == Device.UWP ? 0 : DefaultBorderWidth;
                canvas.DrawRoundRect(DefaultBorderWidth, DefaultBorderWidth, imageInfo.Width - (DefaultBorderWidth * 2), imageInfo.Height - (DefaultBorderWidth * 2), cornerRadius, cornerRadius, stroke);
            }
        }

        void DrawFill(SKPaintSurfaceEventArgs e)
        {
            var skImageInfo = e.Info;
            var canvas = e?.Surface?.Canvas;

            using (var fill = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.ToSKColor(),
                StrokeJoin = SKStrokeJoin.Round,
                IsAntialias = true
            })
            {
                var cornerRadius = Device.RuntimePlatform == Device.UWP ? 0 : DefaultBorderWidth;
                canvas.DrawRoundRect(DefaultBorderWidth, DefaultBorderWidth, skImageInfo.Width - (DefaultBorderWidth * 2), skImageInfo.Height - (DefaultBorderWidth * 2), cornerRadius, cornerRadius, fill);
            }

            using (var checkPath = new SKPath())
            {
                checkPath.MoveTo(.2f * skImageInfo.Width, .5f * skImageInfo.Height);
                checkPath.LineTo(.425f * skImageInfo.Width, .7f * skImageInfo.Height);
                checkPath.LineTo(.8f * skImageInfo.Width, .275f * skImageInfo.Height);

                using (var glyph = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = Color.White.ToSKColor(),
                    StrokeWidth = DefaultBorderWidth,
                    IsAntialias = true
                })
                {
                    glyph.StrokeCap = SKStrokeCap.Butt;
                    canvas.DrawPath(checkPath, glyph);
                }
            }
        }

        void OnTapped(object sender, EventArgs e)
        {
            IsChecked = !IsChecked;
        }

        async Task AnimateCheckedChanged()
        {
            await _skiaView.ScaleTo(0.85, 100);
            _skiaView.InvalidateSurface();
            await _skiaView.ScaleTo(1, 100, Easing.BounceOut);
        }

        public void Dispose()
        {
            if (_skiaView != null)
                _skiaView.PaintSurface -= OnPaintSurface;

            if (_tapGestureRecognizer != null)
                _tapGestureRecognizer.Tapped -= OnTapped;

            GestureRecognizers.Clear();
        }
    }
}