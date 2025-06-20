using System.Collections.Generic;
using ValoCord.Data;
using ValoCord.Handlers;

namespace ValoCord.ViewModels;

public class HomeViewModel : ViewModelBase
{
    public List<NewsData> RecentPatches => ValorantPatchNotes.FetchLatestPatch();
}