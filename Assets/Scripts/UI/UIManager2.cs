using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager2 : MonoBehaviour
{
    [SerializeField] private TMP_Text _actionsUI;
    [SerializeField] private TMP_Text _cellCount;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _drawButton;
    [SerializeField] private TMP_Text _turns;
    [SerializeField] private Button _undoButton;
    [SerializeField] private UICardContainer _uiCardContainer;

    
    public void UpdateTurnsText(int turns) => _turns.text = "Turns: " + turns;
    public void UpdateActionsText(int actions)
    {
        _actionsUI.text = "Actions: " + actions;
        _uiCardContainer.ActionCheck(actions);
    }

    public void UpdateCellCountText(int i) => _cellCount.text = "Alive Cells: " + i;

    public void DisableInteractions(bool b)
    {
        ButtonInteractions(b);
        CardContainerInteractions(b);
        DisableUndoInteractions(false);
    }

    public void DisableUndoInteractions(bool b) => _undoButton.interactable = b;

    public void CardContainerInteractions(bool b) => _uiCardContainer.CardRaycastTarget(b);

    public void ButtonInteractions(bool b)
    {
        _startButton.interactable = b;
        _drawButton.interactable = b;
    }

    public void NewGame(int turns, int actions)
    {
        _uiCardContainer.DisableCards();
        UpdateActionsText(actions);
        UpdateTurnsText(turns);
        UpdateCellCountText(0);
        DisableInteractions(false);
    }

}
