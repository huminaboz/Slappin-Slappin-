using System;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class InGameMessageAnnouncer : Singleton<InGameMessageAnnouncer>
{
    [SerializeField] private TextMeshProUGUI[] announcements;
    [SerializeField] private GameObject activator;
    
    
    private Coroutine clearTextCo;

    private void Start()
    {
        //Get it off the screen
        FadeAnnouncement(0f, 0f);
        activator.SetActive(true);
    }

    public void MakeAnouncement(string announcement, float duration, float fadeDuration)
    {
        //Pop it up quickly
        FadeAnnouncement(.25f, 1f);
        
        for (int i = 0; i < announcements.Length; i++)
        {
            announcements[i].text = announcement;
        }

        if (clearTextCo is not null)
        {
            StopCoroutine(clearTextCo);
        }

        //Fade it out after a little time
        clearTextCo = StartCoroutine(BozUtilities
            .DoAfterRealTimeDelay(duration, () => { FadeAnnouncement(fadeDuration, .0f); }));
    }

    private void FadeAnnouncement(float duration, float goal)
    {
        foreach (TextMeshProUGUI text in announcements)
        {
            text.DOFade(goal, duration).SetEase(Ease.OutQuad).SetUpdate(true);
        }
    }
}