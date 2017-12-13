using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TOD_MinAttribute))]
public class TOD_MaxDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		label = EditorGUI.BeginProperty(position, label, property);

		var attr = attribute as TOD_MaxAttribute;

		if (property.propertyType == SerializedPropertyType.Float)
		{
			EditorGUI.BeginChangeCheck();
			float newValue = EditorGUI.FloatField(position, label, property.floatValue);
			if (EditorGUI.EndChangeCheck()) property.floatValue = Mathf.Min(newValue, attr.max);
			
		}
		else if (property.propertyType == SerializedPropertyType.Integer)
		{
			EditorGUI.BeginChangeCheck();
			int newValue = EditorGUI.IntField(position, label, property.intValue);
			if (EditorGUI.EndChangeCheck()) property.intValue = Mathf.Min(newValue, (int)attr.max);
		}
		else
		{
			EditorGUI.LabelField (position, label.text, "Use TOD_Max with float or int.");
		}

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(TOD_MinAttribute))]
public class TOD_MinDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		label = EditorGUI.BeginProperty(position, label, property);

		var attr = attribute as TOD_MinAttribute;

		if (property.propertyType == SerializedPropertyType.Float)
		{
			EditorGUI.BeginChangeCheck();
			float newValue = EditorGUI.FloatField(position, label, property.floatValue);
			if (EditorGUI.EndChangeCheck()) property.floatValue = Mathf.Max(newValue, attr.min);
		}
		else if (property.propertyType == SerializedPropertyType.Integer)
		{
			EditorGUI.BeginChangeCheck();
			int newValue = EditorGUI.IntField(position, label, property.intValue);
			if (EditorGUI.EndChangeCheck()) property.intValue = Mathf.Max(newValue, (int)attr.min);
		}
		else
		{
			EditorGUI.LabelField (position, label.text, "Use TOD_Min with float or int.");
		}

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(TOD_RangeAttribute))]
public class TOD_RangeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		label = EditorGUI.BeginProperty(position, label, property);

		var attr = attribute as TOD_RangeAttribute;

		if (property.propertyType == SerializedPropertyType.Float)
		{
			EditorGUI.BeginChangeCheck();
			float newValue = EditorGUI.FloatField(position, label, property.floatValue);
			if (EditorGUI.EndChangeCheck()) property.floatValue = Mathf.Clamp(newValue, attr.min, attr.max);
		}
		else if (property.propertyType == SerializedPropertyType.Integer)
		{
			EditorGUI.BeginChangeCheck();
			int newValue = EditorGUI.IntField(position, label, property.intValue);
			if (EditorGUI.EndChangeCheck()) property.intValue = Mathf.Clamp(newValue, (int)attr.min, (int)attr.max);
		}
		else
		{
			EditorGUI.LabelField (position, label.text, "Use TOD_Range with float or int.");
		}

		EditorGUI.EndProperty();
	}
}
