using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dog : MonoBehaviour
{
    private Animator animator;
    public AudioClip petSound;
    public GameObject starEffect;
    public AudioSource barkAudioSource;
    //public AudioClip petClip;
    // Start is called before the first frame update
    void Start()
    {
        animator=GetComponent<Animator>();
        // 如果 Inspector 没有手动拖，就自动获取 Dog 身上的 AudioSource
        // if (barkAudioSource == null)
        // {
        //     barkAudioSource = GetComponent<AudioSource>();
        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BeHappy()
    {
        animator.CrossFade("Comfortable",0);
        if(barkAudioSource != null)
        {
            barkAudioSource.Stop();
        }
        GameManager.Instance.hasPetTheDog=true;
        GameManager.Instance.SetContentIndex();
        GameManager.Instance.UpdateTaskText();
        Destroy(starEffect);
        GameManager.Instance.PlaySound(petSound);
        //Invoke("CanControlLuna",1.5f);
    }
    public void CanControlLuna()
    {
        GameManager.Instance.CanControlLuna=true;
    }
}
