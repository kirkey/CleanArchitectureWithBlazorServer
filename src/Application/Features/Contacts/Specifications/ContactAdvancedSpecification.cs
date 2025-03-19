﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This file is part of the CleanArchitecture.Blazor project.
//     Licensed to the .NET Foundation under the MIT license.
//     See the LICENSE file in the project root for more information.
//
//     Author: neozhu
//     Created Date: 2025-03-13
//     Last Modified: 2025-03-13
//     Description: 
//       Defines a specification for applying advanced filtering options to the 
//       Contact entity, supporting different views and keyword-based searches.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Specifications;
#nullable disable warnings
/// <summary>
/// Specification class for advanced filtering of Contacts.
/// </summary>
public class ContactAdvancedSpecification : Specification<Contact>
{
    public ContactAdvancedSpecification(ContactAdvancedFilter filter)
    {
        DateTime today = DateTime.UtcNow;
        var todayrange = today.GetDateRange(ContactListView.TODAY.ToString(), filter.CurrentUser.LocalTimeOffset);
        var last30daysrange = today.GetDateRange(ContactListView.LAST_30_DAYS.ToString(),filter.CurrentUser.LocalTimeOffset);

        Query.Where(q => q.Name != null)
             .Where(filter.Keyword,!string.IsNullOrEmpty(filter.Keyword))
             .Where(q => q.CreatedBy == filter.CurrentUser.UserId, filter.ListView == ContactListView.My && filter.CurrentUser is not null)
             .Where(x => x.Created >= todayrange.Start && x.Created < todayrange.End.AddDays(1), filter.ListView == ContactListView.TODAY)
             .Where(x => x.Created >= last30daysrange.Start, filter.ListView == ContactListView.LAST_30_DAYS);
       
    }
}
