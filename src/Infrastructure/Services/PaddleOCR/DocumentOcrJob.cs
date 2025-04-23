// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
using CleanArchitecture.Blazor.Application.Features.Documents.Caching;
using CleanArchitecture.Blazor.Domain.Common.Enums;

namespace CleanArchitecture.Blazor.Infrastructure.Services.PaddleOCR;

public class DocumentOcrJob(
    IApplicationHubWrapper appNotificationService,
    IApplicationDbContext context,
    IHttpClientFactory httpClientFactory,
    ILogger<DocumentOcrJob> logger)
    : IDocumentOcrJob
{
    private readonly Stopwatch _timer = new();

    public void Do(int id)
    {
        Recognition(id, CancellationToken.None).Wait();
    }

    public async Task Recognition(int id, CancellationToken cancellationToken)
    {
        try
        {
            using var client = httpClientFactory.CreateClient("ocr");
            _timer.Start();

            var doc = await context.Documents.FindAsync(id);
            if (doc == null)
            {
                logger.LogWarning("Document with Id {Id} not found.", id);
                return;
            }

            await appNotificationService.JobStarted(id, doc.Title!);
            CancelCacheToken();

            if (string.IsNullOrEmpty(doc.Url))
            {
                logger.LogWarning("Document URL is null or empty for Id {Id}.", id);
                return;
            }

            var response = await client.GetAsync($"?imageUrl={doc.Url}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync(cancellationToken);
                if (result.Length > 4000)
                {
                    result = result.Substring(0, 4000);
                }

                doc.Status = JobStatus.Done;
                doc.Description = "Recognition result: success";
                doc.Content = result;

                await context.SaveChangesAsync(cancellationToken);
                await appNotificationService.JobCompleted(id, doc.Title!);
                CancelCacheToken();

                _timer.Stop();
                logger.LogInformation(
                    "Image recognition completed successfully {@Document}. Id: {Id}, Elapsed Time: {ElapsedMilliseconds}ms", doc,
                    id, _timer.ElapsedMilliseconds);
            }
            else
            {
                var result = await response.Content.ReadAsStringAsync(cancellationToken);
                doc.Status = JobStatus.Pending;
                doc.Content = result;

                await context.SaveChangesAsync(cancellationToken);
                await appNotificationService.JobCompleted(id, $"Error: {result}");
                CancelCacheToken();

                logger.LogError("Image recognition failed for Id: {Id}, Status Code: {StatusCode}, Message: {Message}",
                    id, response.StatusCode, result);
            }
        }
        catch (Exception ex)
        {
            await appNotificationService.JobCompleted(id, $"Error: {ex.Message}");
            logger.LogError(ex, "Image recognition error for Id: {Id}, Message: {Message}", id, ex.Message);
        }
        finally
        {
            if (_timer.IsRunning)
            {
                _timer.Stop();
            }
        }
    }

    private void CancelCacheToken()
    {
        DocumentCacheKey.Refresh();
    }
}
#pragma warning disable CS8981
internal class OcrResult
{
    [JsonPropertyName("resultcode")] public string? ResultCode { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("data")] public List<List<List<dynamic>>>? Data { get; set; }
}
#pragma warning restore CS8981