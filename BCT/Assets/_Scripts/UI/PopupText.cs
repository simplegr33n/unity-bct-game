using UnityEngine;
using UnityEngine.UI;

public class PopupText : MonoBehaviour {

    public Animator animator;

    void Start()
    {
        AnimatorClipInfo[] clipArray = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipArray[0].clip.length);
        
    }

    public void SetPopupText(string text, Color color)
    {
        Text popupText = animator.GetComponent<Text>();
        popupText.color = color;
        popupText.text = text;
       
    }
}
