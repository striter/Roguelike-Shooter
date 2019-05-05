using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider))]
public class InteractTrigger : InteractBase
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == GameLayers.IL_Player)
            TryInteract();
    }
}
