﻿using System.Collections.Generic;
using System.Linq;
using NodeEditorFramework;
using UnityEngine;

[NodeCanvasType("Dialog Canvas")]
public class DialogNodeCanvas : NodeCanvas
{
    public override string canvasName
    {
        get { return "Dialog"; }
    }

    public string Name = "Dialog";

    public override bool allowRecursion
    {
        get { return true; }
    }

    public override bool CanAddNode(string nodeID)
    {
        // Could use nodeID directly if it were consistently named
        return nodeID.ToLower().Contains("dialog") || NodeTypes.getNodeData(nodeID).adress.StartsWith("Dialog");
    }

    private Dictionary<int, BaseDialogNode> _lstActiveDialogs = new Dictionary<int, BaseDialogNode>();

    public DialogStartNode getDialogStartNode(int dialogID)
    {
        return (DialogStartNode) this.nodes.FirstOrDefault(x => x is DialogStartNode
                                                                && ((DialogStartNode) x).DialogID == dialogID);
    }

    public bool HasDialogWithId(int dialogIdToLoad)
    {
        DialogStartNode node = getDialogStartNode(dialogIdToLoad);
        return node != default(Node) && node != default(DialogStartNode);
    }

    public IEnumerable<int> GetAllDialogId()
    {
        foreach (Node node in this.nodes)
        {
            if (node is DialogStartNode)
            {
                yield return ((DialogStartNode) node).DialogID;
            }
        }
    }

    public void ActivateDialog(int dialogIdToLoad, bool goBackToBeginning)
    {
        BaseDialogNode node;
        if (!_lstActiveDialogs.TryGetValue(dialogIdToLoad, out node))
        {
            node = getDialogStartNode(dialogIdToLoad);
            _lstActiveDialogs.Add(dialogIdToLoad, node);
        }
        else
        {
            if (goBackToBeginning && !(node is DialogStartNode))
            {
                _lstActiveDialogs[dialogIdToLoad] = getDialogStartNode(dialogIdToLoad);
            }
        }
    }

    public BaseDialogNode GetDialog(int dialogIdToLoad)
    {
        BaseDialogNode node;
        if (!_lstActiveDialogs.TryGetValue(dialogIdToLoad, out node))
        {
            ActivateDialog(dialogIdToLoad, false);
        }

        return _lstActiveDialogs[dialogIdToLoad];
    }

    public bool InputToDialog(int dialogIdToLoad, int inputValue)
    {
        BaseDialogNode node;
        if (_lstActiveDialogs.TryGetValue(dialogIdToLoad, out node))
        {
            if (node == null)
            {
                return false;
            }

            node = node.Input(inputValue);
            if (node != null)
                node = node.PassAhead(inputValue);
            _lstActiveDialogs[dialogIdToLoad] = node;
        }

        return true;
    }
}