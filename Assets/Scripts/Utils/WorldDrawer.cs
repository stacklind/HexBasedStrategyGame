using UnityEngine;

public class WorldDrawer
{
    // Create text in world with optional values
    public static TextMesh CreateWorldText(string text, string name = "World_Text", Transform parent = null, Vector3 localPosition = default, 
        Vector3 rotationDegrees = default, int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, 
        TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, name, localPosition, rotationDegrees, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }

    // Create text in world
    public static TextMesh CreateWorldText(Transform parent, string text, string name, Vector3 localPosition, Vector3 rotationDegrees, int fontSize,
        Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject(name, typeof(TextMesh));
        gameObject.transform.SetParent(parent, false);
        gameObject.transform.localPosition = localPosition;
        gameObject.transform.Rotate(rotationDegrees);
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
}
