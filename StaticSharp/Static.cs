﻿using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;



namespace StaticSharp {
    public static partial class Static {

        static Static() {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        }

        /*public static string ToInvariant(this float value) {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToInvariant(this double value) {
            return value.ToString(CultureInfo.CurrentCulture);
        }*/

        public static string GetThisFilePath([CallerFilePath] string callerFilePath = "") {
            return callerFilePath;
        }


        public static string GetThisFileDirectory([CallerFilePath] string callerFilePath = "") {
            return Path.GetDirectoryName(callerFilePath)!;
        }

        public static string GetThisFileNameWithoutExtension([CallerFilePath] string callerFilePath = "") {
            return Path.GetFileNameWithoutExtension(callerFilePath);
        }

        public static string GetThisFilePathWithNewExtension(string extension = "", [CallerFilePath] string callerFilePath = "") {
            var indexOfDot = callerFilePath.LastIndexOf('.');
            if (indexOfDot == -1)
                return callerFilePath;
            var path = callerFilePath[..(indexOfDot+1)] + extension;
            return path;
        }

        public static string MakeAbsolutePath(string subPath = "", [CallerFilePath] string callerFilePath = "") {
            var path = Path.GetFullPath(subPath, Path.GetDirectoryName(callerFilePath));
            return path;
        }

        public static string ToStringInvariant(this double value) => value.ToString(CultureInfo.InvariantCulture);
        public static string ToStringInvariant(this float value) => value.ToString(CultureInfo.InvariantCulture);        


    }
}