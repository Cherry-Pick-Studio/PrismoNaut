using System.Collections.Generic;
using UnityEngine;

namespace PlayerSystem
{
    // Use this class to gatekeep powers
    // If the player is not supposed to have a power, then don't instantiate it
    public class PlayerPowersModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private SquarePowerModule squarePower;
        private TrianglePowerModule trianglePower;
        private CirclePowerModule circlePower;

        private bool isSquarePowerAvailable = true;
        private bool isTrianglePowerAvailable = true;
        private bool isCirclePowerAvailable = true;

        public PlayerPowersModule(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, Dictionary<Direction, TriggerEventHandler> triggers)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            if (isSquarePowerAvailable) squarePower = new SquarePowerModule(eventBus, playerState, rb2d, triggers[Direction.Down]);
            if (isTrianglePowerAvailable) trianglePower = new TrianglePowerModule(eventBus, playerState, rb2d, triggers[Direction.Up]);
            if (isCirclePowerAvailable) circlePower = new CirclePowerModule(eventBus, playerState, rb2d, triggers[Direction.Left], triggers[Direction.Right]);
        }
    }
}