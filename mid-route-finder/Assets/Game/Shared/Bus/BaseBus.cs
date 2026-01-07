using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Game.Shared.Bus {

public interface IBus<TEvent> where TEvent : Enum {
    void Emit(TEvent evt);
    void On(TEvent evt, Action handler);
    void On<T>(TEvent evt, Action<T> handler);
    void On<T1, T2>(TEvent evt, Action<T1, T2> handler);
    void Unregister(TEvent evt, Action handler);
    void Unregister<T>(TEvent evt, Action<T> handler);
    void Unregister<T1, T2>(TEvent evt, Action<T1, T2> handler);
}

public class BaseBus<TEvent2> : IBus<TEvent2> where TEvent2 : Enum {
    readonly Dictionary<TEvent2, List<Delegate>> _eventListeners = new();

    public void Emit(TEvent2 evt) {
        emit(evt);
    }

    public void Emit<T>(TEvent2 evt, [AllowNull] T arg1) {
        emit(evt, arg1);
    }

    public void Emit<T1, T2>(TEvent2 evt, T1 arg1, T2 arg2) {
        emit(evt, arg1, arg2);
    }

    public void On(TEvent2 evt, Action handler) {
        register(evt, handler);
    }

    public void On<T>(TEvent2 evt, Action<T> handler) {
        register(evt, handler);
    }

    public void On<T1, T2>(TEvent2 evt, Action<T1, T2> handler) {
        register(evt, handler);
    }

    public void Unregister(TEvent2 evt, Action handler) {
        unregister(evt, handler);
    }

    public void Unregister<T>(TEvent2 evt, Action<T> handler) {
        unregister(evt, handler);
    }

    public void Unregister<T1, T2>(TEvent2 evt, Action<T1, T2> handler) {
        unregister(evt, handler);
    }

    protected void emit(TEvent2 evt) {
        if (_eventListeners.TryGetValue(evt, out var handlers)) {
            for (int i = handlers.Count - 1; i >= 0; i--) {
                if (handlers[i] is Action action) {
                    action.Invoke();
                }

                // else {
                //     Debug.LogWarning($"Handler for event {evt} does not match expected signature.");
                // }
            }
        }
    }

    protected void emit<T>(TEvent2 evt, T arg1) {
        if (_eventListeners.TryGetValue(evt, out var handlers)) {
            for (int i = handlers.Count - 1; i >= 0; i--) {
                if (handlers[i] is Action<T> action) {
                    action.Invoke(arg1);
                }

                // else {
                //     Debug.LogWarning($"Handler for event {evt} does not match expected signature.");
                // }
            }
        }
    }

    protected void emit<T1, T2>(TEvent2 evt, T1 arg1, T2 arg2) {
        if (_eventListeners.TryGetValue(evt, out var handlers)) {
            for (int i = handlers.Count - 1; i >= 0; i--) {
                if (handlers[i] is Action<T1, T2> action) {
                    action.Invoke(arg1, arg2);
                }

                // else {
                //     Debug.LogWarning($"Handler for event {evt} does not match expected signature.");
                // }
            }
        }
    }

    protected void register(TEvent2 evt, Delegate handler) {
        if (!_eventListeners.ContainsKey(evt)) {
            _eventListeners[evt] = new List<Delegate>();
        }

        _eventListeners[evt].Add(handler);
    }

    protected void unregister(TEvent2 evt, Delegate handler) {
        if (_eventListeners.TryGetValue(evt, out var handlers)) {
            handlers.Remove(handler);
            if (handlers.Count == 0) {
                _eventListeners.Remove(evt);
            }
        }
    }
}

}
