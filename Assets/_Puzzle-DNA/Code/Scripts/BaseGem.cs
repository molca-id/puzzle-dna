using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[RequireComponent(typeof(SpriteRenderer))]
public class BaseGem : MonoBehaviour, ITouchable
{
    Coroutine moveToCoroutine = null;
    Coroutine animationCoroutine = null;

    public GameObject chainObject;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public Vector2Int position;

    [SerializeField] GemType _type;
    public virtual GemType type
    {
        get { return _type; }
        set { _type = value; }
    }

    [SerializeField] int _minMatch = 3;
    public virtual int minMatch
    {
        get { return _minMatch; }
        set { _minMatch = value; }
    }

    public virtual Func<BaseGem, bool> validateGem
    {
        get { return gem => gem.type == type; }
    }

    public virtual MatchInfo GetMatch()
    {
        return BoardController.GetCrossMatch(this, false, validateGem);
    }

    public bool isEmpty;

    public void SetType(GemData gemData)
    {
        type = gemData.type;
        spriteRenderer.sprite = gemData.sprite;
        minMatch = gemData.minMatch;
    }

    public void SetType(GemData gemData, Sprite emptySprite, bool isEmpty)
    {
        this.isEmpty = isEmpty;
        if (chainObject) chainObject.SetActive(isEmpty);

        if (isEmpty)
        {
            type = GemType.Empty;
            name = $"{position} EmptyGem";
            spriteRenderer.enabled = false;
            minMatch = 999;

            if (emptySprite == null) chainObject.GetComponent<SpriteRenderer>().sprite = null;
            else chainObject.GetComponent<SpriteRenderer>().sprite = emptySprite;
        }
        else
        {
            type = gemData.type;
            name = $"{position} {gemData.type}";
            spriteRenderer.sprite = gemData.sprite;
            minMatch = gemData.minMatch;
        }
    }

    public void SetPosition(Vector2Int position)
    {
        this.position = position;
        BoardController.gemBoard[position.x, position.y] = this;
    }

    public float MoveTo(Vector3 target, float speed, float delay = 0)
    {
        if (moveToCoroutine != null)
            StopCoroutine(moveToCoroutine);

        moveToCoroutine = StartCoroutine(IEMoveTo(target, speed, delay));

        return ((target - transform.position).magnitude / speed) + delay;
    }

    IEnumerator IEMoveTo(Vector3 target, float speed, float delay)
    {
        yield return new WaitForSeconds(delay);
        float distance = (target - transform.position).magnitude;

        while (!Mathf.Approximately(0.0f, distance))
        {
            transform.position = Vector3.MoveTowards(
                transform.position, target, speed * Time.deltaTime
            );
            yield return null;
            distance = (target - transform.position).magnitude;
        }

        transform.position = target;
    }

    public float Creating(float delay = 0)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animator.SetTrigger("creating");
        animationCoroutine = StartCoroutine(IEAnimationDelay(delay));

        return animator.GetCurrentStateDuration() + delay;
    }

    public float Matched(float delay = 0)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animator.SetTrigger("matched");
        animationCoroutine = StartCoroutine(IEAnimationDelay(delay));

        return animator.GetCurrentStateDuration() + delay;
    }

    public void Hint(bool start = true)
    {
        animator.SetBool("hinting", start);
    }

    IEnumerator IEAnimationDelay(float delay)
    {
        animator.enabled = false;

        yield return new WaitForSeconds(delay);

        animator.enabled = true;
    }

    public void TouchDown()
    {

    }

    public void TouchDrag()
    {
        if (Vector2.Distance(transform.position, TouchController.touchPosition) > 0.75f)
        {
            BaseGem otherGem;
            Vector2 delta = TouchController.touchPosition - transform.position;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                int swapX = (int)(position.x + Mathf.Sign(delta.x));
                otherGem = BoardController.GetGem(swapX, position.y);
            }
            else
            {
                int swapY = (int)(position.y + Mathf.Sign(delta.y));
                otherGem = BoardController.GetGem(position.x, swapY);
            }

            if (otherGem && !otherGem.isEmpty)
            {
                BoardController.TryMatch(this, otherGem);
                SoundController.PlaySfx(GameData.GetAudioClip("swap"));
            }

            TouchUp();
        }

    }

    public void TouchUp()
    {
        TouchController.ClearElementClicked();
    }

    public void DestroyGem()
    {
        Destroy(gameObject, Matched());
    }
}
