﻿




function ReactionBase(func) {
    let _this = this

    _this.triggeringProperties = new Set()
    _this.func = func
    
    _this.addTriggeringProperty = function (property) {
        _this.triggeringProperties.add(property)
        property.dependentReactions.add(_this)
    }

    _this.unsubscribeFromTriggeringProperty = function (property) {
        if (property.dependentReactions.delete(_this)) {
            _this.triggeringProperties.delete(property)
            return true
        }
        return false        
    }

    _this.unsubscribeFromTriggeringProperties = function () {
        if (_this.triggeringProperties.size == 0) return

        for (let triggeringProperty of _this.triggeringProperties) {
            triggeringProperty.dependentReactions.delete(_this)
        }
        _this.triggeringProperties.clear()
    }

    _this.execute = function (object) {
        let oldReaction = Reaction.current
        Reaction.current = _this
        _this.unsubscribeFromTriggeringProperties()

        try {
            let result = _this.func(object)
            return result
        }
        finally {
            Reaction.current = oldReaction
        }
    }


    //_this.makeDirty = abstract function
    _this.enabled = true
}


function Reaction(func) {
    let _this = this

    ReactionBase.call(_this, func)
    
    _this.makeDirty = function () {
        if (!_this.enabled) 
            return;
        Reaction.deferred.add(_this)
    }

    if (Reaction.deferred) {
        Reaction.deferred.add(_this)
    } else {
        let d = Reaction.beginDeferred()

        try {
            _this.execute()
        } catch (e) {
            console.warn(e)
        }
        
        d.end()
    }
}

Reaction.prototype = Object.create(ReactionBase.prototype)
Reaction.prototype.constructor = Reaction

Reaction.beginDeferred = function () {
    if (!Reaction.deferred) {
        Reaction.deferred = new Set()
        return {
            end: function () {

                let maxIterations = 64;
                //TODO: нужно проверять рекурсии иначе: нужно пересчитывать сколько раз выполняется одна и та же реакция.
                //текущая реализация увидит рекурсию там где ее нет. Например если действительно собрана очень длинная цепочка реакций a->b->c->...
                while (Reaction.deferred.size > 0 & maxIterations > 0) {
                    //console.log("Reaction.deferred.end", Reaction.deferred.size)
                    let d = Array.from(Reaction.deferred)
                    for (let reaction of d) {
                        Reaction.deferred.delete(reaction)
                        try {
                            reaction.execute()
                        } catch (e) {
                            console.warn(e)
                        }
                        
                    }
                    maxIterations--
                    //d.forEach(x => x.execute())
                    //Reaction.deferred.clear()
                }

                if (maxIterations == 0) {
                    console.error("Recursive property binding", Reaction.deferred)
                }

                //let l = Reaction.deferred;
                Reaction.deferred = undefined
                //l.forEach(x => x.execute())

                

            }
        };
    }
    return {
        end: function () { }
    };
}


Reaction.beginNonReactive = function () {
    if (!Reaction.current) {
        let oldCurrent = Reaction.current
        Reaction.current = undefined
        return {
            end: function () {
                Reaction.current = oldCurrent
            }
        };
    }
    return {
        end: function () { }
    };
}



function Binding(func, onBecameDirty) {
    //console.log("function Binding(func, onChange)", onChange)
    let _this = this

    ReactionBase.call(_this, func)

    _this.dirty = true
    //_this.onBecameDirty = onBecameDirty

    
    _this.makeDirty = function () {
        if (_this.dirty)
            return

        _this.dirty = true        
        onBecameDirty()
    }
}
Binding.prototype = Object.create(ReactionBase.prototype);
Binding.prototype.constructor = Binding;



