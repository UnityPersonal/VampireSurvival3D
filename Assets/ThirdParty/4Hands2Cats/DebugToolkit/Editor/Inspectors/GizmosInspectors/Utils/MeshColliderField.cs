using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement] 
public partial class CustomObjectField : VisualElement
{
    [UxmlAttribute] public string label { get; set; } = "Custom Object"; 

    private ObjectField objectField;

    public CustomObjectField()
    {
        objectField = new ObjectField(label) { objectType = typeof(Object) };
        Add(objectField);
    }
}
