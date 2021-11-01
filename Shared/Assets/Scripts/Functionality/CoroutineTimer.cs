using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTimer
{
    private bool repeat;
    private bool stop;
    private bool doOnce;
    private float duration;
    private Action callback;

    public CoroutineTimer(float duration, bool repeat, Action callback, bool doOnce = false)
    {
        this.stop = false;
        this.doOnce = doOnce;
        this.repeat = repeat;
        this.duration = duration;
        this.callback = callback;
    }

    public IEnumerator Start()
    {

        if (doOnce && callback != null)
            callback();

        do
        {
            if (stop == false)
            {
                yield return new WaitForSeconds(duration);

                if (callback != null)
                    callback();
            }
        } while (repeat && !stop);
    }

    public void Stop()
    {
        this.stop = true;
    }

    public bool Repeat
    {
        get { return repeat; }
    }
}
