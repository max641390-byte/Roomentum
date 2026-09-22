using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public bool CanJump;
    public bool CanWallJump;
    public bool CanDoubleJump;
    public bool CanDash;
    public bool CanGlide;
    public bool CanAttack;
    public bool CanSprint;
    public bool CanSlide;

    public void UnlockJump()
    {
        CanJump = true;
    }

    public void UnlockWallJump()
    {
        CanWallJump = true;
    }

    public void UnlockDoubleJump()
    {
        CanDoubleJump = true;
    }

    public void UnlockDash()
    {
        CanDash = true;
    }

    public void UnlockGlide()
    {
        CanGlide = true;
    }

    public void UnlockAttack()
    {
        CanAttack = true;
    }

    public void UnlockSprint()
    {
        CanSprint = true;
    }

    public void UnlockSlide()
    {
        CanSlide = true;
    }
}