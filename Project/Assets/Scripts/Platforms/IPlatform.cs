using PlayerSystem;
using UnityEngine;

public interface IPlatform
{
    PlatformType PlatformType { get; set; }

    public void PlatformEnterAction(PlayerSystem.PlayerState playerState, Rigidbody2D playerRigidBody);
    public void PlatformExitAction(Rigidbody2D playerRigidBody);
}