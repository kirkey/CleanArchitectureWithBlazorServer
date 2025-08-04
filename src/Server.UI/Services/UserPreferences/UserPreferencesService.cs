// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CleanArchitecture.Blazor.Server.UI.Services.UserPreferences;

public interface IUserPreferencesService
{
    /// <summary>
    ///     Saves UserPreference in local storage
    /// </summary>
    /// <param name="userPreferences">The userPreferences to save in the local storage</param>
    public Task SaveUserPreferences(UserPreference userPreferences);

    /// <summary>
    ///     Loads UserPreference in local storage
    /// </summary>
    /// <returns>UserPreference object. Null when no settings were found.</returns>
    public Task<UserPreference> LoadUserPreferences();
}

public class UserPreferencesService(ProtectedLocalStorage localStorage) : IUserPreferencesService
{
    private const string Key = "userPreferences";

    public async Task SaveUserPreferences(UserPreference userPreferences)
    {
        await localStorage.SetAsync(Key, userPreferences).ConfigureAwait(false);
    }

    public async Task<UserPreference> LoadUserPreferences()
    {
        try
        {
            var result = await localStorage.GetAsync<UserPreference>(Key).ConfigureAwait(false);
            if (result.Success && result.Value is not null) return result.Value;
            return new UserPreference();
        }
        catch (CryptographicException)
        {
            await localStorage.DeleteAsync(Key).ConfigureAwait(false);
            return new UserPreference();
        }
        catch (Exception)
        {
            return new UserPreference();
        }
    }
}