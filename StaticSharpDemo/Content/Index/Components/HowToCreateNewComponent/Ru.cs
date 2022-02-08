﻿

namespace StaticSharpDemo.Content.Index.Components.HowToCreateNewComponent {

    [Representative]
    partial class Ru : Material {
        public override string Title => "Как создать новый компонент";
        public override Paragraph Description => new Paragraph() { "Компоненты для создания страниц." };

        public override MaterialContent Content => new() {
            $"сначала напишу все в кучу, а потом правильно распределю..",
            new Heading("SCSS"),
            $"Элемент отталкивается от предидущего. т.е. "
        };
    }

    
}
