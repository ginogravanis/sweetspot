namespace SweetSpot.Util
{
    public class StringValueAttribute : System.Attribute
    {
        protected string value;

        public StringValueAttribute(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }
    }
}
