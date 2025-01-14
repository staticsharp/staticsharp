﻿using StaticSharp.Gears;
using StaticSharp.Html;
using StaticSharp.Tree;
using System.Runtime.CompilerServices;


namespace StaticSharp {

    public interface JPage : JBaseModifier {
        public double Width { get; set; }
        public double Height { get; set; }
        public double FontSize { get; set; }

    }

    /*public interface IMainVisual {
        void GetMeta(Dictionary<string,string> meta, Context context);
    }*/

    [ConstructorJs]
    //[Scripts.Storage]
    [RelatedScript("Initialization")]
    [RelatedStyle("../Normalization")]

    public abstract partial class Page : BaseModifier {

        [Socket]
        public abstract Blocks UnmanagedChildren { get; }

        protected virtual void Setup(Context context) {
            FontSize = 16;
            //CodeFontFamilies = new[] { new FontFamilyGenome("Roboto Mono") };
            FontFamilies = new() { new FontFamilyGenome("Roboto") };
            Weight = FontWeight.Regular;
        }
        public virtual Genome<IAsset>? Favicon => null;
        public abstract string? SiteName { get; }
        public abstract string PageLanguage { get; }
        public abstract string Title { get; }
        public virtual Genome<IAsset>? MainVisual => null;
        
        public abstract Inlines? Description { get; }
        public abstract Node VirtualNode { get; }

        protected override string TagName => "body";

        protected override void AddRequiredInclues(Context context) {

            base.AddRequiredInclues(context);

            if (context.DeveloperMode) {
                var genome = new Scripts.DeveloperModeAttribute().GetGenome();
                context.AddScript(genome);
            }
            
        }

        private Tag GenerateMetaTags(Context context) {
            var result = new Tag();

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
                string description = Description.GetPlainText();
                //meta["description"] = description;
                meta["og:description"] = description;
                meta["twitter:description"] = description;

                result.Add(new Tag("meta") {
                    ["name"] = "description",
                    ["content"] = description
                });
            }

            meta["og:type"] = "website";

            CreateMainVisualMeta(meta, context);

            result.Add(meta.Select(x => Tag.Meta(x.Key, x.Value)));

            return result;
        }

        void CreateMainVisualMeta(Dictionary<string, string> meta, Context context) {
            if (MainVisual == null) return;
            var source = MainVisual.ToWebImage().Result;
            var imageInfo = source.GetImageInfo();
            var url = (context.BaseUrl + context.AddAsset(source)).ToString();
            meta["og:image"] = url;
            meta["og:image:width"] = imageInfo.Width.ToString();
            meta["og:image:height"] = imageInfo.Height.ToString();

            meta["twitter:image"] = url;
        }


        private Tag? GenerateFavicon(Context context) {
            //<link rel="icon" type="image/x-icon" href="/images/favicon.ico">
            if (Favicon == null)
                return null;
            var asset = Favicon.Result;

            var url = context.PathFromHostToCurrentPage.To(context.AddAsset(asset));
            return new Tag("link") {
                ["rel"] = "icon",
                ["type"] = asset.GetMediaType(),
                ["href"] = url,
            };
        }

        public string GeneratePageHtml(Context context) {

            Setup(context);
            

            


            var bodyTagAndScript = Generate(context);

            AddSvgDefs(context, bodyTagAndScript.Tag, bodyTagAndScript.Script);

            bodyTagAndScript.Script.Add($"{bodyTagAndScript.Tag.Id}.extras = {TagToJsValue("extras")}");
            bodyTagAndScript.Tag.Add(new Tag("extras", "extras"));


            var head = new Tag("head"){
                new Tag("meta"){["charset"] = "utf-8" },
                new Tag("meta"){
                    ["name"]="viewport",
                    ["content"] = "width=device-width,initial-scale=1.0,maximum-scale=5.0,user-scalable=yes"
                },
                new Tag("title"){
                    Title
                },
                context.HeadTags,


                GenerateMetaTags(context),
                GenerateFavicon(context)
                //<meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=1.0; user-scalable=0;"/>
            };


            var document = new Tag(null) {
                new Tag("!doctype"){ ["html"] = ""},
                new Tag("html") {
                    //["translate"]="no",
                    ["lang"] = PageLanguage,
                    Children ={
                        head,
                        bodyTagAndScript.Tag,
                        
                    }
                }
                
            };

            var initializationScript =
                new Scopes.C.Scope("function Initialize()"){
                    bodyTagAndScript.Script
                }.ToString();


            initializationScript = context.ReplaceTemporaryId(initializationScript);

            head.Add(new Tag("script", "create StaticSharp namespace") {
                new Html.PureHtmlNode("window.StaticSharp = {}")
            });

            head.Add(context.GenerateFontsScript());

            head.Add(context.GenerateScript());
            head.Add(context.GenerateStyle());
            

            head.Add(new Tag("script") {
                new PureHtmlNode(initializationScript)
            });

            return document.GetHtml();
        }

        private void AddSvgDefs(Context context, Tag tag, Scopes.Group script) {
            var svgDefsTags = context.SvgDefs.GetOrderedItems().ToArray();
            if (svgDefsTags.Length > 0) {

                var svgDefs = new Tag("svg", context.CreateId()) {
                    Style = {
                        ["display"] = "none"
                        },
                    Children = {
                        new Tag("defs"){
                            svgDefsTags
                        }
                    }
                };

                tag.Add(svgDefs);
                script.Add($"{tag.Id}.svgDefs = {TagToJsValue(svgDefs)}");
            }
        }


        /*public override void ModifyTagAndScript(Context context, Tag tag, Scopes.Group scriptBeforeConstructor, Scopes.Group scriptAfterConstructor) {
            base.ModifyTagAndScript(context, tag, scriptBeforeConstructor, scriptAfterConstructor);
            //tag["class"] = "nojs";

            
        }*/




    }

}