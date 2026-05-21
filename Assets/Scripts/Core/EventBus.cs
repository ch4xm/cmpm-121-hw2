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

    public event Action<float> OnStandStill;

    public void DoStandStill(float time)
    {
        OnStandStill?.Invoke(time);
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
