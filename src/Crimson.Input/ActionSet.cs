namespace Crimson.Input;

public class ActionSet
{
    private bool _enabled;
    
    public readonly string Name;
    
    public readonly Dictionary<string, InputAction> Actions;

    public bool CursorVisible;

    public bool Enabled
    {
        get => _enabled;
        internal set
        {
            _enabled = value;
            foreach ((_, InputAction action) in Actions)
                action.Enabled = value;
        }
    }

    public ActionSet(string name, bool cursorVisible = true)
    {
        Name = name;
        CursorVisible = cursorVisible;
        Actions = [];
    }

    public void AddAction(InputAction action)
    {
        Actions.Add(action.Name, action);
    }

    public InputAction GetAction(string name)
    {
        return Actions[name];
    }

    public void Update()
    {
        foreach ((_, InputAction action) in Actions)
            action.Update();
    }
}