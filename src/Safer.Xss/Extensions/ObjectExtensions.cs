using System.Collections.Generic;

namespace G42.Safer.Xss.Extensions
{
    public static class ObjectExtensions
    {
        public static object ToSaferObject(this object input)
        {
            if (input == null)
            {
                return null;
            }

            var inputType = input.GetType();

            var properties = inputType.GetProperties();

            foreach (var prop in properties)
            {
                //if we can't write a property, what is the point?
                if (!prop.CanWrite)
                {
                    continue;
                }

                var propType = prop.PropertyType;

                //let's try to detect nested objects and recursively do our thing
                if (!propType.IsValueType && propType != typeof(string)
                                          && !(propType == typeof(List<string>) || propType == typeof(IEnumerable<string>))
                   )
                {
                    var indexedParameters = prop.GetIndexParameters();
                    var isIndexed = indexedParameters.Length != 0;

                    //Due to a limitation in Reflection, we cannot generically determine values for indexed properties (e.g. Dictionaries)
                    //Therefore if you decide to emit to a client, an indexed property; you will have to handle that yourself b/c it
                    //requires implementation details in order to determine some things.
                    //Please feel free to send a PR if you're more clever than I.
                    if (!isIndexed)
                    {
                        var propValue = prop.GetValue(input, null);

                        if (propValue != null)
                        {
                            prop.SetValue(input, propValue.ToSaferObject());
                        }
                    }
                }

                //raw string properties
                if (propType == typeof(string))
                {
                    var indexedParameters = prop.GetIndexParameters();
                    var isIndexed = indexedParameters.Length != 0;

                    //we need to avoid any indexed properties hitting this
                    if (!isIndexed)
                    {
                        var value = prop.GetValue(input, null);

                        var newValue = ((string)value).ToSaferString();

                        prop.SetValue(input, newValue);
                    }
                }

                //list and IEnumerables
                if (propType == typeof(List<string>) || propType == typeof(IEnumerable<string>))
                {
                    var values = (List<string>)prop.GetValue(input, null);

                    var newList = new List<string>();

                    if (values != null)
                    {
                        foreach (var value in values)
                        {
                            var newValue = ((string)value).ToSaferString();

                            newList.Add(newValue);

                            prop.SetValue(input, newList);
                        }
                    }
                }
            }

            return input;
        }
    }
}
