﻿using StaticSharp.Gears;
using StaticSharp.Html;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace StaticSharp {

    namespace Gears {
        [System.Diagnostics.DebuggerNonUserCode]
        public class FlipperJs : Block {
            //public float Before => throw new NotEvaluatableException();
        }



        public class MFlipperBindings<FinalJs> : BlockBindings<FinalJs> where FinalJs : new() {
        }
    }


    [Mix(typeof(MFlipperBindings<FlipperJs>))]
    [ConstructorJs]
    public sealed class Flipper : Block, IBlock {

        public Block First { get; init; }
        public Block Second { get; init; }

        public Flipper([CallerLineNumber] int callerLineNumber = 0, [CallerFilePath] string callerFilePath = "")
            : base(callerLineNumber, callerFilePath) { }

        protected override async Task ModifyHtmlAsync(Context context, Tag elementTag) {            
            elementTag.Add(                
                await First.GenerateHtmlAsync(context,new Role(false,"First"))
            );
            elementTag.Add(
                await Second.GenerateHtmlAsync(context, new Role(false, "Second"))
            );
            await base.ModifyHtmlAsync(context, elementTag);
        }
    }
}
