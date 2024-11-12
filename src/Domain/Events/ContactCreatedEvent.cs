﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This file is part of the CleanArchitecture.Blazor project.
//     Licensed to the .NET Foundation under the MIT license.
//     See the LICENSE file in the project root for more information.
//
//     Author: neozhu
//     Created Date: 2024-11-08
//     Last Modified: 2024-11-08
//     Description: 
//       Represents a domain event that occurs when a new contact is created.
//       Used to signal other parts of the system that a new contact has been added.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CleanArchitecture.Blazor.Domain.Events;

    public class ContactCreatedEvent : DomainEvent
    {
        public ContactCreatedEvent(Contact item)
        {
            Item = item;
        }

        public Contact Item { get; }
    }
