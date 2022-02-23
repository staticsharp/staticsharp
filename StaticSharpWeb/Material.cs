﻿using StaticSharpGears;
using StaticSharpWeb.Html;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StaticSharpWeb {

    public interface IMaterial {
        string Title { get; }
        Paragraph Description { get; }
        IImage TitleImage { get; }
    }

    internal interface IFontProvider {
        public Font Font { get; }
    }

    public abstract class Material : IMaterial, IInline, IPage, IPlainTextProvider, IFontProvider {
        //public class TChildren : List<object> { }

        public virtual string Title {
            get {
                var n = GetType().Namespace;
                return n[(n.LastIndexOf('.') + 1)..];
            }
        }

        public virtual Paragraph Description => null;
        public virtual MaterialContent Content => null;
        public virtual Footer Footer => null;
        public virtual int ContentWidth => 800;
        public virtual IImage TitleImage => null;
        public virtual RightSideBar RightSideBar => null;
        public virtual LeftSideBar LeftSideBar => null;

        public virtual Font Font => new(Path.Combine(AbsolutePath(), "Fonts", "roboto"), FontWeight.Regular);

        public async Task<string> GeneratePageHtmlAsync(Context context) {
            context.Parents = context.Parents.Prepend(this);
            context.Includes.Require(new Script(AbsolutePath("StaticSharp.js")));
            context.Includes.Require(new Style(AbsolutePath("Normalization.scss")));

            context.Includes.Require(new Style(AbsolutePath("Debug.scss")));

            var head = new Tag("head"){
                    new Tag("meta", new{ charset = "utf-8" }),
                    new Tag("meta", new{
                        name="viewport",
                        content = "width=device-width,initial-scale=1.0,maximum-scale=1.0,user-scalable=0"
                    }),
                    new Tag("title"){
                        Title
                    },
                    //<meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=1.0; user-scalable=0;"/>
                };

            var document = new Tag(null) {
                new Tag("!doctype",new{ html = ""}),
                head,
                await GetBodyAsync(context)
            };
            head.Add(await context.Includes.GenerateScriptAsync(context.Storage));
            head.Add(await context.Includes.GenerateFontAsync(context.Storage));
            head.Add(await context.Includes.GenerateStyleAsync(context.Storage));
            return document.GetHtml();
        }

        public virtual async Task<Tag> GetBodyAsync(Context context) {
            var font = Font.GenerateUsageCss(context);

            var body = new Tag("body", 
                new {
                    style = SoftObject.MergeObjects(
                        new {
                            Display = "none",
                            BackgroundColor = context.Theme.Surface,
                            Color = context.Theme.OnSurface,
                        },
                        font
                    )
                }                
                ){
                new JSCall(AbsolutePath("Material.js"), ContentWidth).Generate(context),
            };
            var tasks = new List<Task<INode>>();
            if (RightSideBar != null) {
                body.Add(await RightSideBar.GenerateSideBarAsync(context));
            }
            if (LeftSideBar != null) {
                body.Add(await LeftSideBar.GenerateSideBarAsync(context));
            }

            if (Title != null) {
                body.Add(new Tag("h1", new { id = "Header" }) { Title });
            }
            if(TitleImage != null) {
                tasks.Add(TitleImage.GenerateHtmlAsync(context));
            }
            if (Description != null) {
                tasks.Add(Description.GenerateHtmlAsync(context));
            } else {
                //warning
            }

            if (Content != null) {
                tasks.AddRange(Content.Select(x => x.GenerateHtmlAsync(context)));
            }
            if (Footer != null) {
                tasks.Add(Footer.GenerateHtmlAsync(context));
            }
            foreach (var item in await Task.WhenAll(tasks)) {
                body.Add(item);
            }

            return body;
        }

        public async Task<INode> GenerateHtmlAsync(Context context) {
            var representative = this as  StaticSharpEngine.IRepresentative;
            var uri = context.Urls.ProtoNodeToUri(representative?.Node);
            if (uri == null) {
                throw new NullReferenceException();//todo: special exception
            }

            var result = new Tag("a", new { href = uri }) { Title };

            if (Description != null) {
                result.Attributes["title"] = await Description.GetPlaneTextAsync(context);
            }

            return result;
        }

        public async Task<string> GetPlaneTextAsync(Context context) {
            return Title;
        }
    }

    public class MaterialContent : ElementContainer, ITextAnchorsProvider, IFillAnchorsProvider, IWideAnchorsProvider {}
}