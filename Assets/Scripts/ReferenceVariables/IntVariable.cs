using Globals;
using UnityEngine;
using UnityEngine.Events;

namespace ReferenceVariables
{
    /// <remarks>
    /// Found this approach from Unity Austin 2017
    /// https://youtu.be/raQ3iHhE_Kk?t=1109
    ///
    /// Event can be separated by Event Architecture from the same approach, but for the sake of simplicity will do.
    /// </remarks>
    [CreateAssetMenu(
        fileName = "New " + nameof(IntVariable),
        menuName = ProjectConstants.ScriptableObjectsAssetMenuName +
                   "/" + ProjectConstants.VariablesAssetMenuName +
                   "/Create new " + nameof(IntVariable))]
    public class IntVariable : ScriptableObject
    {
        [Header("Changed in runtime. Shown here only for debugging.")] 
        [SerializeField] private int _value;

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                Changed?.Invoke(value);
            }
        }

        /// <summary>
        /// Called when <see cref="Value"/> got changed.
        /// </summary>
        /// <remarks>
        /// This is UnityEvent but not an Action, because we can be sure that UnityEvent will be called only in runtime
        /// if we want, and we cant make Action null here in OnDisable, since there is no OnDisable.
        /// UnityEvent is more safe and convenient here.
        /// </remarks>
        public UnityEvent<int> Changed;
    }
}