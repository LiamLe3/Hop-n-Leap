using UnityEngine;

public class ArrowController : MonoBehaviour
{
    const int MaxJumpSprite = 8;
    const int MaxHoldDuration = 1;
    const float JumpDurationSegment = 0.125f;

    FlipController flipController;
    SpriteRenderer arrowSpriteRenderer;

    bool stopArrow;
    bool isAscending = true;
    float angle = 0;
    
    [SerializeField] float radius;
    [SerializeField] float interval = 0.02f;    
    [SerializeField] GameObject arrowObject;
    [SerializeField] Sprite[] arrowSprites;

    void Awake()
    {
        arrowSpriteRenderer = arrowObject.GetComponent<SpriteRenderer>();
        flipController = GetComponent<FlipController>();
    }

    void Start()
    {
        InvokeRepeating("MoveArrow", 0f, interval);
    }
    
    void OnStopArrow()
    {
        stopArrow = !stopArrow;
    }

    void MoveArrow()
    {   
        UpdateArrow();

        if(stopArrow) return;

        if(isAscending)
        {
            angle++;
            if(angle > 55)
                isAscending = !isAscending;
        }
        else
        {
            angle--;
            if(angle < 0)
                isAscending = !isAscending;
        }      
    }

    public void UpdateArrow()
    {
        float angleDegrees = GetArrowAngle();
        UpdateArrowPosition(angleDegrees * Mathf.Deg2Rad);
        UpdateArrowAngle(angleDegrees);
    }

    void UpdateArrowPosition(float angleRadians)
    {
        float x = transform.position.x + radius * Mathf.Cos(angleRadians);
        float y = transform.position.y + radius * Mathf.Sin(angleRadians);
        Vector2 newPos = new Vector2(x, y);

        arrowObject.transform.position = newPos;
    }

    void UpdateArrowAngle(float angleDegrees)
    {
        arrowObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleDegrees));
    }

    public float GetArrowAngle()
    {
        if(!flipController.IsFlipped())
            return 90 + angle;

        return 90 - angle;
    }
    
    public void UpdateArrowIndicator(float startTime)
    {
        if(startTime == 0)
        {
            arrowSpriteRenderer.sprite = arrowSprites[0];
            return;
        }

        arrowSpriteRenderer.sprite = arrowSprites[GetArrowSprite(startTime)];
    }

    int GetArrowSprite(float startTime)
    {
        float heldTime = Time.time - startTime;
        if(heldTime > MaxHoldDuration)
            return MaxJumpSprite;
    
        return (int) (heldTime/JumpDurationSegment);
    }
}