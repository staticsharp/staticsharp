﻿using StaticSharp.Gears;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StaticSharp {

    namespace Js {
        [Mix(typeof(Js.Link))]
        [Mix(typeof(Block))]
        public partial class LinkBlock {
        }
    }

    [Mix(typeof(LinkBindings<Js.LinkBlock>))]
    [Mix(typeof(BlockBindings<Js.LinkBlock>))]
    [ConstructorJs("Link")]
    [ConstructorJs]
    public partial class LinkBlock : Block {
        protected override string TagName => "a";

        StaticSharpEngine.INode? Node;
        public LinkBlock(string url, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0) : base(callerFilePath, callerLineNumber) {
            HRef = url;
        }
        public LinkBlock(StaticSharpEngine.INode node, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0) : base(callerFilePath, callerLineNumber) {
            Node = node;
        }
        public LinkBlock([CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0) : base(callerFilePath, callerLineNumber) {
        }
        public LinkBlock(LinkBlock other, string callerFilePath, int callerLineNumber) : base(other, callerFilePath, callerLineNumber) {
            Node = other.Node;
        }
        public override async IAsyncEnumerable<KeyValuePair<string, string>> GetGeneratedBundingsAsync(Context context) {
            await foreach (var i in base.GetGeneratedBundingsAsync(context)) { yield return i; }
            if (Node != null) {
                var url = context.NodeToUrl(Node);
                yield return new("HRef", $"\"{url}\"");
            }
        }


    }





}