// Unity Classes such as:
// MonoBehaviour, Vector3 and Transform
using UnityEngine;

// Unities MonoBehaviour behavior Class
// MonoBehaviour == allows a person to attach the script to a game object
public class NetBuilder : MonoBehaviour
{
    // This grabs the netPiece Prefab to generate on top of
    public GameObject netPiecePrefab;  // a small cube
    // Spacing of the net, the width, depth and height of the net 
    public int verticalCount = 20;
    public int horizontalCount = 10;
    public float spacing = 0.3f;
    public float width = 6f;
    public float height = 3f;
    public float netDepth = 1.5f;  // How far back the net should be from the goal line
    
    // finish the goal with editable left right and top sides
    public int leftSideCount = 10;  // Number of vertical lines on the left side
    public int rightSideCount = 10;  // Number of vertical lines on the right side
    public int topSideCount = 10;  // Number of horizontal lines on the top side

    // Call when game is started to make the goal
    void Start()
    {
        BuildNet();
    }

    //  This builds the back of the net
    void BuildNet()
    {
        // Build front face - vertical lines spanning from bottom to top
        for (int i = 0; i < verticalCount; i++)
        {
            float xPos = -width / 2 + (i * width / (verticalCount - 1));
            GameObject v = Instantiate(netPiecePrefab, transform);
            // Position at center height, but scale spans from 0 to height
            v.transform.localPosition = new Vector3(xPos, height / 2, netDepth);
            v.transform.localScale = new Vector3(0.05f, height, 0.05f);
        }

        // Build front face - horizontal lines spanning from left to right
        for (int j = 0; j < horizontalCount; j++)
        {
            float yPos = (j * height / (horizontalCount - 1));
            GameObject h = Instantiate(netPiecePrefab, transform);
            // Position at center width, but scale spans the full width
            h.transform.localPosition = new Vector3(0, yPos, netDepth);
            h.transform.localScale = new Vector3(width, 0.05f, 0.05f);
        }
        
        // Build left side - vertical lines from front to back
        BuildLeftSide();
        
        // Build right side - vertical lines from front to back
        BuildRightSide();
        
        // Build top side - horizontal lines from front to back
        BuildTopSide();
    }
    
    // Build the left side of the net
    void BuildLeftSide()
    {
        // Left side: vertical lines at x = -width/2, spanning from y=0 to y=height, z=0 to z=netDepth
        for (int i = 0; i < leftSideCount; i++)
        {
            float yPos = (i * height / (leftSideCount - 1));
            GameObject side = Instantiate(netPiecePrefab, transform);
            // Position at center depth, but scale spans from 0 to netDepth
            side.transform.localPosition = new Vector3(-width / 2, yPos, netDepth / 2);
            side.transform.localScale = new Vector3(0.05f, 0.05f, netDepth);
        }
    }
    
    // Build the right side of the net   
    void BuildRightSide()
    {
        // Right side: vertical lines at x = width/2, spanning from y=0 to y=height, z=0 to z=netDepth
        for (int i = 0; i < rightSideCount; i++)
        {
            float yPos = (i * height / (rightSideCount - 1));
            GameObject side = Instantiate(netPiecePrefab, transform);
            // Position at center depth, but scale spans from 0 to netDepth
            side.transform.localPosition = new Vector3(width / 2, yPos, netDepth / 2);
            side.transform.localScale = new Vector3(0.05f, 0.05f, netDepth);
        }
    }
    
    // Build the top side of the net
    void BuildTopSide()
    {
        // Top side: horizontal lines at y = height, spanning from x=-width/2 to x=width/2, z=0 to z=netDepth
        for (int i = 0; i < topSideCount; i++)
        {
            float xPos = -width / 2 + (i * width / (topSideCount - 1));
            GameObject side = Instantiate(netPiecePrefab, transform);
            // Position at center depth, but scale spans from 0 to netDepth
            side.transform.localPosition = new Vector3(xPos, height, netDepth / 2);
            side.transform.localScale = new Vector3(0.05f, 0.05f, netDepth);
        }
    }
}
