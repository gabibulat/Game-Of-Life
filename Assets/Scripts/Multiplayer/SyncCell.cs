using UnityEngine;
using Mirror;

public class SyncCell : NetworkBehaviour
{
    [SerializeField] private Cell cell;
    [SyncVar] public bool isAlive;
    [SyncVar] public Color cellColor;
    [SyncVar] public int numNeighbours;

    public override void OnStartServer()
    {
        cell.onChangedAlive += OnChangeAlive;
        cell.onChangedNeighbours += OnChangeNeighbours;
    }

    public override void OnStartClient()
    {
        cell.onChangedAlive += OnChangeAlive;
        cell.onChangedNeighbours += OnChangeNeighbours;

        if (isAlive)
        {
            cell.SetAliveColor(cellColor);
            cell.SetCurrentColor(cellColor);
            cell.GetCellSpriteRenderer().color = cellColor;
            cell._isAlive = isAlive;
            cell._aliveNeighbours = numNeighbours;
        }
        else
        {
            cell.GetCellSpriteRenderer().color = cell.GetDeadColor();
            cell.SetAliveColor(cell.GetDeadColor());
            cell.SetCurrentColor(cell.GetDeadColor());
            cell.SetIsAlive(false);
            cell._aliveNeighbours = numNeighbours;
        }
    }

    private void OnChangeNeighbours(object sender, int i)
    {
        if (isServer)
        {
            numNeighbours = i;
            cell._aliveNeighbours = i;
            RPCchangeNeighbours(i);

        }
        else CmdChangeNeighbours(cell.GetAliveNeighbours());
    }

    [Command(requiresAuthority = false)]
    public void CmdChangeNeighbours(int i)
    {
        RPCchangeNeighbours(i);
        if (isServerOnly)
        {
            numNeighbours = i;
            cell._aliveNeighbours = i;
        }
    }

    [ClientRpc]
    public void RPCchangeNeighbours(int i) => cell._aliveNeighbours = i;

    private void OnChangeAlive(object sender, bool b)
    {
        if (isServer)
        {
            isAlive = b;
            if (b) cellColor = cell.GetAliveColor();
            else cellColor = cell.GetDeadColor();
            cell._isAlive = b;
            if (b)
            {
                cell.SetAliveColor(cell.GetAliveColor());
                cell.SetCurrentColor(cell.GetAliveColor());
                cell.GetCellSpriteRenderer().color = cell.GetAliveColor();
            }
            else
            {
                cell.GetCellSpriteRenderer().color = cell.GetDeadColor();
                cell.SetCurrentColor(cell.GetDeadColor());
            }
        }
        else CmdAliveCell(b, cell.GetAliveColor());
    }

    [Command(requiresAuthority = false)]
    public void CmdAliveCell(bool b, Color color)
    {
        isAlive = b;
        if (b) cellColor = color;
        else cellColor = cell.GetDeadColor();

        RPCAliveCell(b, color);
        if (isServerOnly)
        {
            cell._isAlive = b;
            if (b)
            {
                cell.SetAliveColor(color);
                cell.SetCurrentColor(color);
                cell.GetCellSpriteRenderer().color = color;
            }
            else
            {
                cell.GetCellSpriteRenderer().color = cell.GetDeadColor();
                cell.SetCurrentColor(cell.GetDeadColor());
            }
        }
    }

    [ClientRpc]
    public void RPCAliveCell(bool b, Color color)
    {
        cell._isAlive = b;
        if (b)
        {
            cell.SetAliveColor(color);
            cell.SetCurrentColor(color);
            cell.GetCellSpriteRenderer().color = color;
        }
        else
        {
            cell.GetCellSpriteRenderer().color = cell.GetDeadColor();
            cell.SetCurrentColor(cell.GetDeadColor());
        }
    }

}
