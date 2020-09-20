using Xamarin.Forms;

namespace DotNet2020.TypeConverters
{
    public class ContentTypeConverter : TypeConverter
    {
        public override object ConvertFromInvariantString(string value)
        {
            Label label = new Label
            {
                Text = value
            };

            return label;
        }
    }
}