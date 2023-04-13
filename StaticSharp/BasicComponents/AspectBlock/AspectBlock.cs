﻿using Scopes;
using StaticSharp.Gears;
using StaticSharp.Html;
using System.Runtime.CompilerServices;

namespace StaticSharp {

    public enum Fit { 
        Inside,
        Outside,
        Stretch
    }

    public interface JAspectBlock : JBlock {
        public double Aspect { get; }
        public Fit Fit { get; }
        public double GravityVertical { get; }
        public double GravityHorizontal { get; }
    }


    namespace Gears {
        public class AspectBlockBindings<FinalJs> : BlockBindings<FinalJs> {
            public Binding<double> Aspect { set { Apply(value); } }
            public Binding<Fit> Fit { set { Apply(value); } }
            public Binding<double> GravityVertical { set { Apply(value); } }
            public Binding<double> GravityHorizontal { set { Apply(value); } }
        }
    }


    [Mix(typeof(AspectBlockBindings<JAspectBlock>))]
    [ConstructorJs]
    public partial class AspectBlock : Block {
        public AspectBlock(AspectBlock other, int callerLineNumber, string callerFilePath): base(other, callerLineNumber, callerFilePath) {}
        public AspectBlock([CallerLineNumber] int callerLineNumber = 0, [CallerFilePath] string callerFilePath = "") : base(callerLineNumber, callerFilePath) {}

        protected static void SetNativeSize(Scopes.Group script, string elementVariableName, double width, double height) {
            script.Add($"{elementVariableName}.NativeWidth = {width}");
            script.Add($"{elementVariableName}.NativeHeight = {height}");
        }



    }

}