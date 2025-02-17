using StaticSharp.Tree;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace StaticSharp.Alpha {
    public static partial class Static {
        public static Block? MenuItem(Node node, string? text = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0) {
            if (node.Representative == null) {
                return null;
            }
            text ??= node.Representative.Title;

            return new Paragraph(text,callerLineNumber, callerFilePath) {
                InternalLink = node,
                Margins = 0,
                PaddingsVertical = 10,
                PaddingsHorizontal = 20,

                Modifiers = { 
                    new Hover()
                },

                UnmanagedChildren = {
                    new Block {
                            BackgroundColor = new(e=>e.Parent.HierarchyForegroundColor),
                            Visibility = new(e=>e.Parent.AsHover().Value ? 0.10 : 0),
                            Depth = -1
                    }.FillHeight().FillWidth(),
                }
            };
        }
    }
}