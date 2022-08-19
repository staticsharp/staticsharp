
function Material(element) {

    Hierarchical(element)


    function getWindowWidth() {
        return document.documentElement.clientWidth//window.innerWidth
    }
    function getWindowHeight() {
        return document.documentElement.clientHeight//window.innerHeight
    }

    //detect slim scroll or thick
    document.documentElement.style.overflowX = "hidden"
    document.documentElement.style.overflowY = "scroll"
    if (window.innerWidth == document.documentElement.clientWidth) {
        document.documentElement.style.overflowY = "auto"
        //console.log("document.documentElement.style.overflowY = auto")
    } else {
        //console.log("document.documentElement.style.overflowY = scroll")
    }



    element.Reactive = {
        WindowWidth: getWindowWidth(),
        WindowHeight: getWindowHeight(),
        ContentWidth: 960,
        BarsCollapsed: () =>
            element.WindowWidth < Sum(
                element.ContentWidth,
                element.LeftSideBar ? element.LeftSideBar.Width : 0,
                element.RightSideBar ? element.RightSideBar.Width : 0),

        Content: () => element.Child("Content"),
        LeftSideBar: () => element.Child("LeftSideBar"),
        RightSideBar: () => element.Child("RightSideBar"),
        Footer: undefined,
    }



    window.onresize = function (event) {        
        let d = Reaction.beginDeferred()
        element.WindowWidth = getWindowWidth()
        element.WindowHeight = getWindowHeight()
        d.end()

    }

    let loadingDeffered = Reaction.beginDeferred()
    let loadEventsToWait = 2
    function onLoadEvent() {
        loadEventsToWait--
        if (loadEventsToWait == 0) {
            
            loadingDeffered.end()
            element.style.visibility = "visible";
            /*let d = Reaction.beginDeferred()
            element.WindowWidth = getWindowWidth()
            element.WindowHeight = getWindowHeight()
            d.end()*/
        }
    }

    document.fonts.ready
        .then(() => {
            onLoadEvent()            
        })

    document.addEventListener("DOMContentLoaded", function (event) {
        onLoadEvent()
    })



    new Reaction(() => {

        let LeftBarSize = 0
        let RightBarSize = Max(element.RightSideBar?.Width, 0)



        if (element.LeftSideBar) {
            if (element.BarsCollapsed) {
                element.LeftSideBar.style.visibility = "hidden"
            } else {
                element.LeftSideBar.style.visibility = "visible"
                LeftBarSize = Max(element.LeftSideBar.Width, 0)
            }            

            element.LeftSideBar.style.position = "fixed"
            element.LeftSideBar.Height = element.WindowHeight
        }



        let width = element.WindowWidth - LeftBarSize - RightBarSize
        let innerWidth = Math.min(width, element.ContentWidth)
        let contentSpace = (width - innerWidth) * 0.5

        if (element.Content) {


            element.Content.Width = width - 2 * contentSpace

            element.Content.MarginLeft = contentSpace

            element.Content.MarginRight = contentSpace

            element.Content.LayoutX = LeftBarSize + contentSpace
            element.Content.LayoutHeight = Math.max(element.Content.InternalHeight, element.WindowHeight)
        }

    })

}


