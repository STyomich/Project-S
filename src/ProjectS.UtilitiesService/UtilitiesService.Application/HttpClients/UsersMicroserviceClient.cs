namespace UtilitiesService.Application.HttpClients;

public class UsersMicroserviceClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<bool> IsUserExists(Guid userId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"/api/users/{userId}");

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new HttpRequestException("Bad request", null, System.Net.HttpStatusCode.BadRequest);
            }
            else
                throw new Exception();
        }
        if (response.IsSuccessStatusCode)
            return true;

        return false;
    }
}
