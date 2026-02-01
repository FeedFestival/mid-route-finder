namespace Game.Shared.Interfaces.Core {

public interface IFocusable : IIdable, ICommandable {
    void SetFocused(bool focus = true);
    void DisableFocus();
    void EnableFocus(bool enable = true);
}

}
