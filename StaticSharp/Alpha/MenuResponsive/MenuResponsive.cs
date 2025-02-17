﻿using AngleSharp.Dom;
using StaticSharp.Gears;
using StaticSharp.Html;
using System.Runtime.CompilerServices;

namespace StaticSharp {

    public interface JMenuResponsive : JBlockWithChildren {
        public double PrimaryGravity { get; }
        public double SecondaryGravity { get; }
        public bool DropdownExpanded { get; }

        public JBlock? Logo { get; }
        public JBlock Button { get; }
        public JBlockWithChildren Dropdown { get; }
    }

    [Scripts.LayoutUtils]
    [ConstructorJs]
    public partial class MenuResponsive : BlockWithChildren {

        protected static Color DefaultBackgroundColor => Color.FromGrayscale(0.9);

        [Socket]
        public Block? Logo { get; set; } = null;

        [Socket]
        public Block Button { get; set; } = new SvgIconBlock(SvgIcons.MaterialDesignIcons.Menu) {
            Visibility = new(e => e.Parent.AsMenuResponsive().Dropdown.Children.Any() ? 1 : 0),
            //BackgroundColor = new(e => e.Parent.AsMenuResponsive().DropdownExpanded ? DefaultBackgroundColor : Color.White),
            Modifiers = { 
                new Toggle(),
                new Cursor(CursorOption.Pointer),
            }
        };

        [Socket]
        public BlockWithChildren Dropdown { get; set; } = new LinearLayout() {
            Depth = 1, // TODO: ???
            Vertical = true,
            BackgroundColor = DefaultBackgroundColor,
            Paddings = 5,
            Visibility = new(e => e.Parent.AsMenuResponsive().DropdownExpanded ? 1 : 0),
            Y = new(e=>e.Parent.Height),
            X = new(e=>e.Parent.Width - e.Width),
            Modifiers = { 
                new BorderRadius(){
                    Radius= 5,
                },
                new Button{ 
                    Script = """console.log("clicked")""",
                }
            }

        };

        [Socket]
        public Block Glass { get; set; } = new Block() {
            Depth = -1,
            Width = 500,
            Height = 500,
            BackgroundColor = Color.Black,
        };


    }
}
