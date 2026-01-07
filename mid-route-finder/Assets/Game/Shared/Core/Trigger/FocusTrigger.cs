using System;
using System.Collections.Generic;
using EPOOutline;
using Game.Shared.Core.Store;
using Game.Shared.Interfaces;
using Game.Shared.Interfaces.Core;
using R3;
using UnityEngine;

namespace Game.Shared.Core {

public class FocusTrigger : MonoBehaviour, IFocusTrigger {
    int _instanceId;
    IDisposable _focusChangeListenerRef;
    Outlinable _outlinable;
    protected HashSet<IFocusHitProxy> HitProxyList;

    void Awake() {
        _outlinable = GetComponent<Outlinable>();

        if (_outlinable == null) {
            Debug.LogError($"No Outlinable found on {gameObject.name}. FocusTrigger finds no reason to exist.");
            return;
        }

        _outlinable.enabled = false;

        tryGetHitProxyFromGameObject(gameObject);
        foreach (Transform child in transform) {
            tryGetHitProxyFromGameObject(child.gameObject);
        }

        if (HitProxyList == null) {
            Debug.LogWarning(
                $"No HitProxies, no reason to have Focus Trigger. Consider removing this component from {gameObject.name}.");
            return;
        }

        foreach (IFocusHitProxy hitProxy in HitProxyList) {
            hitProxy.Init(onHit);
        }
    }

    void tryGetHitProxyFromGameObject(GameObject go) {
        var hitProxy = go.GetComponent<IFocusHitProxy>();
        if (hitProxy != null) {
            if (HitProxyList == null)
                HitProxyList = new();

            HitProxyList.Add(hitProxy);
        }
    }

    void onHit(int instanceId) {
        _outlinable.enabled = true;
        _instanceId = instanceId;
        _focusChangeListenerRef = Store2.State.FocusedTriggerID.Subscribe(onFocusChange);
    }

    void onFocusChange(int focusedTriggerId) {
        if (focusedTriggerId != _instanceId)
            _outlinable.enabled = false;

        _focusChangeListenerRef?.Dispose();
    }
}

}
