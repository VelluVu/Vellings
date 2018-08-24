using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{

    public class GameControl : MonoBehaviour
    {
        public Texture2D levelTexture;
        public SpriteRenderer levelRenderer;

        public float posOffset = 0.01f;

        int maxX;
        int maxY;

        Node[,] grid;

        Node currentNode;
        Node previousNode;

        Vector3 mousePos;

        private void Start()
        {
            CreateLevel();
        }

        void CreateLevel()
        {
            maxX = levelTexture.width;
            maxY = levelTexture.height;
            grid = new Node[maxX, maxY];

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    Node n = new Node();
                    n.x = x;
                    n.y = y;

                    Color c = levelTexture.GetPixel(x, y);
                    n.isEmpty = (c.a == 0);

                    grid[x, y] = n;
                }
            }
        }

        private void Update()
        {
            GetMousePosition();
            HandleMouseInput();
        }

        void HandleMouseInput()
        {
            if (currentNode == null)
            {
                return;
            }
            if (Input.GetMouseButton(0))
            {
                if (currentNode != previousNode)
                {
                    previousNode = currentNode;

                    Color c = Color.white;
                    c.a = 0;

                    for (int x = -6; x < 6; x++)
                    {
                        for (int y = -6; y < 6; y++)
                        {
                            int _x = x + currentNode.x;
                            int _y = y + currentNode.y;

                            levelTexture.SetPixel(_x, _y, c);
                        }
                    }
                    levelTexture.Apply();
                }
            }
        }

        void GetMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            mousePos = ray.GetPoint(5);
        }

        Node GetNodeFromWorldPos(Vector3 wp)
        {
            int _x = Mathf.RoundToInt(wp.x / posOffset);
            int _y = Mathf.RoundToInt(wp.y / posOffset);

            return GetNode(_x, _y);
        }

        Node GetNode(int x, int y)
        {
            if (x < 0 || y < 0 || x > maxX - 1 || y > maxY - 1)
            {
                return null;
            }
            return grid[x, y];
        }

    }
    public class Node
    {
        public int x;
        public int y;
        public bool isEmpty;
    }
}
