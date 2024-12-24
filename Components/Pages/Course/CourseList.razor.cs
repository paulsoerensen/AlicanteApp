using AlicanteWeb.Components.Layout;
using AlicanteWeb.Data;
using AlicanteWeb.Models;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Radzen.Blazor;

namespace AlicanteWeb.Components.Pages.Course;

public partial class CourseList
{
    RadzenDataGrid<CourseModel> coursesGrid;
    private ConfirmDialog dialog = default!;

    [Inject] public required ToastService ToastService { get; set; }
    [Inject] private IRepository _repo { get; set; }
    public IEnumerable<CourseModel> courses { get; set; }
    public AppModal Modal { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadData();
    }

    async Task EditRow(CourseModel course)
    {
        await coursesGrid.EditRow(course);
    }

    void OnUpdateRow(CourseModel course)
    {
        ;
    }

    async Task SaveRow(CourseModel model)
    {
        try
        {
            await _repo.UpsertCourse(model);
            await LoadData();
            ToastService.Notify(new(ToastType.Success, "Banen er opdateret"));
        }
        catch (Exception ex)
        {
            ToastService.Notify(new(ToastType.Warning, "Der opstod en fejl"));
            coursesGrid.CancelEditRow(model);
        }
        await coursesGrid.Reload();
    }

    void CancelEdit(CourseModel course)
    {
        coursesGrid.CancelEditRow(course);
    }
    async Task DeleteRow(CourseModel course)
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

        await _repo.DeleteCourse(course.CourseId);
        if (courses.Contains(course))
        {
            await coursesGrid.Reload();
        }
        else
        {
            coursesGrid.CancelEditRow(course);
            await coursesGrid.Reload();
        }
    }
    async Task InsertRow()
    {
        var course = new CourseModel();
        await coursesGrid.InsertRow(course);
    }
    void OnCreateRow(CourseModel course)
    {
        ;
    }

    protected async Task LoadData()
    {
        courses = await _repo.GetAllCourses();
    }
}
