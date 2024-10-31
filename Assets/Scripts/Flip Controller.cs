using UnityEngine;

public class FlipController : MonoBehaviour
{
    bool isFlipped;
    [SerializeField] GameObject arrowObject;

    ArrowController arrowController;
    JumpController jumpController;
    
    void Awake()
    {
        arrowController = GetComponent<ArrowController>();
        jumpController = GetComponent<JumpController>();
    }

    void OnFlipPlayer()
    {
        if(jumpController.IsWalled()) return;
        
        if(jumpController.IsGrounded())
        {
            ToggleFlip();
            FlipSprite();
            FlipArrow();
            arrowController.UpdateArrow();
        }
    }

    public void FlipSprite()
    {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }

    public void FlipArrow()
    {
        arrowObject.transform.localScale = new Vector2(arrowObject.transform.localScale.x, arrowObject.transform.localScale.y);
    }

    public void ToggleFlip()
    {
        isFlipped = !isFlipped;
    }
    
    public bool IsFlipped()
    {
        return isFlipped;
    }

}