


StaticSharpClass("StaticSharp.AspectBlock", (element) => {
    StaticSharp.Block(element)

    element.Reactive = {
        Width: undefined,
        Height: undefined,
    }
    
    element.Reactive = {
        NativeAspect: e => e.NativeWidth / e.NativeHeight,
        NativeWidth: undefined,
        NativeHeight: undefined,
        

        //Aspect: e => e.NativeAspect,
        Width: () => Num.First(element.Height * element.NativeAspect, element.NativeWidth),
        Height: () => Num.First(element.Width / element.NativeAspect, element.NativeHeight),

        Fit: "Inside",
        GravityVertical: 0,
        GravityHorizontal: 0,
    }

    


})