using System;
using System.Collections.Generic;
using System.Linq;

namespace EnumExtension
{
    [AttributeUsage(AttributeTargets.Field)]
    /// <summary> enumにTextを付記するAttribute </summary>
    public class EnumText : Attribute
    {
        public EnumText(string text) { Text = text; }
        /// <summary> 付記Text </summary>
        public string Text { get; set; }
    }

    public static class EnumExtension
    {
        /// <summary> 一度読み込んだEnumTextを保存する </summary>
        private static Dictionary<Enum, string> textCache = new Dictionary<Enum, string>();

        /// <summary> enumから付記Textを取得する </summary>
        /// <param name="instance">enum</param>
        public static string GetText(this Enum instance)
        {
            lock (textCache)
            {
                if (textCache.ContainsKey(instance)) return textCache[instance];

                var instanceType = instance.GetType();

                string enumToText(Enum enumElement)
                {
                    if (textCache.ContainsKey(enumElement)) return textCache[enumElement];

                    var attributes
                        = instanceType.GetField(enumElement.ToString()).GetCustomAttributes(typeof(EnumText), true);
                    if (attributes.Length == 0) return instance.ToString();

                    var enumText = ((EnumText)attributes[0]).Text;
                    textCache.Add(enumElement, enumText);

                    return enumText;
                }

                if (Enum.IsDefined(instanceType, instance))
                    return enumToText(instance);
                if (instanceType.GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0)
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
                        textCache.Add(instance, enumText);

                    return enumText;
                }
                return instance.ToString();
            }
        }

        /// <summary> Textから対応するenumを取得する </summary>
        /// <param name="str">Text文字列</param>
        /// <typeparam name="T">enum型</typeparam>
        public static T GetEnumByText<T>(this string str) where T : Enum
        {
            foreach (Enum e in Enum.GetValues(typeof(T)))
                if (e.GetText() == str)
                    return (T)Enum.Parse(typeof(T), Enum.GetName(typeof(T), e), false);
            return (T)Enum.ToObject(typeof(T), 0);
        }

        /// <summary> enumからコンテナを取得する </summary>
        /// <typeparam name="T">enum型</typeparam>
        public static IEnumerable<T> GetEnumerable<T>() where T : Enum
        {
            foreach (T e in Enum.GetValues(typeof(T)))
                yield return e;
        }
    }
}