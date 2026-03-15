using CommunityToolkit.Mvvm.Input;
using parcel_station1.Models;

namespace parcel_station1.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}