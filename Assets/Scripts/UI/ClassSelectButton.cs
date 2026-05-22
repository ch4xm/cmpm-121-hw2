using UnityEngine;

public class ClassSelectButton : MonoBehaviour
{
    public string className;

    public void SelectClass()
    {
        EventBus.Instance.ClassSelected(className);
    }
}