namespace Game.Shared.Interfaces {

public interface IHoverable {
    bool Disabled { get; set; }
    void SetHover(bool hover = true);
}

}
