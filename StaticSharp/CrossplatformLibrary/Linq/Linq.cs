using Javascriptifier;
using System.Linq.Expressions;

namespace StaticSharp.Js;

public interface Enumerable {
    [JavascriptOnlyMember]
    public static IEnumerable<R> FromArguments<R>(params R[] args) => throw new JavascriptOnlyException();
}

public interface Enumerable<T> : Enumerable {
    T First(Expression<Func<T, bool>> func);
}

