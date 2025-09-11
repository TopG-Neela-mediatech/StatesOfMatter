using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using TMKOC.StatesOfMatter;
using UnityEngine;

public class HeaderHandler : MonoBehaviour
{
    [SerializeField] private Vector3 enterPosition, exitPosition;

    private float waitTime;

    private void Awake()
    {
        MoveIn();
    }

    private void OnEnable()
    {
        GameManager.OnGameStart += EntryRoutine;
        GameManager.OnGameRestart += EntryRoutine;
    }

    private void Start()
    {
        waitTime = GameManager.Instance.StartWaitTime;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= EntryRoutine;
        GameManager.OnGameRestart -= EntryRoutine;
    }

    [Button]
    public void MoveIn()
    {
        transform.DOLocalMove(enterPosition, 0.75f);
    }

    [Button]
    public void MoveOut()
    {
        transform.DOLocalMove(exitPosition, 0.75f);
    }

    private void EntryRoutine()
    {
        StartCoroutine(EnterAndExit());
    }

    private IEnumerator EnterAndExit()
    {
        MoveIn();

        yield return new WaitForSeconds(waitTime + 0.75f);

        MoveOut();
    }
}
