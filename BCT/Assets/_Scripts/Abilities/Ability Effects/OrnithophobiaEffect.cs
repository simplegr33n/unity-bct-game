using UnityEngine;

public class OrnithophobiaEffect : MonoBehaviour {

    Animator[] birds;

    bool STARTED;

	// Use this for initialization
	void Start () {

        // Destroy self in 3 seconds
        Destroy(gameObject, 3f);

        birds = gameObject.GetComponentsInChildren<Animator>();
        foreach (Animator animator in birds)
        {
            animator.Play("move");
        }

        foreach (Animator animator in birds)
        {
            animator.transform.LookAt(transform.position);
        }
        
    }

    private void Update()
    {
        foreach (Animator animator in birds)
        {
            if (animator != null)
            {

                animator.transform.position = Vector3.MoveTowards(animator.transform.position,
                    transform.position,
                    Time.deltaTime * 1f);

                if (animator.transform.position == transform.position)
                {
                    Destroy(animator.gameObject);
                }

            }
        }
    }


}
