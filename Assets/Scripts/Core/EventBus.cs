using UnityEngine;
using System;

public class EventBus 
{
    private static EventBus theInstance;
    public static EventBus Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new EventBus();
            return theInstance;
        }
    }

    public event Action<Vector3, Damage, Hittable> OnDamage;
    
    public void DoDamage(Vector3 where, Damage dmg, Hittable target)
    {
        OnDamage?.Invoke(where, dmg, target);
    }

    public event Action<float, Hittable> OnStandStill;

    public void DoStandStill(float time, Hittable target)
    {
        OnStandStill?.Invoke(time, target);
    }
    
    public event Action<float, Hittable> OnMove;

    public void DoMove(float time, Hittable target)
    {
        OnMove?.Invoke(time, target);
    }
    
    public event Action<float, Hittable, Hittable> OnKill;
    
    public void DoKill(float time, Hittable source, Hittable target)
    {
        OnKill?.Invoke(time, source, target);
    }
    
    public event Action<int> OnWaveEnd;

    public void WaveEnd(int currentWave)
    {
        OnWaveEnd?.Invoke(currentWave);
    }


    public event Action OnNextWave;

    public void NextWave()
    {
        OnNextWave?.Invoke();
    }

}
