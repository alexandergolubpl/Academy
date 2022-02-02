using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPanelTool
{
    string ToolName { get; set; }

    void Execute();
}
