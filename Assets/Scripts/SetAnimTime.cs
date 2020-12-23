using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimTime : MonoBehaviour
{
    private Animator an;
    private AnimatorClipInfo[] _animatorClipInfo;
    public float animTime;
    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
        _animatorClipInfo = an.GetCurrentAnimatorClipInfo(0);
        animTime = _animatorClipInfo[0].clip.length;
    }

    // Update is called once per frame
    void Update()
    {
        an.SetFloat("UnTime", (Utility.universalTimeS() % animTime)/animTime);
    }
}