using System;
using System.Collections.Generic;
using EPOOutline;
using Game.Shared.Core.Store;
using Game.Shared.Interfaces;
using Game.Shared.Interfaces.Core;
using Game.Shared.Interfaces.EntitySystem;
using R3;
using UnityEngine;

namespace Game.Shared.Core {

public class FocusTrigger : MonoBehaviour, IFocusTrigger {
    HashSet<int> _instanceIds;
    IDisposable _focusChangeListenerRef;
    Outlinable _outlinable;
    protected HashSet<IFocusHitProxy> HitProxyList;

    bool _disabled;

    public void Enable() {
        _disabled = false;

        foreach (IFocusHitProxy focusHitProxy in HitProxyList) {
            focusHitProxy.t.gameObject.SetActive(true);
        }
    }

    public void Disable() {
        _disabled = true;

        foreach (IFocusHitProxy focusHitProxy in HitProxyList) {
            focusHitProxy.t.gameObject.SetActive(false);
        }
    }

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

        _instanceIds = new HashSet<int>();
        foreach (IFocusHitProxy hitProxy in HitProxyList) {
            int instanceId = hitProxy.Init(onHit);
            _instanceIds.Add(instanceId);
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

    void onHit() {
        if (_outlinable.enabled || _disabled) return;

        var id = gameObject.GetComponent<IEntityId>().ID;
        Store2.SetFocusedID(id);

        _outlinable.enabled = true;
        _focusChangeListenerRef = Store2.State.FocusedInstanceID.DistinctUntilChanged().Subscribe(onFocusChange);
    }

    void onFocusChange(int instanceId) {
        if (_instanceIds.Contains(instanceId)) return;

        _outlinable.enabled = false;
        _focusChangeListenerRef?.Dispose();
    }
}

}
