﻿using System.Text;

namespace StaticSharp.Html {

    public class TextNode : INode {

        public TextNode(string text) => Content = text;

        public string Content { get; set; }

        public void WriteHtml(StringBuilder builder) {
            builder.Append(Content.ReplaceInvalidTagContentSymbols());
        }

    }
}