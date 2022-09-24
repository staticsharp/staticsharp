﻿using StaticSharp;
using StaticSharp.BasicComponents.Page;
using StaticSharp.Gears;
using StaticSharp.Html;
using StaticSharpWeb.Html;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StaticSharp {


    public class PageJs : BaseModifierJs {
        public float WindowWidth => NotEvaluatableValue<float>();
        public float WindowHeight => NotEvaluatableValue<float>();
        public float DevicePixelRatio => NotEvaluatableValue<float>();
        public bool Touch => NotEvaluatableValue<bool>();
        public bool UserInteracted => NotEvaluatableValue<bool>();
        public float FontSize => NotEvaluatableValue<float>();

    }

    namespace Gears {
        public class PageBindings<FinalJs> : BaseModifierBindings<FinalJs> where FinalJs : new() {
            public Binding<float> FontSize { set { Apply(value); } }

        }
    }




    [Mix(typeof(PageBindings<PageJs>))]
    [ConstructorJs]
    [RelatedScript("../Watch")]
    [RelatedScript("../Color")]
    //[RelatedScript("Cookies")]
    [RelatedStyle("../Normalization")]

    public abstract partial class Page : BaseModifier, IPageGenerator, IPlainTextProvider {
        protected virtual Task Setup(Context context) {
            FontSize = 16;
            return Task.CompletedTask;
        }


        public abstract string Title { get; }

        
        protected override string TagName => "body";

        public Page([CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
            : base(callerFilePath, callerLineNumber) {

            FontFamilies = new[] { new FontFamily("Roboto") };
            FontStyle = new FontStyle(FontWeight.Regular);
        }

        public async Task<string> GeneratePageHtmlAsync(Context context) {

            await Setup(context);




            var head = new Tag("head"){
                    new Tag("meta"){["charset"] = "utf-8" },
                    new Tag("meta"){
                        ["name"]="viewport",
                        ["content"] = "width=device-width,initial-scale=1.0,maximum-scale=1.0,user-scalable=0"
                    },
                    new Tag("title"){
                        Title
                    },
                    //<meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=1.0; user-scalable=0;"/>
                };


            var body = await GenerateHtmlAsync(context);



            var document = new Tag(null) {
                new Tag("!doctype"){ ["html"] = ""},
                head,
                body
            };

            body.Add(
                new Tag("svg") {
                    Style = {
                        ["display"] = "none"
                    },
                    Children = {
                        new Tag("defs"){
                            await context.SvgDefs.GetAllAsync()
                        }
                    }
                }
                );



            head.Add(context.GenerateScript());
            head.Add(context.GenerateStyle());
            head.Add(await context.GenerateFontsAsync());

            return document.GetHtml();
        }



        public virtual async Task<Tag> GenerateHtmlAsync(Context context, string? id = null) {

            await AddRequiredInclues(context);

            context = ModifyContext(context);

            var tag = new Tag(TagName, id) { };

            ModifyTag(tag);

            tag.Add(await CreateConstructorScriptAsync(context));

            tag.Add(await GenerateChildrenHtmlAsync(context, tag));

            return tag;
        }

        protected abstract Task<Tag> GenerateChildrenHtmlAsync(Context context, Tag elementTag);

        public async Task<string> GetPlaneTextAsync(Context context) {
            return Title;
        }
    }

}