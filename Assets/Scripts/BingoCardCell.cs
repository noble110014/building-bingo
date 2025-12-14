using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProを使うために必要
using UniRx;

public class BingoCardCell : MonoBehaviour
{
    private TextMeshProUGUI numberText;

    public void Initialize()
    {
        numberText = GetComponent<TextMeshProUGUI>();
    }

    // データを受け取って表示を更新し、購読を開始するメソッド
    public void Bind(BingoSquare squareData)
    {
        // 1. テキストのセット
        numberText.text = squareData.ID;

        // 2. 穴あき状態（IsOpen）を購読
        // 以前の購読が残らないように、このGameObjectの寿命に紐づけます
        squareData.IsOpen
            .Subscribe(isOpen =>
            {
                if(isOpen) ChangeTextColor(Color.red);
            })
            .AddTo(this);
    }

    public void ChangeTextColor(Color color)
    {
        numberText.color = color;
    }
}