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
    
    public event Action<Hittable> OnCastSpell;
    
    public void DoCastSpell(Hittable source)
    {
        OnCastSpell?.Invoke(source);
    }
    
    public event Action<int> OnWaveEnd;

    public void DoWaveEnd(int currentWave)
    {
        OnWaveEnd?.Invoke(currentWave);
    }

    public event Action OnNextWave;

    public void DoNextWave()
    {
        OnNextWave?.Invoke();
    }

    public event Action OnHideRewardScreen;

    public void DoHideRewardScreen()
    {
        OnHideRewardScreen?.Invoke();
    }

    public event Action<Relic> OnRelicPickup;
    public void PickupRelic(Relic relic)
    {
        OnRelicPickup?.Invoke(relic);
    }

    public event Action<string> OnLevelSelected;
    public void LevelSelected(string level)
    {
        OnLevelSelected?.Invoke(level);
    }

    public event Action<string> OnClassSelected;
    public void ClassSelected(string playerClass)
    {
        OnClassSelected?.Invoke(playerClass);
    }

    public event Action<string, string> OnGameStart;
    public void StartGame(string levelName, string playerClass)
    {
        OnGameStart?.Invoke(levelName, playerClass);
    }
}
