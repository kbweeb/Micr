using System.Collections.Generic;

namespace MicrDbChequeProcessingSystem.ViewModels;

public class AccountTypeIndexViewModel
{
    public IList<AccountTypeListItem> Items { get; set; } = new List<AccountTypeListItem>();
}
