using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class EventManager : SingletonBase<EventManager>
{
    [SerializeField] private int eventInterval;

    private readonly ReactiveProperty<int> _countdownTimePrep = new ReactiveProperty<int>(0);

    private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

    private float _accumulatedTime = 0f;

    [ContextMenu("DebugStartEvent")]
    public void StartEvent()
    {
        _compositeDisposable.Clear();

        this.UpdateAsObservable()
            .Where(_ => _countdownTimePrep.Value > 0)
            .Subscribe(_ =>
            {
                _accumulatedTime += Time.deltaTime;
                if (_accumulatedTime >= eventInterval)
                {
                    _countdownTimePrep.Value -= (int)_accumulatedTime;
                    _accumulatedTime -= (int)_accumulatedTime;

                    //randomƒCƒxƒ“ƒg
                }
            })
            .AddTo(_compositeDisposable);
    }

    public void StopTimer()
    {
        _compositeDisposable.Clear();
    }

    public void ResetTimer()
    {
        _compositeDisposable.Clear();
        _countdownTimePrep.Value = 0;
    }

    protected override void OnDestroy()
    {
        _compositeDisposable.Dispose();
        base.OnDestroy();
    }

}
