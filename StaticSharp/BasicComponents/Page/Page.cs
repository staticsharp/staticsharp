﻿using StaticSharp.Gears;
using StaticSharp.Html;
using StaticSharp.Tree;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace StaticSharp {

    namespace Js {
        public class Page : BaseModifier {
            public double Width => NotEvaluatableValue<double>();
            public double Height => NotEvaluatableValue<double>();
            public double FontSize => NotEvaluatableValue<double>();

        }
    }

    namespace Gears {
        public class PageBindings<FinalJs> : BlockBindings<FinalJs> where FinalJs : new() {
            //public Binding<double> FontSize { set { Apply(value); } }

        }
    }


    




    public interface IMainVisual {
        void GetMeta(Dictionary<string,string> meta, Context context);
    }



    [Mix(typeof(PageBindings<Js.Page>))]
    [ConstructorJs]
    [RelatedScript("../../CrossplatformLibrary/Storage/Storage")]
    [RelatedStyle("../Normalization")]

    public abstract partial class Page : Block {
        protected virtual void Setup(Context context) {
            FontSize = 16;
        }
        public virtual Genome<IAsset>? Favicon => null;
        public virtual string? SiteName => null;
        public abstract string PageLanguage { get; }
        public abstract string Title { get; }
        public abstract object? MainVisual { get; }
        
        public abstract Inlines? Description { get; }
        public abstract Node VirtualNode { get; }

        protected override string TagName => "body";


        protected abstract Blocks BodyContent { get; }


        public Page([CallerLineNumber] int callerLineNumber = 0, [CallerFilePath] string callerFilePath = "")
            : base(callerLineNumber, callerFilePath) {
            CodeFontFamilies = new[] { new FontFamilyGenome("Roboto Mono") };
            FontFamilies = new(){ new FontFamilyGenome("Roboto") };
            Weight = StaticSharp.FontWeight.Regular;

        }

        protected override void AddRequiredInclues(Context context) {

            base.AddRequiredInclues(context);

            if (context.DeveloperMode) {
                var genome = RelatedFileAttribute.GetGenome(typeof(Page), "../Watch.js");
                context.AddScript(genome);
            }
            
        }

        private Tag GenerateMetaTags(Context context) {


            var meta = new Dictionary<string, string>();
            if (SiteName!=null)
                meta["og:site_name"] = SiteName;

            meta["og:title"] = Title;
            meta["twitter:title"] = Title;
            meta["twitter:card"] = "summary_large_image";

            var url = context.NodeToAbsoluteUrl(VirtualNode).ToString();
            meta["og:url"] = url;
            meta["twitter:url"] = url;

            if (Description != null) {
                string description = Description.GetPlaneText(context);
                meta["description"] = description;
                meta["og:description"] = description;
                meta["twitter:description"] = description;
            }

            meta["og:type"] = "website";

            if (MainVisual is IMainVisual mainVisual) {
                mainVisual.GetMeta(meta,context);
            }
            var result = new Tag() {
                meta.Select(x=>Tag.Meta(x.Key,x.Value))
            };

            return result;
        }


        public Tag? GenerateFavicon(Context context) {
            //<link rel="icon" type="image/x-icon" href="/images/favicon.ico">
            if (Favicon == null)
                return null;
            var asset = Favicon.Result;
            var url = context.AddAsset(asset);
            return new Tag("link") {
                ["rel"] = "icon",
                ["type"] = asset.GetMediaType(),
                ["href"] = url,
            };
        }

        public string GeneratePageHtml(Context context) {

            Setup(context);
            

            var head = new Tag("head"){
                    new Tag("meta"){["charset"] = "utf-8" },
                    new Tag("meta"){
                        ["name"]="viewport",
                        ["content"] = "width=device-width,initial-scale=1.0,maximum-scale=1.0,user-scalable=0"
                    },
                    new Tag("title"){
                        Title
                    },
                    GenerateMetaTags(context),
                    GenerateFavicon(context)
                    //<meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=1.0; user-scalable=0;"/>
                };


            var body = GenerateHtml(context,null);
            //body.Style["visibility"] = "hidden";


            var document = new Tag(null) {
                new Tag("!doctype"){ ["html"] = ""},
                new Tag("html") {
                    ["lang"] = PageLanguage,
                    Children ={
                        head,
                        body
                    }
                }
                
            };

            body.Add(
                new Tag("svg") {
                    Style = {
                        ["display"] = "none"
                    },
                    Children = {
                        new Tag("defs"){
                            context.SvgDefs.GetOrderedItems()
                        }
                    }
                }
                );



            head.Add(context.GenerateScript());
            head.Add(context.GenerateStyle());
            head.Add(context.GenerateFonts());

            return document.GetHtml();
        }

        protected override void ModifyHtml(Context context, Tag elementTag) {
            BodyContent.GenerateHtml(elementTag, context);
            //elementTag.Add(BodyContent.GenerateHtml(context));
            base.ModifyHtml(context, elementTag);
        }


    }

}