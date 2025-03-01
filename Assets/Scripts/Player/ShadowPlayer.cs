using UnityEngine;

public class ShadowPlayer : MonoBehaviour
{
    private Animator anim;
    private Animator rightHandAnim;
    private Animator leftHandAnim;


    void Awake()
    {
        GetComponents();
    }


    public void animations(int choice, Vector2 localScale, int choice_hands)
    {
        if (localScale.x > 0) transform.localScale = new Vector2(0.82f, 0.72f);
        else transform.localScale = new Vector2(0.82f, 0.72f);

        anim.SetBool("Idle", choice == 1);
        rightHandAnim.SetBool("Idle", choice_hands == 1);
        leftHandAnim.SetBool("Idle", choice_hands == 1);

        anim.SetBool("Running", choice == 2);
        rightHandAnim.SetBool("Running", choice_hands == 2);
        leftHandAnim.SetBool("Running", choice_hands == 2);


        rightHandAnim.SetBool("Throw_light", choice_hands == 3);
        leftHandAnim.SetBool("Throw_light", choice_hands == 3);

        rightHandAnim.SetBool("Holding_heavy", choice_hands == 4);
        leftHandAnim.SetBool("Holding_heavy", choice_hands == 4);

        rightHandAnim.SetBool("Holding_light", choice_hands == 5);
        leftHandAnim.SetBool("Holding_light", choice_hands == 5);
    }


    private void GetComponents()
    {
        anim = GetComponent<Animator>();

        rightHandAnim = transform.Find("Right hand shadow").GetComponentInChildren<Animator>();
        leftHandAnim = transform.Find("Left hand shadow").GetComponentInChildren<Animator>();
    }
}


