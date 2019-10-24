using System;
using System.Collections.Generic;
using System.Linq;
 
namespace EnumExtension
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumText : Attribute
    {
		public EnumText(string text) { Text = text; }
		public string Text { get; set; }
    }
 
    public static class EnumExtension
    {
        private static Dictionary<Enum, string> _textCache = new Dictionary<Enum, string>();
 
        public static string GetText(this Enum _instance)
        {
            lock (_textCache)
            {
                if (_textCache.ContainsKey(_instance)) return _textCache[_instance];
 
                var instanceType = _instance.GetType();
 
                Func<Enum, string> enumToText = delegate(Enum enumElement)
                {
                    if (_textCache.ContainsKey(enumElement)) return _textCache[enumElement];
 
                    var attributes
                        = instanceType.GetField(enumElement.ToString()).GetCustomAttributes(typeof(EnumText), true);
                    if (attributes.Length == 0) return _instance.ToString();
 
                    var enumText = ((EnumText)attributes[0]).Text;
                    _textCache.Add(enumElement, enumText);
 
                    return enumText;
                };
 
                if (Enum.IsDefined(instanceType, _instance))
                {
                    return enumToText(_instance);
                }
                else if (instanceType.GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0)
                {
                    var instanceValue = Convert.ToInt64(_instance);
 
                    var enumes =
                        from Enum value in Enum.GetValues(instanceType)
                        where (instanceValue & Convert.ToInt64(value)) != 0
                        select value;
 
                    var enumSumValue = 
                        enumes.Sum(value => Convert.ToInt64(value));
 
                    if (enumSumValue != instanceValue) return _instance.ToString();
 
                    var enumText = string.Join(", ",
                        (from Enum value in enumes
                         select enumToText(value)).ToArray());
 
                    if (!_textCache.ContainsKey(_instance))
                    {
                        _textCache.Add(_instance, enumText);
                    }
 
                    return enumText;
                }
                else
                {
                    return _instance.ToString();
                }
            }
        }

		public static T GetEnumByText<T>(this string _str) where T : struct
		{
			foreach (Enum e in Enum.GetValues(typeof(T)))
				if (e.GetText() == _str)
					return (T)Enum.Parse(typeof(T), Enum.GetName(typeof(T), e), false);
			return (T)Enum.ToObject(typeof(T), 0);
		}
	}
}