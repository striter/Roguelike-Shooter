using System;
using GameSetting;

public class InteractContainerBattle : InteractContainer {
    public override enum_Interaction m_InteractType => enum_Interaction.ContainerBattle;
    protected override bool B_SelfRecycleOnInteract => true;
    Action OnInteract;
    public void Play(Action _OnInteract, InteractBase _interactItem)
    {
        base.Play();
        Attach(_interactItem);
        OnInteract = _OnInteract;
    }
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        OnInteract();
    }
}