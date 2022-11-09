﻿using StaticSharp.Gears;
using StaticSharp.Tree;

namespace StaticSharp {
    public interface IPageFinder {
        IPageGenerator? FindPage(string requestPath);

    }
}