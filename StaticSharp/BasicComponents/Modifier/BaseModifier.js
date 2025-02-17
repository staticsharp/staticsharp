StaticSharpClass("StaticSharp.BaseModifier", (element) => {

    StaticSharp.Hierarchical(element)


    //TODO: fix file protocol
    /*if (element.parentElement.tagName == "A") {
        if (window.location.protocol == "file:") {
            let a = element.parentElement
            let href = a.getAttribute("href")
            let AbsoluteUrlRegExp = new RegExp('^(?:[a-z+]+:)?//', 'i')
            if (!AbsoluteUrlRegExp.test(href)) {
                a.setAttribute("href", href + ".html")
            }
        }        
    }*/
    


    //var extension = (window.location.protocol == "file:") ? ".html" : ""
    

    //window.location.replace(matchLanguage([{{ languages }}]) + extension)

    /**type */

    let baseAs = element.as
    element.as = function (typeName) {
        

        let result = baseAs(typeName)
        if (result != undefined)
            return result

        
        

        if (element.Modifiers != undefined) {
            /*for (let i of element.Modifiers) {
                console.log("as", i.is)
            }*/



            let oldAs = element.as
            element.as = () => undefined
            result = element.Modifiers.find(x => x.is(typeName))
            element.as = oldAs
        }
        
        return result
    }


    //element.Modifiers = undefined,

    element.Reactive = {
        BackgroundColor: undefined,
        HierarchyBackgroundColor: () => element.BackgroundColor || element.Parent.HierarchyBackgroundColor,

        ForegroundColor: () => { 
            if (element.BackgroundColor != undefined)
                return element.BackgroundColor.ContrastColor()
            else
                return undefined
        },
        HierarchyForegroundColor: () => {
            let result = element.ForegroundColor
            if (result) return result
            let parent = element.Parent
            if (parent) {
                result = parent.HierarchyForegroundColor
                if (result) return result
            }
            return new Color(0)
        },

        TextDecorationColor: () => {
            if (element.ForegroundColor != undefined)
                return element.ForegroundColor
            else
                return undefined
        },

        Visibility: 1
    }

    new Reaction(() => {
        let visibility = element.Visibility
        if (visibility == 0) {
            element.style.visibility = "hidden"
            element.style.opacity = ""
        } else if (visibility == 1) {
            element.style.visibility = ""
            element.style.opacity = ""
        } else {
            element.style.opacity = visibility
            element.style.visibility = ""
        }        
    })
    

    new Reaction(() => {
        element.style.backgroundColor = ToCssValue(element.BackgroundColor)
    })

    new Reaction(() => {
        element.style.color = ToCssValue(element.ForegroundColor)
    })

    new Reaction(() => {
        element.style.textDecorationColor = ToCssValue(element.TextDecorationColor)
    })
})
