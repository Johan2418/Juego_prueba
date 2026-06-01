using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class WorldSpaceLabel : MonoBehaviour
{
    public string labelText = "Label";
    public Color labelColor = Color.white;
    public float yOffset = 1.3f;
    public float fontSize = 8f;
    public bool showBackground = true;

    private TextMeshPro _tmp;
    private SpriteRenderer _bg;

    private void OnEnable()
    {
        SetupLabel();
    }

    private void Update()
    {
        if (_tmp == null || _bg == null) SetupLabel();
        
        if (_tmp != null)
        {
            if (_tmp.text != labelText) _tmp.text = labelText;
            if (_tmp.color != labelColor) _tmp.color = labelColor;
            
            // Keep text upright and independent of parent scale
            Vector3 worldScale = transform.lossyScale;
            _tmp.transform.localScale = new Vector3(
                worldScale.x != 0 ? 1.0f / worldScale.x : 1,
                worldScale.y != 0 ? 1.0f / worldScale.y : 1,
                1);
            
            _tmp.fontSize = fontSize;
            _tmp.transform.localPosition = new Vector3(0, yOffset, -0.2f);

            if (_bg != null)
            {
                _bg.enabled = showBackground;
                _bg.transform.localPosition = _tmp.transform.localPosition + new Vector3(0, 0, 0.1f);
                
                // Adjust BG scale based on text size (world units)
                float bgW = (_tmp.preferredWidth + 0.4f);
                float bgH = (_tmp.preferredHeight + 0.1f);
                _bg.transform.localScale = new Vector3(bgW, bgH, 1);
            }
        }
    }

    private void SetupLabel()
    {
        // 1. Text
        Transform textTransform = transform.Find("LabelText");
        GameObject textObj;
        if (textTransform == null)
        {
            textObj = new GameObject("LabelText");
            textObj.transform.SetParent(transform);
        }
        else
        {
            textObj = textTransform.gameObject;
        }

        _tmp = textObj.GetComponent<TextMeshPro>() ?? textObj.AddComponent<TextMeshPro>();
        _tmp.text = labelText;
        _tmp.color = labelColor;
        _tmp.fontSize = fontSize;
        _tmp.alignment = TextAlignmentOptions.Center;
        _tmp.fontStyle = FontStyles.Bold;
        _tmp.sortingOrder = 1000;

        // 2. Background
        Transform bgTransform = transform.Find("LabelBG");
        GameObject bgObj;
        if (bgTransform == null)
        {
            bgObj = new GameObject("LabelBG");
            bgObj.transform.SetParent(transform);
            _bg = bgObj.AddComponent<SpriteRenderer>();
            
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            _bg.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            _bg.color = new Color(0, 0, 0, 0.85f);
            _bg.sortingOrder = 999;
        }
        else
        {
            _bg = bgTransform.GetComponent<SpriteRenderer>();
        }
    }
}
