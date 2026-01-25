using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Joined;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;

namespace MindCabinet.Client.Services.DataPresenters;



public partial class ContextPostsSupplier {
    public int GetCurrentPage() => this.CurrentPage;
    public void SetCurrentPage( int page ) => this.CurrentPage = page;

    public int GetMaxPostsPerPage() => this.MaxPostsPerPage;
    public void SetMaxPostsPerPage( int perPage ) => this.MaxPostsPerPage = perPage;

    public bool GetSortOrder() => this.SortAscendingByDate;
    public void SetSortOrder( bool isAscending ) => this.SortAscendingByDate = isAscending;
}
