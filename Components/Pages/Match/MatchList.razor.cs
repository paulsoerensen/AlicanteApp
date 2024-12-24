using AlicanteWeb.Data;
using AlicanteWeb.Models;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Radzen.Blazor;

namespace AlicanteWeb.Components.Pages.Match;

public partial class MatchList
{
    RadzenDataGrid<MatchViewModel> matchGrid;
    private ConfirmDialog dialog = default!;

    [Inject] private IRepository _repo { get; set; }
    public IEnumerable<MatchViewModel> matches { get; set; }
    public List<MatchViewModel> persistedMatches { get; set; }
    public IEnumerable<CourseModel> courses { get; set; }

    public bool Editing { get; set; } = false;

    [Inject] public required ToastService ToastService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        courses = await _repo.GetAllCourses();
        await LoadData();
    }

    #region Grid events

    async Task OnUpdateRow(MatchViewModel match)
    {
        await SaveRow(match);
    }
    async void OnCreateRow(MatchViewModel match)
    {
        //await UpdateRow(match);
        throw new NotImplementedException("OnCreateRow called");
    }

    async Task EditRow(MatchViewModel match)
    {
        matches = persistedMatches;
        await matchGrid.EditRow(match);
        Editing = true;
    }
    async Task SaveRow(MatchViewModel match)
    {
        MatchModel model = new()
        {
            MatchId = match.MatchId,
            CourseId = match.CourseId,
            MatchDate = match.MatchDate,
        };
        try { 
            await _repo.UpsertMatch(model);
            ToastService.Notify(new(ToastType.Success, "Matchen er opdateret"));
            await LoadData();
        }
        catch(Exception ex)
        {
            ToastService.Notify(new(ToastType.Warning, "Der opstod en fejl"));
            matchGrid.CancelEditRow(match);
        }
        await matchGrid.Reload();
        Editing = false;
    }
    async Task DeleteRow(MatchViewModel match)
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
            await _repo.DeleteMatch(match.MatchId);
            ToastService.Notify(new(ToastType.Success, "Matchen er slettet"));
            var matchToRemove = persistedMatches.FirstOrDefault(m => m.MatchId == match.MatchId);
            if (matchToRemove != null)
            {
                persistedMatches.Remove(matchToRemove);
                matches = persistedMatches;
            }
        }
        catch (Exception ex)
        {
            matchGrid.CancelEditRow(match);
        }
        await matchGrid.Reload();
    }
    async Task InsertRow()
    {
        var match = new MatchViewModel();
        await matchGrid.InsertRow(match);
        Editing = true;
    }
    void CancelEdit(MatchViewModel match)
    {
        matchGrid.CancelEditRow(match);
        Editing = false;
    }
    #endregion

    protected async Task LoadData()
    {
        try
        {
            matches = await _repo.GetAllMatches();
            persistedMatches = matches.ToList();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
