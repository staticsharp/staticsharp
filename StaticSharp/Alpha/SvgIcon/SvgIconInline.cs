﻿using StaticSharp.Gears;
using StaticSharp.Html;
using System.Runtime.CompilerServices;


namespace StaticSharp {

    namespace Js {
        public interface SvgIconInline: Block, SvgIcon {
            public double BaselineOffset  { get; } 
        }
    }

    namespace Gears {
        public class SvgIconInlineBindings<FinalJs> : SvgIconBindings<FinalJs> {
            public Binding<double> BaselineOffset { set { Apply(value); } }
        }
    }

    [Mix(typeof(SvgIconInlineBindings<Js.SvgIconInline>))]
    [Mix(typeof(InlineBindings<Js.SvgIconInline>))]
    [ConstructorJs("SvgIcon")]
    [ConstructorJs]
    public partial class SvgIconInline : Inline {

        SvgIcons.Icon icon;
        public double Scale { get; set; } = 1;
        public SvgIconInline(SvgIcons.Icon icon, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0) : base(callerLineNumber, callerFilePath) {
            this.icon = icon;
        }

        public SvgIconInline(SvgIconInline other, string callerFilePath, int callerLineNumber) : base(other, callerLineNumber, callerFilePath) {
            icon = other.icon;
            Scale = other.Scale;
        }

        protected override void ModifyHtml(Context context, Tag elementTag) {

            var code = icon.GetSvg();
            elementTag["data-width"] = icon.Width;
            elementTag["data-height"] = icon.Height;
            if (Scale != 1) {
                elementTag["data-scale"] = Scale;
            }

            elementTag.Add(new PureHtmlNode(code));
            elementTag.Add(CreateScript_AssignPreviousTagToParentProperty("content"));

            base.ModifyHtml(context, elementTag);
        }


    }





}