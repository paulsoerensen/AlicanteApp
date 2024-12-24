namespace AlicanteWeb.Components;

public class AppState
{
    private int _tournamentId;

    public int TournamentId
    {
        get => _tournamentId;
        set
        {
            if (_tournamentId != value)
            {
                _tournamentId = value;
                StateHasChanged();
            }
        }
    }
    public string TournamentName { get; set; }

    public event EventHandler StateChangedHandler;
    
    private void StateHasChanged()
    {
        StateChangedHandler?.Invoke(this, EventArgs.Empty);
    }
}
