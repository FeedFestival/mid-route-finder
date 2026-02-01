using System.Linq;
using Game.Shared.Core;
using UnityEngine;

public class RouteFocusTrigger : FocusTrigger {
    internal void AlignAndScaleToFit(Vector3 from, Vector3 dir, float yScale, float dist) {
        var mid = from + dir * 0.5f;

        if (HitProxyList == null || HitProxyList.Count == 0) {
            Debug.LogWarning("Tried to AlignAndScaleToFit() but no HitProxy was found");
            return;
        }

        var hitProxy = HitProxyList.FirstOrDefault()!;

        hitProxy.t.position = mid + new Vector3(0, yScale * 0.5f, 0);
        hitProxy.t.rotation = Quaternion.LookRotation(dir, Vector3.up);

        var ps = hitProxy.t.localScale;
        ps.z = dist;
        hitProxy.t.localScale = ps;
    }
}
