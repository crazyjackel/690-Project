using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

public static class Func_Extensions
{

    public static TaskAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation operation)
    {
        var tcs = new TaskCompletionSource<AsyncOperation>();
        operation.completed += oper => { tcs.SetResult(oper); };
        return tcs.Task.GetAwaiter();
    }

    public static IDisposable BindTo<T>(this ReactiveProperty<T> source, Action<T> doAct)
    {
        return source.ObserveEveryValueChanged(x => x.Value).Subscribe(doAct);
    }

    public static IDisposable BindTo<T>(this ReactiveProperty<T> source, ReactiveProperty<T> connect)
    {
        return source.ObserveEveryValueChanged(x => x.Value).Subscribe(x => connect.Value = x);
    }

    public static IDisposable BindToClick(this Button button, ReactiveCommand command)
    {
        var d1 = command.CanExecute.SubscribeWithState(button, (x, s) => s.SetEnabled(x));
        var d2 = Observable.FromEvent(h => h, h => button.RegisterCallback<ClickEvent>(evt => h.Invoke()), h => button.clicked -= h).SubscribeWithState(command, (x, c) => c.Execute());
        return StableCompositeDisposable.Create(d1, d2);
    }
    public static IDisposable BindToTextLabel(this TextField field, ReactiveProperty<string> property) { 
        EventCallback<ChangeEvent<string>> eventCallback = new EventCallback<ChangeEvent<string>>(evt => property.Value = evt.newValue);
        field.RegisterCallback(eventCallback);
        return Disposable.Create(() => field.UnregisterCallback(eventCallback));
    }
    public static IDisposable BindToTextLabel<T>(this TextField field, ReactiveProperty<T> property, Func<string,T,T> selector)
    {
        EventCallback<ChangeEvent<string>> eventCallback = new EventCallback<ChangeEvent<string>>(evt => property.Value = selector(evt.newValue, property.Value));
        field.RegisterCallback(eventCallback);
        return Disposable.Create(() => field.UnregisterCallback(eventCallback));
    }

    public static IDisposable BindToEvent(this Action act, ReactiveCommand command)
    {
        return Observable.FromEvent(h => act += h, h => act -= h).SubscribeWithState(command, (x, c) => c.Execute(x));
    }
}
