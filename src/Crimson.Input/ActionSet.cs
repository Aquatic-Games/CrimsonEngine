namespace Crimson.Input;

public sealed class ActionSet
{
    private bool _enabled;
    
    public readonly string Name;
    
    public readonly Dictionary<string, InputAction> Actions;

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

    public ActionSet(string name)
    {
        Name = name;
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
}