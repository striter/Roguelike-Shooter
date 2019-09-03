using System;
using GameSetting;

public class InteractContainerBattle : InteractContainer {
    public override enum_Interaction m_InteractType => enum_Interaction.ContainerBattle;
    protected override bool B_RecycleOnInteract => true;
    protected override bool B_InteractaOnce => true;
    Action OnInteract;
    public void Play(Action _OnInteract, InteractBase _interactItem)
    {
        base.Play();
        Attach(_interactItem);
        OnInteract = _OnInteract;
    }
    protected override void OnInteractSuccessful(EntityPlayerBase _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        OnInteract();
        Detach();
    }
}
