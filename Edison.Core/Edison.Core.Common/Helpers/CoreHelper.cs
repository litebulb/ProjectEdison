using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Edison.Core.Common
{
    public static class CoreHelper
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        public static Task TaskForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            Task task = new Task(() =>
            {
                foreach (T item in enumeration)
                {
                    action(item);
                }
            });
            task.Start();
            return task;
        }

        public static string GetWildCardExpression(string value)
        {
            return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
        }
    }
}
