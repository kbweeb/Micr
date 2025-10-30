using System.Collections.Generic;

namespace MicrDbChequeProcessingSystem.ViewModels;

public class RegionIndexViewModel
{
    public IList<RegionListItem> Items { get; set; } = new List<RegionListItem>();
}
