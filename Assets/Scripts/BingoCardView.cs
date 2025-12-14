using System.Collections.Generic;
using UnityEngine;
using UniRx;
using JetBrains.Annotations;

[RequireComponent(typeof(BingoManager))]
public class BingoCardView : MonoBehaviour
{
    // Inspectorで25個のBingoCellを割り当てておく
    // （Hierarchy順に並んでいる想定）
    [SerializeField] private List<BingoCardCell> bingoCardCells;

    private const int BOARD_SIZE = 5;

    public void Initialize()
    {
        // Managerのカードデータ全体を監視
        BingoManager.Instance.BingoCard
            .Where(card => card != null) // nullじゃないときだけ
            .Subscribe(card =>
            {
                GenerateGridVisuals(card);
            })
            .AddTo(this);
    }
 
    // ここがリクエストの書き直し部分です
    private void GenerateGridVisuals(BingoSquare[,] cardData)
    {
        // ガード節：Inspector設定漏れ防止
        if (bingoCardCells.Count < BOARD_SIZE * BOARD_SIZE)
        {
            Debug.LogError("BingoCellsの数が足りません。Inspectorで25個登録してください。");
            return;
        }

        // 5x5 の二重ループでデータを回す
        for (int y = 0; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                // 1. データを取り出す
                BingoSquare squareData = cardData[y, x];

                // 2. 対応するUI（BingoCell）を取得する
                // 2次元座標 (y, x) を 1次元インデックス (0~24) に変換
                int index = (y * BOARD_SIZE) + x;
                BingoCardCell cellView = bingoCardCells[index];

                // 3. データとUIを紐付ける (Bind)
                cellView.Initialize();
                cellView.Bind(squareData);
            }
        }
    }

    public void AccessBingoCardTextColor(string id, Color color)
    {
        foreach (var cell in bingoCardCells)
        {
            if(cell.numberText.text == id)
            {
                cell.ChangeTextColor(color);
            }
        }
    }
}