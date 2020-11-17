using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Derection { Up, Down, Right, Left , UpLeft , UpRight , DownLeft, DownRight, None };

public class AnimationController : MonoBehaviour
{
    [Header("Walk Settings")]
    public SpriteRenderer spriteRenderer;
    public int walkDistance = 1;
    public float walkSpeed;

    private Derection currentDerection;

    [Header("Attack Settings")]
    public List<AnimationClip> animations = new List<AnimationClip>();
    private Animation animator;

    public bool isAttacking, isWalking;
    public Transform hand,arm,holder;

    private void Start()
    {
        animator = hand.GetComponent<Animation>();
    }

    public void WalkVisualy(bool flipX = false)
    {
        spriteRenderer.flipX = flipX;


        float flip = -180f;
        Transform current = holder.transform;
        //base code will be inproved later...
        if (flipX && current.localEulerAngles.y != flip)
        {
            holder.transform.localEulerAngles = new Vector3(current.localEulerAngles.x, flip, current.localEulerAngles.z);
        }
        else
        {
            holder.transform.localEulerAngles = new Vector3(current.localEulerAngles.x, 0, current.localEulerAngles.z);
        }
        ResetHand();
    }

    public void WalkTowards(Derection newDerection, bool UpdateRound = false)
    {
        Vector3 currentPos = transform.position;
        Vector3 NewPos = Vector3.zero;
        //set new position
        switch (newDerection)
        {
            case Derection.Up:
                NewPos = currentPos + new Vector3(0, walkDistance, 0);
                break;
            case Derection.Down:
                NewPos = currentPos + new Vector3(0, -walkDistance, 0);
                break;
            case Derection.Right:
                NewPos = currentPos + new Vector3(walkDistance, 0, 0);
                break;
            case Derection.Left:
                NewPos = currentPos + new Vector3(-walkDistance, 0, 0);
                break;
            case Derection.UpLeft:
                NewPos = currentPos + new Vector3(-walkDistance, walkDistance, 0);
                break;
            case Derection.UpRight:
                NewPos = currentPos + new Vector3(walkDistance, walkDistance, 0);
                break;
            case Derection.DownLeft:
                NewPos = currentPos + new Vector3(-walkDistance, -walkDistance, 0);
                break;
            case Derection.DownRight:
                NewPos = currentPos + new Vector3(walkDistance, -walkDistance, 0);
                break;
        }

        if (!isWalking && !isAttacking)
        {
            if (newDerection == Derection.Left 
                || newDerection == Derection.DownLeft 
                || newDerection == Derection.UpLeft)
            {
                WalkVisualy(true);
            }
            if (newDerection == Derection.Right
                || newDerection == Derection.DownRight
                || newDerection == Derection.UpRight)
            {
                WalkVisualy(false);
            }

            currentDerection = newDerection;

            if (MapGenerator.current.CheckTileIndex(0, NewPos))
            {
                isWalking = true;
                StartCoroutine(MoveTowards(NewPos, UpdateRound));

            }
        }
    }

    public void Attack(Derection attackDerection)
    {
        if (isAttacking)
            return;

        Vector3 armRot =  arm.transform.eulerAngles;
        switch (attackDerection)
        {
            case Derection.Up:
                armRot.z = 180f;
                arm.transform.eulerAngles = armRot;
                break;
            case Derection.Down:
                armRot.z = 0f;
                arm.transform.eulerAngles = armRot;
                break;
            case Derection.Right:
                armRot.z = 90f;
                arm.transform.eulerAngles = armRot;
                break;
            case Derection.Left:
                armRot.z = -90f;
                arm.transform.eulerAngles = armRot;
                break;
            case Derection.UpLeft:
                armRot.z = -135f;
                arm.transform.eulerAngles = armRot;
                break;
            case Derection.UpRight:
                armRot.z = 135f;
                arm.transform.eulerAngles = armRot;
                break;
            case Derection.DownLeft:
                armRot.z = 45f;
                arm.transform.eulerAngles = armRot;
                break;
            case Derection.DownRight:
                armRot.z = 45f;
                arm.transform.eulerAngles = armRot;
                break;
        }

        AnimationClip current = animations[0];

        current.legacy = true;
        animator.clip = current;
        animator.AddClip(current, "current");
        animator.Play("current");

        isAttacking = true;
        StartCoroutine(CheckAttack());
    }

    private IEnumerator CheckAttack()
    {
        float time = animator.clip.length;

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        animator.RemoveClip("current");
        animator.clip = null;
        GameEvents.current.NextRound();
        isAttacking = false;
        ResetHand();

    }

    

    private IEnumerator MoveTowards(Vector3 newPosition , bool UpdateRound)
    {
        while (transform.position != newPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPosition, walkSpeed * Time.deltaTime);
            yield return null;
        }

        isWalking = false;

        if (UpdateRound)
        {
            GameEvents.current.NextRound();
        }
    }

    private void ResetHand()
    {
        //reset hand
        animator.transform.localPosition = holder.localPosition;
        animator.transform.eulerAngles = holder.eulerAngles;
    }
}
