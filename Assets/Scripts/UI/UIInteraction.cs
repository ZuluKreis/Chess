using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIInteraction : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    private Button singleplayerButton;
    [SerializeField]
    private Button multiplayerButton;
    [Space(10)]

    [Header("Components")]
    [SerializeField]
    private Animator animator;

    private void Start()
    {
    }

    public void startSingleplayer()
    {
        animator.enabled = true;
        StartCoroutine(animate());
    }

    public void startMultiplayer()
    {

    }

    IEnumerator animate()
    {
        animator.enabled = true;
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(1);
    }
}
