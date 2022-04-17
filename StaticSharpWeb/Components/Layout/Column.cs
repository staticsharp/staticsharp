﻿
using StaticSharpWeb.Html;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticSharpWeb {

    public interface IColumn { }

    public class Column : ElementContainer {

        string TagName { get; set; } = "div";
        /*public override IEnumerable<Task<Tag>> Before(Context context) {
            foreach (var i in base.Before(context)) yield return i;
            yield return Task.FromResult(
                new JSCall(AbsolutePath("Column.js"), null, "Before").Generate(context)
                );
        }      

        public override IEnumerable<Task<Tag>> After(Context context) {
            foreach (var i in base.After(context)) yield return i;
            yield return Task.FromResult(
                new JSCall(AbsolutePath("Column.js"), null, "After").Generate(context)
                );
        }*/

        public async Task<Tag> GenerateHtmlAsync(Context context) {
            return new Tag(TagName) {
                new JSCall(AbsolutePath("Item.js"), null, "Before").Generate(context),
                new JSCall(AbsolutePath("Column.js"), null, "Before").Generate(context),

                await Task.WhenAll(Items.Select(x=>x.GenerateHtmlAsync(context))),

                new JSCall(AbsolutePath("Item.js"), null, "After").Generate(context),
                new JSCall(AbsolutePath("Column.js"), null, "After").Generate(context),
            };
        }


    }



}
