using AlicanteWeb.Data;
using AlicanteWeb.Models;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace AlicanteWeb.Components.Pages.Tournament;

public partial class TournamentList
{
    RadzenDataGrid<TournamentModel> tournamentGrid;
    private ConfirmDialog dialog = default!;

    [Inject] public required IRepository _repo { get; set; }
    [Inject] public required ToastService ToastService { get; set; }


    public IEnumerable<TournamentModel> tournaments { get; set; } = default!;
    public int activeId { get; set; }
    public int deleteId { get; set; }
    public bool Editing { get; set; } = false;

    private async Task<GridDataProviderResult<TournamentModel>> TournamentDataProvider(GridDataProviderRequest<TournamentModel> request)
    {
        if (tournaments is null) 
            await LoadData();

        return await Task.FromResult(request.ApplyTo(tournaments));
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadData();
        activeId = tournaments.Where(x => x.Active == true).Select(x => x.TournamentId).SingleOrDefault();
        //_appState.TournamentId = activeId;

        StateHasChanged();
    }

    #region Grid events

    async Task OnUpdateRow(TournamentModel tournament)
    {
        await SaveRow(tournament);
    }
    async void OnCreateRow(TournamentModel tournament)
    {
        //await UpdateRow(tournament);
        throw new NotImplementedException("OnCreateRow called");
    }

    async Task EditRow(TournamentModel tournament)
    {
        await tournamentGrid.EditRow(tournament);
        Editing = true;
    }
    async Task SaveRow(TournamentModel model)
    {
        try
        {
            await _repo.UpsertTournament(model);
            await LoadData();
        }
        catch (Exception ex)
        {
            tournamentGrid.CancelEditRow(model);
        }
        Editing = false;
    }

    async Task DeleteRow(TournamentModel tournament)
    {
        var options = new ConfirmDialogOptions { Size = DialogSize.Small };

        var confirmation = await dialog.ShowAsync(
            title: "Hva sååå!",
            message1: "Skal den slettes?",
            confirmDialogOptions: options);

        if (!confirmation)
        {
            return;
        }
        try
        {
            int i = await _repo.DeleteTournament(tournament.TournamentId);
            ToastService.Notify(new(ToastType.Success, "Turneringen er slettet"));
            await LoadData();
        }
        catch (Exception ex)
        {
            tournamentGrid.CancelEditRow(tournament);
            ToastService.Notify(new(ToastType.Warning, ex.Message));
        }
        await tournamentGrid.Reload();
    }

    async Task InsertRow()
    {
        var tournament = new TournamentModel();
        await tournamentGrid.InsertRow(tournament);
        Editing = true;
    }
    void CancelEdit(TournamentModel tournament)
    {
        tournamentGrid.CancelEditRow(tournament);
        Editing = false;
    }
    #endregion

    async Task ChangeActive()
    {
        //var res = await http.PostAsJsonAsync("/api/Tournament/active", activeId);
        //if (res!.IsSuccessStatusCode)
        //{
        //    _appState.TournamentId = activeId;
        //    ToastService.Notify(new(ToastType.Success, "Turneringen er nu aktivt"));
        //}
    }

    protected async Task LoadData()
    {
        tournaments = await _repo.GetTournaments();
        //var res = await http.GetFromJsonAsync<BaseResponseModel>("/api/Tournament");
        //if (res != null && res.Success)
        //{
        //    tournaments = JsonConvert.DeserializeObject<List<TournamentModel>>(res.Data.ToString());
        //}
    }

}
