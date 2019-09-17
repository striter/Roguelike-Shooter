using System;

public class CampInteractScene : CampInteract {
    public enum_Scene m_scene { get; private set; }
    Action<enum_Scene> OnInteract;
    public void Play(enum_Scene _scene, Action<enum_Scene> _OnInteract)
    {
        m_scene = _scene;
        OnInteract = _OnInteract;
    }
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        OnInteract(m_scene);
    }
}
