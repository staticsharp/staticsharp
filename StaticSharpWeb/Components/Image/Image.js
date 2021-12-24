﻿function Image(element) {
    let parent = element.parentElement;
    element.onAnchorsChanged = [];
    //console.log(element.firstChild);
    element.updateWidth = function() {
        let left = parent.anchors.wideLeft;
        let right = parent.anchors.wideRight;
        element.style.marginLeft = left + "px";
        element.style.width = right - left + "px";
        //element.style.minWidth = "1280px";
        //element.style.minHeight = "500px";
        element.style.height = "500px";
        element.style.overflow = "hidden";
        element.style.backgroundColor = "rgb(60, 61, 63)";
        //let innerHeight = Math.max(element.style.height, element.style.minHeight);
        //console.log(element.style.height);
        //TranslateInnerImage(element);
        //element.image.style.transform = "translate: (0px, -100px)";
        //element.style.backgroundColor = "red";
    }

    try {
        parent.onAnchorsChanged.push(element.updateWidth);
    } catch {

    }
}

// function TranslateInnerImage(element) {
//     let innerHeight = element.style.height;
//     console.log(innerHeight);
//     element.firstChild.style.transform = "translate(0px, 0px)";
// }