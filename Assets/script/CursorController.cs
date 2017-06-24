using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour {

    public Texture2D[] cursors = new Texture2D[10];

    public void SetCursor(int n = 0) {
        Cursor.SetCursor(cursors[n], new Vector2(16f,16f), CursorMode.ForceSoftware);
    }
    public void SetMenuCursor() {
        Cursor.SetCursor(cursors[4], new Vector2(), CursorMode.ForceSoftware);
    }
    public void SetEnterCursor()
    {
        Cursor.SetCursor(cursors[5], new Vector2(), CursorMode.ForceSoftware);
    }
}
