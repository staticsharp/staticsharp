
var pageHash = "t!dcsctAYNTSYMJaKLcdZPtZ#n@KPIjkK)ppteSZ4t%W)N*3RC8k645V4DUMW5G!";


function Watch() {

    const refreshIntervalMs = 500;
    if (pageHash == undefined) return;

    function CheckRefresh() {
        fetch("/api/get_page_hash")
            .then((response) => {
                if (response.ok) {
                    response.text().then((text) => {
                        let changed = text !== pageHash
                        if (changed) {
                            console.info("Refreshing page...", pageHash, text);
                            document.location.reload();
                        } else {
                            setTimeout(CheckRefresh, refreshIntervalMs);
                        }
                    })
                } else {
                    setTimeout(CheckRefresh, refreshIntervalMs);
                }
                
            })            
            .catch(err => {
                setTimeout(CheckRefresh, refreshIntervalMs);
            })
    }
    setTimeout(CheckRefresh, refreshIntervalMs);
}

console.log("PageHash:", pageHash, performance.now())
Watch()

var developerMode = {
    elementFrame: undefined
}
developerMode.Reactive = {
    CtrlKeyPressed: false,
    ElementMouseIsOver: undefined,
}


document.addEventListener("keydown", () => {
    developerMode.CtrlKeyPressed = event.ctrlKey
});
document.addEventListener("keyup", () => {
    
    developerMode.CtrlKeyPressed = event.ctrlKey
    //console.log("keyup", developerMode.CtrlKeyPressed)
});

document.addEventListener("mousemove", () => {
    var x = event.clientX
    var y = event.clientY
    //console.log("mousemove", document.elementsFromPoint(x, y))
    developerMode.ElementMouseIsOver = document.elementsFromPoint(x, y).find(x => {
        if (!x.is) return false
        return x.is("StaticSharp.Block")
    });
    developerMode.CtrlKeyPressed = event.ctrlKey
});


function GoToSourceCode(callerFilePath, callerLineNumber) {
    let json = JSON.stringify({
                callerFilePath: callerFilePath,
                callerLineNumber: Number(callerLineNumber)
    })
    fetch("/api/go_to_source_code",
        {
            method: "PUT",
            body: json
        }
    )
}



new Reaction(() => {

    if (developerMode.CtrlKeyPressed) {
        if (!developerMode.elementFrame) {
            let margin = document.createElement("developer-mode-element-frame")
            margin.style.backgroundColor = "red"
            margin.style.opacity = 0.25
            margin.style.zIndex = Number.MAX_SAFE_INTEGER

            margin.addEventListener("click", () => {

                let target = developerMode.ElementMouseIsOver
                if (target.dataset.callerFilePath !== undefined) {
                    GoToSourceCode(target.dataset.callerFilePath, target.dataset.callerLineNumber || 0)
                } else {
                    console.warn("GoToSourceCode requires the 'data-caller-file-path' attribute to work")
                }
                event.preventDefault()
                
            });

            let padding = document.createElement("padding")
            padding.style.backgroundColor = "green"
            padding.style.opacity = 1

            let internal = document.createElement("internal")
            internal.style.backgroundColor = "blue"
            internal.style.opacity = 1

            padding.appendChild(internal)
            margin.appendChild(padding)

            developerMode.elementFrame = margin
            document.body.extras.appendChild(developerMode.elementFrame)
        }
        developerMode.elementFrame.style.display = "block"

        let target = developerMode.ElementMouseIsOver
        if (target) {
            let margin = developerMode.elementFrame
            let padding = margin.children[0]
            let internal = padding.children[0]

            //margin.title = target.dataset.callerFilePath + "\nline: " + target.dataset.callerLineNumber

            margin.style.left = Num.Sum(target.AbsoluteX, -target.MarginLeft) + "px"
            margin.style.top = Num.Sum(target.AbsoluteY, -target.MarginTop) + "px"
            margin.style.width = Num.Sum(target.Width, target.MarginLeft, target.MarginRight) + "px"
            margin.style.height = Num.Sum(target.Height, target.MarginTop, target.MarginBottom) + "px"

            padding.style.left = Num.First(target.MarginLeft, 0) + "px"
            padding.style.top = Num.First(target.MarginTop, 0) + "px"
            padding.style.width = Num.First(target.Width, 0) + "px"
            padding.style.height = Num.First(target.Height, 0) + "px"

            internal.style.left = Num.First(target.PaddingLeft, 0) + "px"
            internal.style.top = Num.First(target.PaddingTop, 0) + "px"
            internal.style.width = Num.Sum(target.Width, -target.PaddingLeft, -target.PaddingRight) + "px"
            internal.style.height = Num.Sum(target.Height, -target.PaddingTop, -target.PaddingBottom) + "px"
        }
    } else {
        if (developerMode.elementFrame) {
            developerMode.elementFrame.style.display = "none"
        }
    }
})