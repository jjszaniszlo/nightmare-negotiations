using Godot;

namespace NightmareNegotiations;

public partial class DelegateOnZeroCounter : Node
{
    private int counter;
    
    [Signal] public delegate void OnZeroEventHandler();

    public void Add()
    {
        counter++;
    }
    public void Subtract()
    {
        counter--;
        if (counter == 0)
        {
            EmitSignal(SignalName.OnZero);
        }
    }
}