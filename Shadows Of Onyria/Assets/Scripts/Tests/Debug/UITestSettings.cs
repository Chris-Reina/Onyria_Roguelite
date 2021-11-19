using DoaT;

public class UITestSettings : CanvasGroupController, IInputUIComponent
{
    private void Start()
    {
        UIMasterController.OnActivateMenu += ShowUI;
    }

    public void OnSelectInput() { }

    public void OnReturnInput()
    {
        HideUI();
    }
}
