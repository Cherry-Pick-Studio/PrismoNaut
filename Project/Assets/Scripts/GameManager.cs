using UnityEngine;

public enum FormState
{
    Undefined = -1,
    Circle = 0,
    Square = 1,
    Triangle = 2
}

public enum LifeIconState
{
    Undefined,
    Normal,
    Damaged,
    Off,
}

public class GameManager : MonoBehaviour
{
    public static int maxLifes = 3;
    public static FormState startingForm = FormState.Square;

}

public enum ObstacleType
{
    Asteroid,
    Hollow,
    Block,
}

public enum PlatformType
{
    MovingPlatform,
    FallingPlatform,
    CrumblingPlatform,
}