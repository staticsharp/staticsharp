﻿using StaticSharp.Gears;

namespace StaticSharp {
    namespace Js {



        [ConvertToJs("window")]
        public static class Window {
            public static bool Touch => NotEvaluatableValue<bool>();
            public static bool UserInteracted => NotEvaluatableValue<bool>();
            public static double DevicePixelRatio => NotEvaluatableValue<double>();

        }

        public static class Math {

            [ConvertToJs("First({0})")]
            public static double First(params double[] value) => NotEvaluatableValue<double>();

            [ConvertToJs("Sum({0})")]
            public static double Sum(params double[] value) => NotEvaluatableValue<double>();

            [ConvertToJs("Min({0})")]
            public static double Min(params double[] value) => NotEvaluatableValue<double>();

            [ConvertToJs("Max({0})")]
            public static double Max(params double[] value) => NotEvaluatableValue<double>();
        }

    }


}

