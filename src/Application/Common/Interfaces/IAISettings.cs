namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

/// <summary>
/// AI configuration settings interface
/// </summary>
public interface IAiSettings
{
    /// <summary>
    /// Gets the Gemini API key
    /// </summary>
    string GeminiApiKey { get; }
} 