function MaterialBefore(element) {
    HierarchicalBefore(element)

    /*var template = document.createElement('div');
    template.style.display = "contents"
    element.appendChild(template);
    new Reaction(() => {
        template.innerHTML = element.Html;

    })*/

    //element.insertAdjacentHTML('beforeend', `<svg width="800" viewBox="0 0 800 600" x="2 ${200}"><circle cx="500" cy="500" cx="500"r="20"></circle></svg>`);
    //console.log(`265 && element.ContentWidth`)



    /*element.Reactive =
    {
        A: () => {
            console.log("Get A")
            return Max(element.B, element.E)
        },

        //C: () => { Max(element.E, element.A) },

        E: () => {
            console.log("Get E")
            return element.A
        },

        B: 6
    }

    //let d = Reaction.beginDeferred()

    new Reaction(() => {
        console.group("Reaction A:")
        console.log("element.Reactive.E.binding.dirty:", element.Reactive.E.binding.dirty)
        console.log("element.Reactive.A.binding.dirty:", element.Reactive.A.binding.dirty)

        console.log("Reaction A:", element.A)

        console.groupEnd()
    })

    //d.end()

*/
    


    /*new Reaction(() => {
        console.log("Reaction E:", element.E)
    })

    console.log("element.Reactive.E.binding.dirty:", element.Reactive.E.binding.dirty)
    console.log("element.Reactive.A.binding.dirty:", element.Reactive.A.binding.dirty)*/

    




    //PropertyTest()

    

    

    /*let previous = this.Content
    let onChanged = function (previous, current) {
        if (current) {
            this.Content.InnerWidth =
                () => Math.min(this.Content.Width, parameters.ContentWidth)
        }
    }
    new Reaction(() => {
        let current = this.Content
        if (current != previous) {
            onChanged(previous, current)
            previous = current
        }
    })*/


    /*new Reaction(() => {

        if (this.Content)
            console.log("Content.PaddingLeft changed", this.Content.PaddingLeft)
    })*/



 

    //console.log(parameters.ContentWidth)


    /*new Property().attach(element, "Content")

    new Property(window.innerWidth).attach(window, "InnerWidth")
    new Property(window.innerHeight).attach(window, "InnerHeight")


    const LeftBarSize = 200
    const RightBarSize = 50

    new Reaction(() => {
        if (element.Content) {

            element.Content.style.left = LeftBarSize+"px"

            element.Content.MaxInnerWidth = parameters.ContentWidth
            element.Content.Width = window.InnerWidth - LeftBarSize - RightBarSize
        }
    })


    

    */


    /*element.css({
        margin: "0",
    })
    let leftBarWidth = 0;
    let rightBarWidth = 0;

    //contentWidth = 800;

    element.onAnchorsChanged = []
    element.anchors = {}
    var leftBar = element.querySelector("#leftBar");
    var rightBar = document.getElementById("#rightBar");

    leftBarWidth = leftBar == null ? 0 : leftBar.width;
    rightBarWidth = rightBar == null ? 0 : rightBar.width;

    function updateHeaderWidth() {
        let left = element.anchors.textLeft;
        let right = element.anchors.textRight;
        header.style.marginLeft = left + "px";
        header.style.width = right - left + "px";
    }



    function updateAnchors() {
        

        var leftBar = element.querySelector("#leftBar");
        var rightBar = element.querySelector("#rightBar");
        leftBarWidth = leftBar == null ? 0 : leftBar.offsetWidth;
        rightBarWidth = rightBar == null ? 0 : rightBar.offsetWidth;
        const textMargin = 12;
        let width = window.innerWidth;
        let wideAnchorsCollapsed = width < (leftBarWidth + rightBarWidth + contentWidth);
        element.anchors.wideLeft = wideAnchorsCollapsed ? 0 : leftBarWidth;
        element.anchors.wideRight = wideAnchorsCollapsed ? width : width - rightBarWidth;
        element.anchors.wideAnchorsCollapsed = wideAnchorsCollapsed;
        let fillAnchorsCollapsed = width < contentWidth;
        let center = 0.5 * (element.anchors.wideLeft + element.anchors.wideRight);
        element.anchors.fillLeft = fillAnchorsCollapsed ? 0 : (center - 0.5 * contentWidth);
        element.anchors.fillRight = fillAnchorsCollapsed ? width : (center + 0.5 * contentWidth);

        element.anchors.textLeft = Math.max(element.anchors.fillLeft, element.anchors.wideLeft + textMargin);
        element.anchors.textRight = Math.min(element.anchors.fillRight, element.anchors.wideRight - textMargin);

        element.onAnchorsChanged.map(x => x());
        if (wideAnchorsCollapsed) {
            document.getElementById("rightBar").style.visibility = "hidden";
            document.getElementById("leftBar").style.visibility = "hidden";
        } else {
            document.getElementById("rightBar").style.visibility = "";
            document.getElementById("leftBar").style.visibility = "";
        }

    }

    DetectSwipe(element, swiper);

    function swiper(direction, swipe, touchEnd, event) {
        let getTranslate = (offsetWidth) => {
            var value = direction == 'left' || direction == 'right' ? swipe.swipeX : swipe.swipeY;
            let percent = toPercents(Math.abs(value), offsetWidth);
            let translate = percent > 0 ? percent : 0;
            return translate;
        }

        let showChildren = (element) => Array.from(element.children).forEach(x => {
            x.css({ visibility: "visible" });
        });

        if (direction == 'left') {
            let allSwiped = event.path.map(x => x.scrollLeft == null ? true : x.scrollLeft === x.scrollWidth - x.clientWidth).every(x => x == true);
            if (allSwiped) {
                let startY = swipe.clientStartY;
                let touchedElementSpace = menusHitBoxes.find(x => x.top <= startY && x.bottom >= startY);
                if (touchedElementSpace == null) { return; }
                let selectedMenu = touchedElementSpace.element
                if (selectedMenu.position == 'minimizedRight') {
                    showChildren(selectedMenu);
                    let translate = 100 - getTranslate(selectedMenu.offsetWidth);
                    selectedMenu.css({ transform: `translateX(clamp(0%, ${translate}%, 70%))` });
                }
            }
        } else {
            //menusHitBoxes.forEach(x => x.element.css({ transform: 'translateX(70%)' }));
        }
    }

    menusHitBoxes = [];


    window.addEventListener("resize", updateAnchors);
    document.addEventListener("DOMContentLoaded", function() {

        element.style.display = "";
        rightBarWidth = document.getElementById("rightBar").scrollWidth;
        leftBarWidth = document.getElementById("leftBar").scrollWidth;
        header = document.getElementById("Header");
        if (header != null) {
            element.onAnchorsChanged.push(updateHeaderWidth);
        }
        updateAnchors()
        var rightBar = document.getElementById('rightBar');
        var children = Array.from(rightBar.children)
        children.forEach(x => {
            var rect = x.getBoundingClientRect();
            let element = {
                element: x,
                top: rect.top,
                bottom: rect.bottom
            }
            menusHitBoxes.push(element);
        });
    });*/

}