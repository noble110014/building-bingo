using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;

public class TimerManager : SingletonBase<TimerManager>
{
    private readonly ReactiveProperty<int> _countdownTimePrep = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> CountdownTimePrep => _countdownTimePrep;
    public float ElapsedTime => _countdownTimePrep.Value;

    private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

    private float _accumulatedTime = 0f;

    [SerializeField] private int timeLimit = 60;
    [SerializeField] private int penaltySecond = 5;
    public int TimeLimit { get; set; }

    [SerializeField] private TextMeshProUGUI timerText;

    public void Initialize()
    {
        _countdownTimePrep.Value = timeLimit;
    }

    [ContextMenu("Start Timer")]
    public void StartTimer()
    {
        _compositeDisposable.Clear();
        timerText.text = ToMin(_countdownTimePrep.Value);

        this.UpdateAsObservable()
            .Where(_ => _countdownTimePrep.Value > 0)
            .Subscribe(_ =>
            {
                _accumulatedTime += Time.deltaTime;
                if (_accumulatedTime >= 1f)
                {
                    _countdownTimePrep.Value -= (int)_accumulatedTime;
                    _accumulatedTime -= (int)_accumulatedTime;
                    timerText.text = ToMin(_countdownTimePrep.Value);
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

    public void PenaltyTime()
    {
        _countdownTimePrep.Value -= penaltySecond;
    }

    protected override void OnDestroy()
    {
        _compositeDisposable.Dispose();
        base.OnDestroy();
    }

    private string ToMin(int sec)
    {
        return ((sec / 60).ToString("D2") + ":" + (sec % 60).ToString("D2"));
    }
}
