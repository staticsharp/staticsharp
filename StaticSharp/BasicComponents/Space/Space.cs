﻿using StaticSharp.Gears;
using StaticSharp.Html;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace StaticSharp {

    namespace Js {
        public class Space : Hierarchical {
            public float Before => NotEvaluatableValue<float>();
            public float Between => NotEvaluatableValue<float>();
            public float After => NotEvaluatableValue<float>();
        }
    }


    namespace Gears {
        public class SpaceBindings<FinalJs> : HierarchicalBindings<FinalJs> where FinalJs : new() {
            public Binding<float> Before { set { Apply(value); } }
            public Binding<float> Between { set { Apply(value); } }
            public Binding<float> After { set { Apply(value); } }
        }
    }

    [Mix(typeof(SpaceBindings<Js.Space>))]
    [ConstructorJs]
    public sealed partial class Space : Hierarchical, IBlock {
        protected override string TagName => "ws";
        /*public Space([CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            : base(callerFilePath, callerLineNumber) { }*/

        public Space(float before = 1, float between = 1, float after = 1, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            : base(callerFilePath, callerLineNumber) {
            if (before != 1) Before = before;
            if (between != 1) Between = between;
            if (after != 1) After = after;
        }

        public async Task<Tag> GenerateHtmlAsync(Context context, string? id = null) {
            await AddRequiredInclues(context);
            var tag = new Tag(TagName, id) {
                await CreateConstructorScriptAsync(context)
            };
            AddSourceCodeNavigationData(tag,context);
            return tag;
        }
    }
}
