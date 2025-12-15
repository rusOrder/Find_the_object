using System;
using UnityEngine;
using Zenject;

public interface IGameTimer
{
    float RemainingTime { get; }
    bool IsRunning { get; }
    void StartTimer(float duration);
    void StopTimer();
    event Action OnTimeExpired;
}

public class GameTimer : IGameTimer, ITickable
{
    public float RemainingTime { get; private set; }
    public bool IsRunning { get; private set; }
    public event Action OnTimeExpired;
    
    public void StartTimer(float duration)
    {
        RemainingTime = duration;
        IsRunning = true;
    }
    
    public void StopTimer()
    {
        IsRunning = false;
    }
    
    public void Tick()
    {
        if (!IsRunning) return;
        
        RemainingTime -= Time.deltaTime;
        
        if (RemainingTime <= 0)
        {
            RemainingTime = 0;
            IsRunning = false;
            OnTimeExpired?.Invoke();
        }
    }
}