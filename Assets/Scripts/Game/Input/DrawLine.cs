using System.Collections.Generic;
using CommonData;
using UnityEngine;
using UnityEngine.UI;
using Game.Goods.Abstract;
using UnityEngine.Analytics;

public class DrawLine : AbstractLevelComponent, Iinput
{
    public const float THICKNESS = 0.05f;

    public Material LineMaterial => lineMaterial;

    [SerializeField] private Toggle draw;
    [SerializeField] private Toggle clear;
    [SerializeField] private Material lineMaterial;

    private Iinput drawSimpleLine;
    private Iinput clearLine;

    private Iinput currentInput;

    void Awake()
    {
        drawSimpleLine = new DrawSimpleLine(this);
        clearLine = new LineClearer(this);

        draw.onValueChanged.AddListener(OnDrawSelect);
        clear.onValueChanged.AddListener(OnClearSelect);

        OnDrawSelect(true);
    }

    public void OnStart(Vector3 ScreenPosition)
    {
        currentInput.OnStart(ScreenPosition);
    }

    public void OnMove(Vector3 ScreenPosition)
    {
        currentInput.OnMove(ScreenPosition);
    }

    public void OnEnd(Vector3 ScreenPosition)
    {
        currentInput.OnEnd(ScreenPosition);
    }

    private void OnDrawSelect(bool state)
    {
        if (!state)
        {
            return;
        }

        currentInput = drawSimpleLine;
    }

    private void OnClearSelect(bool state)
    {
        if (!state)
        {
            return;
        }

        currentInput = clearLine;
    }
}

class DrawSimpleLine : Iinput
{
    private LineComposer CurrentComposer;
    private DrawLine drawer;

    public DrawSimpleLine(DrawLine drawer)
    {
        this.drawer = drawer;
    }

    public void OnEnd(Vector3 ScreenPosition)
    {
        if (CurrentComposer == null)
        {
            return;
        }

        CurrentComposer = null;
    }

    public void OnMove(Vector3 ScreenPosition)
    {
        if (CurrentComposer != null)
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(ScreenPosition);
            position.z = 0;
            CurrentComposer.AddPointInGlobalSpace(position);
        }
    }

    public void OnStart(Vector3 ScreenPosition)
    {
        if (!drawer.levelDataModel.CanDraw(ScreenPosition))
        {
            return;
        } //TODO may remake
        
        Analytics.CustomEvent(
            Constants.IIinputEvent,
            new Dictionary<string, object>()
            {
                { "name", nameof(DrawSimpleLine) }
            }
        );

        Vector3 position = Camera.main.ScreenToWorldPoint(ScreenPosition);
        position.z = 0;
        CurrentComposer = LineComposer.GetLine("line",
            position,
            DrawLine.THICKNESS,
            drawer.LineMaterial
        );
    }
}

class LineClearer : Iinput
{
    private AbstractLevelComponent drawer;

    public LineClearer(AbstractLevelComponent drawer)
    {
        this.drawer = drawer;
    }

    public void OnEnd(Vector3 ScreenPosition)
    {
    }

    public void OnMove(Vector3 ScreenPosition)
    {
        if (!drawer.levelDataModel.CanDraw(ScreenPosition))
        {
            return;
        } //TODO may remake

        Collider2D[] colls = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(ScreenPosition),
            DrawLine.THICKNESS
        );
        for (int i = 0; i < colls.Length; i++)
        {
            LineComposer line = colls[i].GetComponent<LineComposer>();
            if (line != null)
            {
                GameObject.Destroy(line.gameObject);
                break;
            }
        }
    }

    public void OnStart(Vector3 ScreenPosition)
    {
        Analytics.CustomEvent(
            Constants.IIinputEvent,
            new Dictionary<string, object>()
            {
                { "name", nameof(LineClearer) }
            }
        );
    }
}