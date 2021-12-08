﻿using StaticSharpWeb.Html;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace StaticSharpWeb {

    public interface IIncludes {

        void Require(IScript script);

        void Require(IStyle style);

        void Require(IFont font);

        Task<Tag> GenerateScriptAsync(IStorage storage);

        Task<Tag> GenerateFontAsync(IStorage storage);
        Task<Tag> GenerateStyleAsync(IStorage storage);
    }

    public class Includes : IIncludes {
        private readonly ConcurrentDictionary<string, IScript> scripts = new();
        private readonly ConcurrentDictionary<string, IStyle> styles = new();
        private readonly ConcurrentDictionary<string, IFont> fonts = new();
        private Style superStyle { get; set; }

        public void Require(IScript script) {
            foreach(var i in script.Dependencies) {
                Require(i);
            }

            var id = script.Key;
            if(!scripts.ContainsKey(id)) {
                scripts.TryAdd(id, script);
            }
        }

        public async Task<Tag> GenerateScriptAsync(IStorage storage) {
            var scriptCode = new StringBuilder();
            foreach(var i in scripts.Values) {
                scriptCode.AppendLine(await i.GenerateAsync(storage));
            }
            return new Tag("script") {
                new PureHtmlNode(scriptCode.ToString())
            };
        }

        public void Require(IStyle style) {
            var id = style.Key;
            if(!styles.ContainsKey(id)) {
                styles[id] = style;
            }
        }

        public async Task<Tag> GenerateStyleAsync(IStorage storage) {
            var styleCode = new StringBuilder();
            foreach(var style in styles.Values) {
                styleCode.Append(await style.GenerateAsync(storage));
            }
            
            GenerateSuperStyle();

            //var superStyle = new StringBuilder();
            //superStyle.Append(GenerateSuperStyle());

            //styleCode.Append(GenerateSuperStyle());
            return new Tag("style") {
                //new PureHtmlNode(styleCode.ToString())
                new PureHtmlNode(superStyle.ToString()) //uncomment
            };
        }

        public void GenerateSuperStyle() {
            //var stylesList = new StringBuilder();
            string styleList = "";
            styleList += "@import " + new RelativePath("_functions.scss") + ";";
            //stylesList.Append("@import " + new RelativePath("_functions.scss") + ";");
            foreach(var style in styles.Values) {
                //stylesList.Append("@import " + style.Path + ";");
                styleList += "@import " + style.Path + ";";
            }
            superStyle = new Style("");
            superStyle.GenerateSuperStyle(styleList);
        }

        // public StringBuilder GenerateSuperStyle() {
        //     var stylesList = new StringBuilder();
        //     stylesList.Append("@import " + new RelativePath("_functions.scss") + ";");
        //     foreach(var style in styles.Values) {
        //         stylesList.Append("@import " + style.Path + ";");
        //     }
        //     return stylesList;
        // }

        public void Require(IFont font) {
            var id = font.Key;
            if(!fonts.ContainsKey(id)) {
                fonts[id] = font;
            }
        }

        public async Task<Tag> GenerateFontAsync(IStorage storage) {
            var fontStyle = new StringBuilder();

            foreach(var i in fonts.Values) {
                fontStyle.AppendLine(await i.GenerateAsync(storage));
            }
            return new Tag("style") {
                new PureHtmlNode(fontStyle.ToString())
            };
        }
    }
}