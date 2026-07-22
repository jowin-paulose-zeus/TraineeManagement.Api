using System.Net;
using System.Net.Http.Json;
using TraineeManagement.Data.Contracts;

namespace TraineeManagement.Api.Clients;

public class TrainingDirectoryClient(HttpClient httpClient): ITrainingDirectoryClient
{
    private readonly HttpClient _httpClient = httpClient;
    public async Task<ProcessingProfileResponse> GetProcessingProfileAsync(int submissionId,string correlationId,CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(HttpMethod.Get,$"api/directory/{submissionId}");

        request.Headers.Add("X-Correlation-ID", correlationId);

        HttpResponseMessage response = await _httpClient.SendAsync(request,cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new KeyNotFoundException(
                $"Submission profile {submissionId} not found.");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"TrainingDirectory returned {response.StatusCode}");
        }

        ProcessingProfileResponse? profile =
            await response.Content.ReadFromJsonAsync<ProcessingProfileResponse>(
                cancellationToken: cancellationToken) ?? throw new InvalidOperationException(
                "TrainingDirectory returned an empty response.");
        return profile;
    }
}