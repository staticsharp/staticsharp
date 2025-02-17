﻿using StaticSharp.Gears;
using StaticSharp.Html;
using System.Drawing;
using System.Threading.Tasks;

namespace StaticSharp {

    public interface JPageSideMenus : JPage {
        public double ContentWidth { get; set; }
        public bool BarsCollapsed { get; }
        public double SideBarsIconsSize { get; set; }

    }


    [ConstructorJs]
    public abstract partial class PageSideMenus : Page {        
        protected override void Setup(Context context) {
            SideBarsIconsSize = 48;
            base.Setup(context);
        }
        public override string Title {
            get {
                var n = GetType().Namespace;
                return n[(n.LastIndexOf('.') + 1)..].Replace('_',' ');
            }
        }

        public virtual Block? TopBar => new Paragraph(Title) {
            Height = new(e=>Js.Num.Max(((JPageSideMenus)e.Root).SideBarsIconsSize, e.GetLayer().Height /*InternalHeight*/)),
            //["Height"] = "() => Min(element.Root.SideBarsIconsSize,element.InternalHeight)",
            TextAlignmentHorizontal = TextAlignmentHorizontal.Center,
            MarginsVertical = 0,
            Weight = FontWeight.ExtraLight,
            FontSize = new(e => ((JPageSideMenus)e.Root).SideBarsIconsSize),
        };


        public virtual Blocks? Content => null;
        public virtual Block? Footer => null;
        public virtual Block? LeftSideBarIcon => null;
        public virtual Block? LeftSideBar => null;
        public virtual Block? RightSideBarIcon => null;
        public virtual Block? RightSideBar => null;


        public override Blocks UnmanagedChildren => new Blocks();

        /*protected override Blocks  => new Blocks {
            {"LeftSideBarIcon"  ,LeftSideBarIcon},
            {"LeftSideBar" ,LeftSideBar},

            {"RightSideBarIcon" ,RightSideBarIcon},
            {"RightSideBar" ,RightSideBar},
            {"Content", new ScrollLayout {
                //FontSize = new(e=>Js.Storage.Store("scroll",()=>e.FontSize)),
                Content = new Column {
                    Children = {
                        { "TopBar", TopBar },
                        MainVisual ,
                        (Description != null) ? new Paragraph(Description) : null ,
                        new Block(){ 
                            Height = 1,
                            BackgroundColor = Color.Gray,
                            MarginBottom = 20
                        },
                        Content,
                        new Space(),
                        Footer
                    }
                }
            }
            }
        }*/

    }

}