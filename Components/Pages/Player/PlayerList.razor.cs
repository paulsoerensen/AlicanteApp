using AlicanteWeb.Data;
using AlicanteWeb.Models;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace AlicanteWeb.Components.Pages.Player;


public  class PlayerList2 : ComponentBase
{

    public RadzenDataGrid<PlayerModel> playerGrid;
    private ConfirmDialog dialog = default!;

    public IEnumerable<PlayerModel> players { get; set; }
    
    public bool Editing { get; set; } = false;

    [Inject] public required ToastService ToastService { get; set; }
    [Inject] private IRepository _repo { get; set; }
    
    //protected override async Task OnInitializedAsync()
    //{
    //    players = await _repo.GetAllPlayers();
    //}

    #region Grid events

    async Task OnUpdateRow(PlayerModel player)
    {
        await SaveRow(player);
    }
    async void OnCreateRow(PlayerModel player)
    {
        //await UpdateRow(player);
        throw new NotImplementedException("OnCreateRow called");
    }

    async Task EditRow(PlayerModel player)
    {
        await playerGrid.EditRow(player);
        Editing = true;
    }
    async Task SaveRow(PlayerModel model)
    {
        try { 
            var newModel = await _repo.UpsertPlayer(model);
            ToastService.Notify(new(ToastType.Success, "Spilleren er opdateret"));
            await LoadData();
        }
        catch (Exception ex)
        {
            ToastService.Notify(new(ToastType.Warning, $"Der opstod en fejl: {ex.Message}"));
            playerGrid.CancelEditRow(model);
        }
        await playerGrid.Reload();
        Editing = false;
    }
    async Task DeleteRow(PlayerModel model)
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

        try { 
            int i = await _repo.DeletePlayer(model.PlayerId);
            await LoadData();
            ToastService.Notify(new(ToastType.Success, "Spilleren er droppet"));
        }
        catch(Exception ex)
        {
            playerGrid.CancelEditRow(model);
            ToastService.Notify(new(ToastType.Warning, $"Der opstod en fejl: {ex.Message}"));
        }
        await playerGrid.Reload();
        StateHasChanged();
    }
    async Task InsertRow()
    {
        var model = new PlayerModel();
        await playerGrid.InsertRow(model);
        Editing = true;
    }
    void CancelEdit(PlayerModel model)
    {
        playerGrid.CancelEditRow(model);
        Editing = false;
    }
    #endregion

    protected async Task LoadData()
    {
        players = await _repo.GetAllPlayers();
    }

}

