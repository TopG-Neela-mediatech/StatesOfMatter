using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteMaskToggle : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject[] spriteMasks; // Assign in inspector
    [SerializeField] private float fadeTime = 0.5f;

    private bool isEnabled = true;

    private void Awake()
    {
        StartCoroutine(ToggleOnStart());
    }

    private void Start()
    {
        if (spriteMasks.Length == 0)
        {
            Debug.Log("ERror.... empty sprite masks list");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleMasks();
    }

    private void ToggleMasks()
    {
        isEnabled = !isEnabled;

        foreach (var mask in spriteMasks)
        {
            // togle sprite instead
            if (isEnabled)
            {
                mask.GetComponent<SpriteRenderer>().DOFade(1f, fadeTime).OnComplete(() =>
                {
                    mask.GetComponent<SpriteMask>().enabled = isEnabled;
                });
            }
            else
            {
                mask.GetComponent<SpriteRenderer>().DOFade(0f, fadeTime).OnComplete(() =>
                {
                    mask.GetComponent<SpriteMask>().enabled = isEnabled;
                });
            }
        }
    }

    private IEnumerator ToggleOnStart()
    {
        ToggleMasks();


        yield return new WaitForSeconds(1f);

        ToggleMasks();

        yield return new WaitForSeconds(1.5f);

        ToggleMasks();
    }
}
