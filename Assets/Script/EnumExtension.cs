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
        private static Dictionary<Enum, string> textCache = new Dictionary<Enum, string>();

        public static string GetText(this Enum instance)
        {
            lock (textCache)
            {
                if (textCache.ContainsKey(instance)) return textCache[instance];

                var instanceType = instance.GetType();

                Func<Enum, string> enumToText = delegate (Enum enumElement)
                {
                    if (textCache.ContainsKey(enumElement)) return textCache[enumElement];

                    var attributes
                        = instanceType.GetField(enumElement.ToString()).GetCustomAttributes(typeof(EnumText), true);
                    if (attributes.Length == 0) return instance.ToString();

                    var enumText = ((EnumText)attributes[0]).Text;
                    textCache.Add(enumElement, enumText);

                    return enumText;
                };

                if (Enum.IsDefined(instanceType, instance))
                {
                    return enumToText(instance);
                }
                else if (instanceType.GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0)
                {
                    var instanceValue = Convert.ToInt64(instance);

                    var enumes =
                        from Enum value in Enum.GetValues(instanceType)
                        where (instanceValue & Convert.ToInt64(value)) != 0
                        select value;

                    var enumSumValue =
                        enumes.Sum(value => Convert.ToInt64(value));

                    if (enumSumValue != instanceValue) return instance.ToString();

                    var enumText = string.Join(", ",
                        (from Enum value in enumes
                         select enumToText(value)).ToArray());

                    if (!textCache.ContainsKey(instance))
                    {
                        textCache.Add(instance, enumText);
                    }

                    return enumText;
                }
                else
                {
                    return instance.ToString();
                }
            }
        }

        public static T GetEnumByText<T>(this string str) where T : Enum
        {
            foreach (Enum e in Enum.GetValues(typeof(T)))
                if (e.GetText() == str)
                    return (T)Enum.Parse(typeof(T), Enum.GetName(typeof(T), e), false);
            return (T)Enum.ToObject(typeof(T), 0);
        }
    }
}