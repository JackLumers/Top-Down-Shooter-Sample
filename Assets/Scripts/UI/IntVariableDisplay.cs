using ReferenceVariables;
using TMPro;
using UnityEngine;

namespace UI
{
    // TODO: Could be generic? Otherwise will need to create display class for every variable type if needed.
    public class IntVariableDisplay : MonoBehaviour
    {
        [SerializeField] private IntVariable _playerHealthVariable;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private string _prefix;

        private void OnEnable()
        {
            UpdateValue(_playerHealthVariable.Value);
            
            _playerHealthVariable.Changed.AddListener(UpdateValue);
        }

        private void OnDisable()
        {
            _playerHealthVariable.Changed.RemoveListener(UpdateValue);
        }

        private void UpdateValue(int value)
        {
            _text.text = $"{_prefix} {value}";
        }
    }
}