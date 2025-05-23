﻿//------------------------------------------------------------------------------
// <auto-generated>
// CleanArchitecture.Blazor - MIT Licensed.
// Author: neozhu
// CreatedOn/Modified: 2025-03-19
// Validator for CreateContactCommand: enforces max lengths and required fields for Contact entities.
// Docs: https://docs.cleanarchitectureblazor.com/features/contact
// </auto-generated>
//------------------------------------------------------------------------------

// Usage:
// Validates CreateContactCommand ensuring constraints (e.g., non-empty Name field) before execution.


namespace CleanArchitecture.Blazor.Application.Features.Contacts.Commands.Create;

public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
{
        public CreateContactCommandValidator()
        {
                RuleFor(v => v.Name).MaximumLength(50).NotEmpty(); 
    RuleFor(v => v.Description).MaximumLength(255); 
    RuleFor(v => v.Email).MaximumLength(255); 
    RuleFor(v => v.PhoneNumber).MaximumLength(255); 
    RuleFor(v => v.Country).MaximumLength(255); 

        }
       
}

