using System;
using System.Diagnostics;
using DotNet2020.Controls;
using Xamarin.Forms;

namespace DotNet2020.Views
{
    public partial class MeasureCustomCheckBoxView : ContentPage
    {
        readonly Stopwatch _stopWatch;

        public MeasureCustomCheckBoxView()
        {
            _stopWatch = new Stopwatch();
            _stopWatch.Start();

            InitializeComponent();

            //AddItems(100);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _stopWatch.Stop();

            TimeSpan ts = _stopWatch.Elapsed;
            InfoLabel.Text = $"{ts.TotalMilliseconds} ms";
        }

        void AddItems(int numberOfItems)
        {
            for (int i = 0; i < numberOfItems; i++)
            {
                ParentLayout.Children.Add(new CustomCheckBox());
            }
        }
    }
}