function Property() {

    let _this = this
    _this.name = ""
    _this.object = null

    _this.dependentReactions = new Set()
    //window.debug = (window.debug || 0) + 1
    _this.binding = undefined

    // TODO: never called?
    _this.makeDirty = function () {
        _this.binding.makeDirty()
    }
    
    _this.onBindingBecameDirty = function () {
        _this.makeDependentReactionsDirty()
    }

    _this.makeDependentReactionsDirty = function () {
        /********************************************
         a deferred context is reqired here in case
         makeDirty() is called from a callback
         
         window.setTimeout(() => {
            element.Reactive.SomeProperty.makeDirty();
        },50)

         *******************************************/
        

        if (_this.dependentReactions.size == 0) {            
            return
        }            

        var d = Reaction.beginDeferred() 
        _this.dependentReactions.forEach(x => x.makeDirty())
        d.end()
    }

    /*_this.dependsOn = function (property) {
        if (!_this.binding)
            return false
        _this.getValue()
        for (let i of _this.binding.dependencies) {
            if (i == property)
                return true
        }
        for (let i of _this.binding.dependencies) {
            if (i.dependsOn(property))
                return true
        }
        return false
    }

    _this.getRecursiveDependencies = function () {
        let result = new Set();
        if (!_this.binding)
            return result
        _this.getValue()
        
        _this._collectRecursiveDependencies(result)
        return result
    }

    _this._collectRecursiveDependencies = function (set) {
        for (let i of _this.binding.dependencies) {
            set.add(i)
            i._collectRecursiveDependencies(set)
        }
    }*/
    _this.executionInProgress = false

    _this.replaceValue = function (func) {
        let previousReactionCurrent = Reaction.current
        Reaction.current = undefined
        let newValue = func(this.getValue())
        this.setValue(newValue)
        Reaction.current = previousReactionCurrent
    }

    _this.getValue = function() {

        if (Reaction.current) {
            Reaction.current.addTriggeringProperty(_this)
        }

        if (_this.binding) { //wechat (this.binding?.dirty) not supported
            if (_this.binding.dirty) {
                if (_this.executionInProgress) {


                    if (_this.reactionsWhoReceivedOldValue == undefined) {
                        _this.reactionsWhoReceivedOldValue = new Set()
                    }
                    _this.reactionsWhoReceivedOldValue.add(Reaction.current)                    

                    return _this.value
                }
                try {
                    var oldValue = _this.value
                    try {
                        _this.executionInProgress = true
                        _this.value = _this.binding.execute(_this.object)
                        
                    } finally {
                        _this.executionInProgress = false

                        _this.binding.dirty = false

                        if (_this.reactionsWhoReceivedOldValue) {
                            if (_this.value !== oldValue) {
                                let d = Reaction.beginDeferred()
                                _this.reactionsWhoReceivedOldValue.forEach(x => x.makeDirty())
                                d.end()
                            }
                            _this.reactionsWhoReceivedOldValue = undefined
                        }                            
                    }

                } catch (e) {
                    console.error(e)
                }                
            }
        }
        return _this.value
    }


    /*_this.setValueKeepBinding = function (value) {

        if (_this.value === value)
            return
        _this.value = value
        var d = Reaction.beginDeferred()
        _this.makeDependentReactionsDirty()
        d.end()
        if (_this.binding) {
            _this.binding.dirty = false
        }
    }*/
    _this.onAssign = undefined

    _this.setValue = function(value) {
        //console.log("setValue " + value + " " + _this.onChanged.size)
        if (typeof value === 'function') {

            if (value.isBindingConstructor) {
                value = value(_this)
                console.log("isBindingConstructor", value, this.object)

            }

            if (_this.binding) {
                if (_this.binding.func === value) {
                    return
                }
                _this.binding.unsubscribeFromTriggeringProperties()
                //console.log("change binding from", _this.binding.func, "to", value)
            }
            
            _this.binding = new Binding(value, _this.onBindingBecameDirty)


        } else {
            //console.log("value assigned", _this.value, "->", value, "will notify ", _this.onChanged.size)
            if (_this.binding) {
                _this.binding.unsubscribeFromTriggeringProperties()
                _this.binding = undefined
            }

            if (_this.onAssign) {
                var d = Reaction.beginDeferred()
                _this.onAssign(_this.value, value)
                d.end()
            }

            if (_this.value === value)
                return

            _this.value = value
        }

        var d = Reaction.beginDeferred()
        _this.makeDependentReactionsDirty()
        d.end()
    }


     /**
     * @param { string } name
     */
    _this.attach = function(object, name) {
        //let property = this
        _this.name = name
        _this.object = object
        let accessorDescriptor = {
            get: function () {
                return _this.getValue()
            },
            set: function (value) {
                _this.setValue(value)

            }
        }
        Object.defineProperty(object, name, accessorDescriptor);
        object["__" + name] = _this
        return _this
    }    
    //_this.setValue(value)
}



