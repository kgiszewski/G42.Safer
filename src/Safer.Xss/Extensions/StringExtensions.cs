using System.Web;

namespace G42.Safer.Xss.Extensions
{
    public static class StringExtensions
    {
        public static string ToSaferString(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return HttpUtility.HtmlEncode(input);
        }
    }
}
