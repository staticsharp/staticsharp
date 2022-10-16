function Paragraph(element) {

    Block(element)


    element.Reactive = {
        Selectable: true,
        InternalWidth: () => Sum(element.MaxContentWidth,element.PaddingLeft, element.PaddingRight),

        InternalHeight: undefined,

        MaxContentWidth: undefined,
        MinContentWidth: undefined,

        MaxContentHeight: undefined,
        MinContentHeight: undefined,

        PaddingLeft: () => (element.BackgroundColor != undefined) ? 10 : undefined,
        PaddingRight: () => (element.BackgroundColor != undefined) ? 10 : undefined,
        PaddingTop: () => (element.BackgroundColor != undefined) ? 8 : undefined,
        PaddingBottom: () => (element.BackgroundColor != undefined) ? 8 : undefined,

        MarginLeft: () => (element.BackgroundColor != undefined) ? 0 : 10,
        MarginRight: () => (element.BackgroundColor != undefined) ? 0 : 10,
        MarginTop: () => (element.BackgroundColor != undefined) ? 0 : 8,
        MarginBottom: () => (element.BackgroundColor != undefined) ? 0 : 8,

    }

    /*new Reaction(() => {
        console.log(element, element.Child(0))
    })*/


    /*new Reaction(() => {
        let l = element.PaddingLeft
        let r = element.PaddingRight
        let w = element.Width
        if (l + r > w) {
            let m = w / (l + r)
            l *= m
            r *= m
        }
        element.style.paddingLeft = ToCssSize(l)
        element.style.paddingRight = ToCssSize(r)
    })

    new Reaction(() => {
        element.style.paddingTop = ToCssSize(element.PaddingTop)
    })
    new Reaction(() => {
        element.style.paddingBottom = ToCssSize(element.PaddingBottom)
    })*/


    new Reaction(() => {

        const testFontSize = 128;

        let content = element.children[0]

        //content.style.position = "initial"
        content.style.fontSize = testFontSize + "px";
        content.style.width = "min-content"
        var minWidthRect = content.getBoundingClientRect()
        content.style.width = "max-content"
        var maxWidthRect = content.getBoundingClientRect()

        content.style.fontSize = ""
        content.style.width = ""


        element.MaxContentWidth = () =>  element.HierarchyFontSize / testFontSize * maxWidthRect.width
        element.MinContentWidth = () =>  element.HierarchyFontSize / testFontSize * minWidthRect.width
        element.MaxContentHeight = () => element.HierarchyFontSize / testFontSize * minWidthRect.height
        element.MinContentHeight = () => element.HierarchyFontSize / testFontSize * maxWidthRect.height

    })

    new Reaction(() => {
        let content = element.children[0]
        /*
        Left,
        Center,
        Right,
        Justify,
        JustifyIncludingLastLine
         */
        

        if (element.TextAlignmentHorizontal == "JustifyIncludingLastLine") {
            content.style.textAlign = "justify"
            content.style.textAlignLast = "justify"
        } else {
            content.style.textAlignLast = ""
            if (element.TextAlignmentHorizontal === undefined) {
                content.style.textAlign = ""
            } else {
                content.style.textAlign = element.TextAlignmentHorizontal.toLowerCase()
            }
        }
    })


    new Reaction(() => {
        //console.log("element.HierarchyFontSize", element.HierarchyFontSize, element)
        //console.log("element.Modifier", element, element.Modifier)
        //console.log("element.Modifier.HierarchyFontSize", element, element.Modifier.HierarchyFontSize)
        element.style.width = ToCssSize(element.Width)

        let content = element.children[0]

        content.style.transformOrigin = ""
        content.style.transform = ""
        content.style.width = ""
        content.style.display = ""
        content.style.left = ToCssSize(element.PaddingLeft)
        content.style.top = ToCssSize(element.PaddingTop)

        let minContentWidthWithPaddings = Sum(element.MinContentWidth, element.PaddingLeft, element.PaddingRight)

        if (minContentWidthWithPaddings > element.Width) {
            //element.title = "element.MinContentWidth > element.Width"

            

            let scale = Sum(element.Width, -element.PaddingLeft, -element.PaddingRight) / element.MinContentWidth

            if (scale > 0) {
                content.style.width = "min-content"
                content.style.transformOrigin = "top left"
                content.style.transform = `scale(${scale}, ${scale})`
                element.InternalHeight = Sum(element.MaxContentHeight * scale, element.PaddingTop, element.PaddingBottom)
            } else {
                content.style.display = "hidden"
            }

            
            return
        }

        let maxContentWidthWithPaddings = Sum(element.MaxContentWidth, element.PaddingLeft, element.PaddingRight)
        if (Math.abs(element.Width - maxContentWidthWithPaddings) < 0.001) {
            //element.title = "element.Width == element.MaxContentWidth"

            element.InternalHeight = Sum(element.MinContentHeight, element.PaddingTop, element.PaddingBottom) 
            content.style.width = "max-content"

            return
        }
        

        content.style.width = ToCssSize(Sum(element.Width, -element.PaddingLeft, -element.PaddingRight))

        var rect = content.getBoundingClientRect()
        element.InternalHeight = Sum(rect.height, element.PaddingTop, element.PaddingBottom)

    })

    HeightToStyle(element)
}
