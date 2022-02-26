using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Framework.Extensions
{
    public static class ExceptionExtension
    {
        public static string GetExceptionDetailMessage(this Exception actualException)
        {
            var innerExceptionMessage = actualException.FromHierarchy(innerEx => innerEx.InnerException).Select(inner => inner.Message);
            string errorMessage = $"Message:{actualException.Message}{Environment.NewLine}InnerException:{string.Join(Environment.NewLine, innerExceptionMessage)}{Environment.NewLine}Stacktrace:{actualException.StackTrace}";
            return errorMessage;
        }

        private static IEnumerable<TSource> FromHierarchy<TSource>(this TSource source, Func<TSource, TSource> nextItem, Func<TSource, bool> canContinue)
        {
            for (var current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }

        private static IEnumerable<TSource> FromHierarchy<TSource>(this TSource source, Func<TSource, TSource> nextItem) where TSource : class
        {
            return FromHierarchy(source, nextItem, s => s != null);
        }
        
        public static void WriteErrorInTextFile(Exception ex)
        {
            string errorTextFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpgradeError.txt");
            string shortDateTime = $"{DateTime.Now:G}";

            using StreamWriter file = new StreamWriter(errorTextFile, true);
            file.WriteLine();
            file.WriteLine();
            file.WriteLine("-----------------------");
            file.Write($"{shortDateTime} ERROR : {ex.GetExceptionDetailMessage()}");
        }
    }
}
