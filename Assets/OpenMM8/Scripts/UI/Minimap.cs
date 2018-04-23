using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour 
{
    public Vector2 m_MinimapScale = new Vector2(0.2f, 0.2f);
    public Vector2 WizardEyeRelativeSize = new Vector2(120.0f, 84.0f);
    public float WizardEyeDotRelativeSize = 5.0f;
    public Vector2 GuiRelativeSize = new Vector2(640, 480);
    public float MapSideLength = 650.0f;

    private Transform m_PlayerTransform;
    private RawImage m_MinimapImage;

    private Vector2Int GameScreenResolution = new Vector2Int();
    private Rect WizardEyeRect;
    private float WizardEyeDotSize;
    private Vector2 MaxDistanceFromMarkerToPlayer = new Vector2();

    private float distancePerPixelX;
    private float distancePerPixelY;

    public List<MinimapMarker> MinimapMarkers = new List<MinimapMarker>();
    public List<MinimapMarker> VisibleMinimapMarkers = new List<MinimapMarker>();

	// Use this for initialization
	void Start () 
	{
        m_MinimapImage = GetComponent<RawImage>();
        m_PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        Debug.Log(m_MinimapImage.rectTransform.sizeDelta.ToString());
        GameScreenResolution.x = Screen.width;
        GameScreenResolution.y = Screen.height;

        WizardEyeRect = GetScreenCoordinates(m_MinimapImage.rectTransform);
        // Convert from bottom-left to top-left which GUI.Box method uses
        // x and y are in center
        WizardEyeRect.x = WizardEyeRect.x + WizardEyeRect.width / 2.0f;
        WizardEyeRect.y = GameScreenResolution.y - (WizardEyeRect.y + WizardEyeRect.height / 2.0f);
        WizardEyeRect.width = (GameScreenResolution.x / GuiRelativeSize.x) * WizardEyeRelativeSize.x;
        WizardEyeRect.height = (GameScreenResolution.y / GuiRelativeSize.y) * WizardEyeRelativeSize.y;
        //Debug.Log("WizardEyeRect: " + WizardEyeRect.ToString());

        WizardEyeDotSize = (GameScreenResolution.y / GuiRelativeSize.y) * WizardEyeDotRelativeSize;

        /*MaxDistanceFromMarkerToPlayer.x = ((MapSideLength * m_MinimapScale.x) / (m_MinimapImage.rectTransform.sizeDelta.x / WizardEyeRect.width)) / 2.0f;
        MaxDistanceFromMarkerToPlayer.y = ((MapSideLength * m_MinimapScale.y) / (m_MinimapImage.rectTransform.sizeDelta.y / WizardEyeRect.height)) / 2.0f;*/

        //distancePerPixelX = 0.38085937500148773193359956145287f / (GameScreenResolution.x / GuiRelativeSize.x);//((MapSideLength * m_MinimapScale.x) / m_MinimapImage.rectTransform.sizeDelta.x);
        //distancePerPixelY = 0.44659375f / (GameScreenResolution.y / GuiRelativeSize.y);  //((MapSideLength * m_MinimapScale.y) / m_MinimapImage.rectTransform.sizeDelta.y);

        float sizeCoeffX = m_MinimapImage.texture.width / (m_MinimapImage.rectTransform.rect.width / m_MinimapImage.uvRect.width);
        float sizeCoeffY = sizeCoeffX * (m_MinimapImage.rectTransform.rect.width / m_MinimapImage.rectTransform.rect.height);

        distancePerPixelX = ((MapSideLength / m_MinimapImage.texture.width) / (GameScreenResolution.x / GuiRelativeSize.x)) * sizeCoeffX;
        distancePerPixelY = ((MapSideLength / m_MinimapImage.texture.height) / (GameScreenResolution.y / GuiRelativeSize.y)) * sizeCoeffY;

        //Debug.Log("uvRect: " + m_MinimapImage.uvRect.ToString());
        //Debug.Log("rectTransform.rect: " + m_MinimapImage.rectTransform.rect.ToString());

        MaxDistanceFromMarkerToPlayer.x = (distancePerPixelX * (WizardEyeRect.width) / 2.0f);
        MaxDistanceFromMarkerToPlayer.y = (distancePerPixelY * (WizardEyeRect.height) / 2.0f);

        //Debug.Log("MaxDistanceFromMarkerToPlayer: " + MaxDistanceFromMarkerToPlayer.ToString());
        //Debug.Log("1px X = " + distancePerPixelX + " distance");
        //Debug.Log("Max distance: X: " + MaxDistanceFromMarkerToPlayer.x);
        //Debug.Log("MinmapTexture width: " + m_MinimapImage.texture.width + " height: " + m_MinimapImage.texture.height);

        InvokeRepeating("test", 0.0f, 0.05f);
    }

    private void test()
    {
        VisibleMinimapMarkers.Clear();
        foreach (MinimapMarker marker in MinimapMarkers)
        {
            float deltaX = m_PlayerTransform.position.x - marker.transform.position.x;
            float deltaY = m_PlayerTransform.position.z - marker.transform.position.z;
            if (marker.enabled &&
                (Mathf.Abs(deltaX) < MaxDistanceFromMarkerToPlayer.x) &&
                (Mathf.Abs(deltaY) < MaxDistanceFromMarkerToPlayer.y))
            {
                /*Rect drawRect = new Rect(
                    (WizardEyeRect.x + deltaX / distancePerPixelX) - WizardEyeDotSize / 2,
                    (WizardEyeRect.y - deltaY / distancePerPixelY) - WizardEyeDotSize / 2,
                    WizardEyeDotSize,
                    WizardEyeDotSize);*/

                marker.DrawRect.x = (WizardEyeRect.x + deltaX / distancePerPixelX) - WizardEyeDotSize / 2;
                marker.DrawRect.y = (WizardEyeRect.y - deltaY / distancePerPixelY) - WizardEyeDotSize / 2;
                marker.DrawRect.width = WizardEyeDotSize;
                marker.DrawRect.height = WizardEyeDotSize;

                VisibleMinimapMarkers.Add(marker);

                /*Debug.Log("DrawRect: " + drawRect.ToString());
                Debug.Log("deltaX: " + deltaX);
                Debug.Log("distancePerPixelX: " + distancePerPixelX);
                Debug.Log("deltaX / distancePerPixelX: " + deltaX / distancePerPixelX);*/

                //DrawMarker(drawRect, marker);
            }
        }
    }

    public Rect GetScreenCoordinates(RectTransform uiElement)
    {
        var worldCorners = new Vector3[4];
        uiElement.GetWorldCorners(worldCorners);
        var result = new Rect(
                      worldCorners[0].x,
                      worldCorners[0].y,
                      worldCorners[2].x - worldCorners[0].x,
                      worldCorners[2].y - worldCorners[0].y);
        return result;
    }

    // Update is called once per frame
    void Update () 
	{
        Rect rect = m_MinimapImage.uvRect;
        rect.x = 0.35f - m_PlayerTransform.position.x / MapSideLength;
        rect.y = 0.35f - m_PlayerTransform.position.z / MapSideLength;
        
        m_MinimapImage.uvRect = rect;
    }

    private void OnGUI()
    {
        foreach (MinimapMarker marker in VisibleMinimapMarkers)
        {
            if (marker == null)
            {
                continue;
            }

            
            GUI.DrawTexture(marker.DrawRect, marker.TextureMark);
        }

        /*Rect drawRect = new Rect(
            WizardEyeRect.x - WizardEyeDotSize / 2,
            WizardEyeRect.y - WizardEyeDotSize / 2,
            WizardEyeDotSize,
            WizardEyeDotSize);*/

        /*foreach (MinimapMarker marker in MinimapMarkers)
        {
            float deltaX = m_PlayerTransform.position.x - marker.transform.position.x;
            float deltaY = m_PlayerTransform.position.z - marker.transform.position.z;
            if (marker.enabled &&
                (Mathf.Abs(deltaX) < MaxDistanceFromMarkerToPlayer.x) &&
                (Mathf.Abs(deltaY) < MaxDistanceFromMarkerToPlayer.y))
            {
                Rect drawRect = new Rect(
                    (WizardEyeRect.x + deltaX / distancePerPixelX) - WizardEyeDotSize / 2,
                    (WizardEyeRect.y - deltaY / distancePerPixelY) - WizardEyeDotSize / 2,
                    WizardEyeDotSize,
                    WizardEyeDotSize);

                DrawMarker(drawRect, marker);
            }
        }*/

        /*Debug.Log("2: " + m_MinimapImage.rectTransform.localPosition.ToString());
        Debug.Log("1: " + m_MinimapImage.rectTransform.rect);
        Debug.Log(rect2.ToString());*/
        //DrawQuad(drawRect, Color.green);
    }

    void DrawMarker(Rect position, MinimapMarker marker)
    {
        marker.TextureMark.SetPixel(0, 0, marker.Color);
        marker.TextureMark.Apply();
        GUI.DrawTexture(position, marker.TextureMark);
    }
}
