using System.Collections;
using System.Reflection;

namespace JsonStringAttempt
{
    public static class JsonConverter
    {
        public static Dictionary<string, object> DictionaryDeserialize(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException("Data must not be empty");

            Dictionary<string, object> result = new Dictionary<string, object>();

            if (!data.StartsWith("{"))
            {
                throw new FormatException("Invalid json string");
            }

            data = data[1..^1];
            int level = 0;
            int start = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == '{')
                    level++;
                if (data[i] == '}')
                    level--;

                if (level == 0 && (data[i] == ',' || i == data.Length - 1))
                {
                    string keyValuePair = data.Substring(start, i - start + 1).Trim();
                    string[] pair = keyValuePair.Split(':', 2);

                    if (pair.Length != 2)
                    {
                        throw new FormatException("Invalid key-value pair format");
                    }

                    string key = pair[0].Trim('"');
                    string rawValue = pair[1].Trim();

                    if (rawValue.StartsWith('{'))
                    {
                        Dictionary<string, object> nestedObject = DictionaryDeserialize(rawValue);
                        result.Add(key, nestedObject);
                    }

                    else if (rawValue.StartsWith('['))
                    {
                        List<object> objectList = ListDeserialize(rawValue);
                        result.Add(key, objectList);
                    }

                    else if (rawValue.StartsWith('"'))
                    {
                        string value = rawValue.Trim(',').Trim('"');
                        result.Add(key, value);
                    }

                    else if (int.TryParse(rawValue.Trim(','), out int intValue))
                        result.Add(key, intValue);

                    else if (double.TryParse(rawValue, out double doubleValue))
                        result.Add(key, doubleValue);

                    else if (bool.TryParse(rawValue, out bool boolValue))
                        result.Add(key, boolValue);

                    start = i + 1;
                }
            }
            return result;
        }

        public static List<object> ListDeserialize(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException("Data must not be empty");

            if (!data.StartsWith("["))
                throw new FormatException("Invalid json string");

            List<object> result = new List<object>();
            data = data[1..^1];
            int start = 0;
            int level = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == '[')
                    level++;
                if (data[i] == ']')
                    level--;

                if (level == 0 && (data[i] == ',' || i == data.Length - 1))
                {
                    string value = data.Substring(start, i - start + 1);
                    if (value.StartsWith("["))
                    {
                        List<object> innerList = ListDeserialize(value);
                        result.Add(innerList);
                    }

                    else if (value.StartsWith('{'))
                    {
                        int counter = 1;
                        int indexPointer = start + 1;
                        while (counter != 0)
                        {
                            if (data[indexPointer] == '{')
                                counter++;
                            else if (data[indexPointer] == '}')
                                counter--;
                            indexPointer++;
                        }
                        value = data.Substring(start, indexPointer - start);
                        Dictionary<string, object> obj = DictionaryDeserialize(value);
                        i = indexPointer - 1;
                        result.Add(obj);
                    }

                    else if (value.StartsWith('"'))
                    {
                        string stringValue = value.Trim(',').Trim('"');
                        result.Add(stringValue);
                    }

                    else if (int.TryParse(value.Trim(','), out int integerValue))
                        result.Add(integerValue);

                    else if (double.TryParse(value.Trim(','), out double doubleValue))
                        result.Add(doubleValue);

                    else if (bool.TryParse(value.Trim(','), out bool boolValue))
                        result.Add(boolValue);

                    start = i + 1;
                }
            }
            return result;
        }

        public static void MapToObject<T>(Dictionary<string, object> data, ref T target) where T : class
        {
            foreach (var pair in data)
            {
                PropertyInfo property = typeof(T).GetProperties().FirstOrDefault(x => x.Name.ToLower() == pair.Key.ToLower());
                if (property != null && property.CanWrite)
                {
                    if (pair.Value is Dictionary<string, object>)
                    {
                        var nestedDict = pair.Value as Dictionary<string, object>;
                        if (nestedDict != null)
                        {
                            //  CANNOT MAP TO DYNAMIC OR OBJECT
                            //  when the recursive call is made it is made to an object and it does not have properties eventhough it is initialized
                            dynamic nestedObject = Activator.CreateInstance(property.PropertyType);
                            MapToObject<dynamic>(nestedDict, ref nestedObject);
                            property.SetValue(target, nestedObject);
                        }
                    }
                    else
                    {
                        var value = Convert.ChangeType(pair.Value, property.PropertyType);
                        property.SetValue(target, value);

                    }
                }
            }
        }

        public static T Deserialize<T>(string input) where T : class, new()
        {
            if (typeof(T) == typeof(IEnumerable))
            {
                T listResult = new();
                List<object> dataList = ListDeserialize(input);
                return listResult;
            }

            T result = new();
            Dictionary<string, object> map = DictionaryDeserialize(input);
            MapToObject<T>(map, ref result);
            return result;
        }

        public static string Serialize(object input)
        {
            Type info = input.GetType();

            if (info == typeof(string))
            {
                string value = $"\"{(string)input}\"";
                return value;
            }

            if (info == typeof(Int32))
            {
                int value = (int)input;
                return value.ToString();
            }

            if (info == typeof(double))
            {
                double value = (double)input;
                return value.ToString();
            }

            if (info.IsArray)
            {
                string arrayResult = "[";
                object[] objects = (object[])input;
                for (int i = 0; i < objects.Length; i++)
                {
                    if (i != 0)
                        arrayResult += ",";
                    arrayResult += Serialize(objects[i]);
                }
                arrayResult += "]";
                return arrayResult;
            }

            string result = "{";
            PropertyInfo[] properties = input.GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                if (property.CanRead)
                {
                    if (i != 0)
                    {
                        result += ",";
                    }

                    result += $"\"{property.Name}\":";
                    result += Serialize(property.GetValue(input));
                }
            }
            result += "}";
            return result;
        }
    }
}
