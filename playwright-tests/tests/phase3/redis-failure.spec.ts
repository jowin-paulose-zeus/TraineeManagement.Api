import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { TestDataFactory } from "../utils/testDataFactory";

test.describe("Redis Failure Handling", () => {
  let api: ApiClient;

  test.beforeEach(async ({ request }) => {
    const token = await getAdminToken(request);

    api = new ApiClient(request, token);
  });

  test("should return trainee data when Redis is unavailable", async () => {
    const traineeResponse = await api.post("api/Trainee", {
      ...TestDataFactory.trainee({
        firstName: "RedisFailure",
        lastName: "Test",
      }),
      status: 0,
    });

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    // First request populates the cache.
    const firstResponse = await api.get(
      `api/Trainee/id?id=${trainee.id}`,
    );

    expect(firstResponse.status()).toBe(200);

    const firstBody = await firstResponse.json();

    expect(firstBody.id).toBe(trainee.id);
    expect(firstBody.firstName).toBe("RedisFailure");

    /*
     * Stop Redis manually before running the second request.
     *
     * From another terminal:
     *
     * docker stop trainee-redis
     */

    const secondResponse = await api.get(
      `api/Trainee/id?id=${trainee.id}`,
    );

    expect(secondResponse.status()).toBe(200);

    const secondBody = await secondResponse.json();

    expect(secondBody.id).toBe(trainee.id);
    expect(secondBody.firstName).toBe("RedisFailure");
    expect(secondBody.lastName).toBe("Test");
  });
});