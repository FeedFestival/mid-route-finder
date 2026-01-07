using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Shared.Models {

public class BaseUiController : MonoBehaviour {
    protected UIDocument _doc {
        get {
            if (__doc is null)
                __doc = GetComponent<UIDocument>();
            return __doc;
        }
    }

    UIDocument __doc;
}

public static class UIHelperExtensions {
    public static void SetInvisibile(this VisualElement el) {
        el.SetVisibile(false);
    }

    public static void SetVisibile(this VisualElement el, bool show = true) {
        el.style.visibility = show
            ? Visibility.Visible
            : Visibility.Hidden;
    }

    public static void Hide(this VisualElement el) {
        el.Show(false);
    }

    public static void Show(this VisualElement el, bool show = true) {
        el.style.display = show
            ? DisplayStyle.Flex
            : DisplayStyle.None;
    }
}

}
