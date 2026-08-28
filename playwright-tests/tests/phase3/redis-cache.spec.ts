import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { TestDataFactory } from "../utils/testDataFactory";

test.describe("Redis Cache", () => {
  let api: ApiClient;

  test.beforeEach(async ({ request }) => {
    const token = await getAdminToken(request);

    api = new ApiClient(request, token);
  });

  test("should return the same trainee data on repeated reads", async () => {
    const traineeResponse = await api.post("api/Trainee", {
      ...TestDataFactory.trainee({
        firstName: "Cache",
        lastName: "Test",
      }),
      status: 0,
    });

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    const firstResponse = await api.get(`api/Trainee/id?id=${trainee.id}`);

    expect(firstResponse.status()).toBe(200);

    const firstBody = await firstResponse.json();

    expect(firstBody.id).toBe(trainee.id);
    expect(firstBody.firstName).toBe("Cache");

    const secondResponse = await api.get(`api/Trainee/id?id=${trainee.id}`);

    expect(secondResponse.status()).toBe(200);

    const secondBody = await secondResponse.json();

    expect(secondBody).toEqual(firstBody);
  });

  test("should invalidate trainee cache after update", async () => {
    const createResponse = await api.post("api/Trainee", {
      ...TestDataFactory.trainee({
        firstName: "BeforeCache",
        lastName: "Update",
      }),
      status: 0,
    });

    expect(createResponse.status()).toBe(201);

    const trainee = await createResponse.json();

    // First read - populates cache
    const firstResponse = await api.get(`api/Trainee/id?id=${trainee.id}`);

    expect(firstResponse.status()).toBe(200);

    const firstBody = await firstResponse.json();

    expect(firstBody.firstName).toBe("BeforeCache");

    // Update trainee
    const updatedEmail = TestDataFactory.uniqueEmail("cache-update");

    const updateResponse = await api.put(`api/Trainee/id?id=${trainee.id}`, {
      firstName: "AfterCache",
      lastName: "Update",
      email: updatedEmail,
      techStack: "TypeScript",
      status: 0,
    });

    expect(updateResponse.status()).toBe(200);

    // Read again after update
    const secondResponse = await api.get(`api/Trainee/id?id=${trainee.id}`);

    expect(secondResponse.status()).toBe(200);

    const secondBody = await secondResponse.json();

    expect(secondBody.id).toBe(trainee.id);
    expect(secondBody.firstName).toBe("AfterCache");
    expect(secondBody.lastName).toBe("Update");
    expect(secondBody.email).toBe(updatedEmail);
  });

  test("should return 404 for non-existing trainee", async () => {
    const response = await api.get("api/Trainee/id?id=999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 401 without authentication", async ({ request }) => {
    const response = await request.get("api/Trainee/id?id=1");

    expect(response.status()).toBe(401);
  });
});
