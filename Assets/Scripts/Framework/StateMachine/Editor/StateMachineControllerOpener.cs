
using StateMachine;
using UnityEditor;
using UnityEditor.Callbacks;

public class StateMachineControllerOpener
{
    [OnOpenAsset(1)]
    public static bool ControllerOpener(int instanceID, int line)
    {
        if (Selection.activeObject != null && Selection.activeObject as StateMachineData != null)
        {
            StateMachineController.ShowExample();
        }
        return false;
    }

}