Property.exists = function (target, name) {
    var propertyDescriptor = !!Object.getOwnPropertyDescriptor(target, name)
    var propertyFieldExists = target.hasOwnProperty("__" + name)
    return propertyDescriptor && propertyFieldExists
}

Property.nameAvailable = function (target, name) {
    if (Object.getOwnPropertyDescriptor(target, name))
        return false
    if (target.hasOwnProperty(name))
        return false
    return true
}

Object.defineProperty(Object.prototype, "Reactive", {
    get: function () {
        return new Proxy(
            this,
            {
                get(target, name, receiver) {
                    //console.log(`get target:${JSON.stringify(target)} name: ${name}`)
                    return target["__" + name]
                    //return Reflect.get(...arguments);
                },
                set: function (target, name, value, receiver) {
                    //console.log("set",target,name,value)
                    
                    if (target.hasOwnProperty("__" + name)) {
                        //console.log("asigning existing property", name)
                        const propertyField = target["__" + name]
                        propertyField.setValue(value)
                    } else {
                        let property = new Property()
                        property.setValue(value)
                        property.attach(target, name)
                        //console.log("creating new property", name, target)
                        
                    }
                }
            }
            )
    },
    set: function (obj) {
        let proxy = this.Reactive
        //console.log(proxy)

        let d = Reaction.beginDeferred()
        //отложенное выполнение реакций не нужно при создании свойств
        //тут оно используется т.к. следующий код может не только создать свойство
        //но и присвоить значение существующему свойству
        if (obj instanceof Object) {
            for (const [key, value] of Object.entries(obj)) {
                proxy[key] = value                
            }
        }
        d.end()
    }
});

/**
 * @template T
 * @param {function():T} getter
 * @param {function(T,T) : void} action
*/
function OnChanged(getter, action) {
    let previous = undefined
    return new Reaction(() => {
        let current = getter()
        if (current != previous) {
            let n = Reaction.beginNonReactive()
            action(previous, current)
            n.end()
            previous = current
        }
    })
}




function OnTruthify(predicate, action) {
    
    let previous = undefined
    return new Reaction(() => {        
        let current = predicate()
        if (current === true && (!previous === true)) {
            let n = Reaction.beginNonReactive()
            action()            
            n.end()
        }
        previous = current
    })
}
/*
function PropertyTest() {

    console.group("PropertyTest")

    let root = {
        Field: 7

    }

    console.assert( Property.nameAvailable(root, "A") == true)
    console.assert( Property.nameAvailable(root, "Field") == false)


    console.assert( Property.exists(root, "A") == false)

    root.Reactive.A = 0

    console.assert( Property.exists(root, "A") == true)
    console.assert( Property.nameAvailable(root, "A") == false)

    console.assert(root.A == 0)
    root.Reactive.A = 8
    console.assert(root.A == 8)


    root.Reactive.B = () => root.A * 2
    console.assert( root.B == 16)

    root.A = 10
    console.assert( root.B == 20)

    

    root.Reactive = {
        C: 7,
        D: () => root.C,
        //E: () => root.B*4,
    }

    console.assert( Property.exists(root, "C") == true)
    console.assert( Property.exists(root, "D") == true)

    console.assert(root.D == root.C)
    //console.assert(root.E == root.B*4)

    root.Reactive.B = () => root.A * 3

    new Reaction(() => {
        root.Field = root.C
    })
    console.assert(root.Field == root.C)

    let bReactionResult
    new Reaction(() => {
        bReactionResult = root.B
    })
    console.assert(bReactionResult == root.B)
    console.group("root.A = 20")
    root.A = 20
    console.groupEnd()
    console.assert(bReactionResult == root.B)


    root.C = 9
    console.assert(root.Field == root.C)
    console.assert(root.D == root.C)


    root.Reactive.D = ()=>root.C+1



    console.groupEnd()
}*/
