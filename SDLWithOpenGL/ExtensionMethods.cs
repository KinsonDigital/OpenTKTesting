using System;
using System.Text;

namespace SDLWithOpenGL
{
    public static class ExtensionMethods
    {
        public static string Join(this string[] items, string excludeValue = "")
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items), "The value cannot be null");

            var result = new StringBuilder();

            for (int i = 0; i < items.Length; i++)
            {
                var joinItem = items[i] != excludeValue;

                if (joinItem)
                    result.Append($@"{items[i]}\");
            }


            return result.ToString().TrimEnd('\\');
        }
    }
}
