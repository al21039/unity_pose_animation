using UnityEngine;
using UnityEngine.UI;

public class RestrictedInputField : MonoBehaviour
{
    InputField inputField; // InputFieldをInspectorで設定
    public int minValue = 0;      // 最小値
    public int maxValue = 100;    // 最大値

    private void Start()
    {
        // 入力時にチェックを追加
        inputField.onValueChanged.AddListener(ValidateInput);
    }

    private void ValidateInput(string input)
    {
        // 入力が数値かどうかを確認
        if (int.TryParse(input, out int value))
        {
            // 範囲内の値に制限
            value = Mathf.Clamp(value, minValue, maxValue);

            // 5の倍数に調整
            int closestMultipleOfFive = Mathf.RoundToInt(value / 5.0f) * 5;

            // 調整後の値を設定
            if (closestMultipleOfFive != value)
            {
                inputField.text = closestMultipleOfFive.ToString();
            }
        }
        else if (!string.IsNullOrEmpty(input))
        {
            // 数値でない場合は値をクリア
            inputField.text = "";
        }
    }

    private void OnDestroy()
    {
        // イベントを解除
        inputField.onValueChanged.RemoveListener(ValidateInput);
    }
}
