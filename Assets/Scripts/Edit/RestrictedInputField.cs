using UnityEngine;
using UnityEngine.UI;

public class RestrictedInputField : MonoBehaviour
{
    InputField inputField; // InputField��Inspector�Őݒ�
    public int minValue = 0;      // �ŏ��l
    public int maxValue = 100;    // �ő�l

    private void Start()
    {
        // ���͎��Ƀ`�F�b�N��ǉ�
        inputField.onValueChanged.AddListener(ValidateInput);
    }

    private void ValidateInput(string input)
    {
        // ���͂����l���ǂ������m�F
        if (int.TryParse(input, out int value))
        {
            // �͈͓��̒l�ɐ���
            value = Mathf.Clamp(value, minValue, maxValue);

            // 5�̔{���ɒ���
            int closestMultipleOfFive = Mathf.RoundToInt(value / 5.0f) * 5;

            // ������̒l��ݒ�
            if (closestMultipleOfFive != value)
            {
                inputField.text = closestMultipleOfFive.ToString();
            }
        }
        else if (!string.IsNullOrEmpty(input))
        {
            // ���l�łȂ��ꍇ�͒l���N���A
            inputField.text = "";
        }
    }

    private void OnDestroy()
    {
        // �C�x���g������
        inputField.onValueChanged.RemoveListener(ValidateInput);
    }
}
