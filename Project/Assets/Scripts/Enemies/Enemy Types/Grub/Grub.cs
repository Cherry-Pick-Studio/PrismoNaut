using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grub : Enemy, IEnemyMoveable
{
    public Transform PointA;
    public Transform PointB;
    public override void PlayerPowerInteraction(PlayerSystem.PlayerState playerState)
    {
        if (playerState.activePower == PlayerSystem.Power.Square)
        {
            Damage(1);
        }
        if (playerState.activePower == PlayerSystem.Power.Circle)
        {
            Debug.Log("Player Power Interaction stun");
            StateMachine.ChangeState(StunState);
        }
    }
}
