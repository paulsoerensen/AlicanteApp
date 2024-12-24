using AlicanteWeb.Data;
using AlicanteWeb.Models;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Radzen.Blazor;

namespace AlicanteWeb.Components.Pages.Result;

public partial class ResultList
{
    public int matchId { get; set; } = 0;

    RadzenDataGrid<ResultViewModel> resultGrid;
    private ConfirmDialog dialog = default!;

    [Inject] private IRepository _repo { get; set; }

    public IEnumerable<ResultViewModel> results { get; set; }
    public IEnumerable<PlayerViewModel> players { get; set; }
    public IEnumerable<MatchViewModel> matches { get; set; }

    protected ResultViewModel insertedRow;

    public bool Editing { get; set; } = false;

    [Inject] public required ToastService ToastService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        matches = await _repo.GetAllMatches();
        ToastService.Notify(new(ToastType.Success, "called"));
    }

    #region Grid events

    async Task OnUpdateRow(ResultViewModel result)
    {
        await SaveRow(result);
    }
    async void OnCreateRow(ResultViewModel result)
    {
        //await UpdateRow(result);
        throw new NotImplementedException("OnCreateRow called");
    }

    async Task EditRow(ResultViewModel result)
    {
        await resultGrid.EditRow(result);
        Editing = true;
    }
    async Task SaveRow(ResultViewModel result)
    {
        ResultModel model = new()
        {
            MatchId = matchId,
            PlayerId = result.PlayerId,
            Birdies = result.Birdies,
            Hcp = result.Hcp,
            Par3 = result.Par3,
            Score = result.Score
        };
        try
        {
            var res = await _repo.UpsertResult(model);
            await LoadResult();
            await LoadPlayers();
            ToastService.Notify(new(ToastType.Success, "Resultat er opdateret"));
        }
        catch (Exception)
        {
            resultGrid.CancelEditRow(result);
            ToastService.Notify(new(ToastType.Warning, "Der opstod en fejl"));
        }
        await resultGrid.Reload();
        Editing = false;

    }
    async Task DeleteRow(ResultViewModel result)
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
            int i = await _repo.DeleteResult(result.ResultId);
            ToastService.Notify(new(ToastType.Success, "Resultatet er slettet"));
            await LoadResult();
        }
        catch (Exception ex)
        {
            resultGrid.CancelEditRow(result);
            ToastService.Notify(new(ToastType.Warning, ex.Message));
        }
        await resultGrid.Reload();
    }
    async Task InsertRow()
    {
        insertedRow = new ResultViewModel();
        await resultGrid.InsertRow(insertedRow);
        Editing = true;
    }
    void CancelEdit(ResultViewModel result)
    {
        resultGrid.CancelEditRow(result);
        Editing = false;
    }
    #endregion

    private async Task MatchChanged(int id)
    {
        matchId = id;

        // Cast the value to the appropriate type (if needed)
        await LoadResult();
        await LoadPlayers();
        StateHasChanged();
    }
    private void PlayerChanged(int playerId)
    {
        // Cast the value to the appropriate type (if needed)

        PlayerViewModel? model = players.SingleOrDefault(p => p.PlayerId == playerId);
        insertedRow.Hcp = model!.Hcp;
    }
    private async Task LoadPlayers()
    {
        try
        {
            players = await _repo.GetPlayersForMatch(matchId);
        }
        catch (Exception)
        {

            throw;
        }
    }
    protected async Task LoadResult()
    {
        try
        {
            results = await _repo.GetResults(matchId);
        }
        catch (Exception)
        {

            throw;
        }

    }

}
