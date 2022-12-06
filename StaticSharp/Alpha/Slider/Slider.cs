﻿using StaticSharp.Gears;
using StaticSharp.Html;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace StaticSharp {

    namespace Js {
        public class Slider : Block {
            public double Min => NotEvaluatableValue<double>();
            public double Max => NotEvaluatableValue<double>();
            public double Step => NotEvaluatableValue<double>();
            public double Value => NotEvaluatableValue<double>();
            public double ValueActual => NotEvaluatableValue<double>();
        }
    }

    namespace Gears {
        public class SliderBindings<FinalJs> : BlockBindings<FinalJs> where FinalJs : new() {
            public Binding<double> Min { set { Apply(value); } }
            public Binding<double> Max { set { Apply(value); } }
            public Binding<double> Step { set { Apply(value); } }
            public Binding<double> Value { set { Apply(value); } }
        }
    }


    [Mix(typeof(SliderBindings<Js.Slider>))]
    [ConstructorJs]
    public partial class Slider : Block {

        public static Func<Block> DefaultThumbConstructor = () => new Block() {
            BackgroundColor = new(e => e.Hover ? new Color(0, 0, 0, 0.5) : new Color(0, 0, 0, 0.25)),
            ["Radius"] = "() => Min(element.Width,element.Height) / 2"
        };

        public Block? Thumb { get; set; } = null;
        public Slider(Slider other, int callerLineNumber, string callerFilePath)
            : base(other, callerLineNumber, callerFilePath) { }

        public Slider([CallerLineNumber] int callerLineNumber = 0, [CallerFilePath] string callerFilePath = "") : base(callerLineNumber, callerFilePath) { }

        protected override async Task ModifyHtmlAsync(Context context, Tag elementTag) {
            var thumb = Thumb;
            if (thumb == null) {
                thumb = DefaultThumbConstructor();
            }

            elementTag.Add(
                await thumb.GenerateHtmlAsync(context, new Role(false,"Thumb"))
                );
            await base.ModifyHtmlAsync(context, elementTag);
        }

    }

}