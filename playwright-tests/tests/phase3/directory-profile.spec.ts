import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";

test.describe("Training Directory HTTP Client", () => {
  let api: ApiClient;

  test.beforeEach(async ({ request }) => {
    const token = await getAdminToken(request);

    api = new ApiClient(request, token);
  });

  test("should retrieve processing profile through TrainingDirectory API", async () => {
    const submissionId = 123;

    const response = await api.get(
      `api/directory-profile/${submissionId}`,
    );

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body).toBeDefined();

    expect(body.submissionId).toBe(submissionId);

    expect(body.requiresChecksumValidation).toBe(true);

    expect(body.requiresVirusScan).toBe(false);

    expect(body.maxFileSizeMb).toBe(10);

    expect(body.allowedExtensions).toEqual([
      ".pdf",
      ".png",
      ".jpg",
      ".jpeg",
      ".doc",
      ".docx",
    ]);
  });

  test("should return different processing profile for another submission", async () => {
    const submissionId = 456;

    const response = await api.get(
      `api/directory-profile/${submissionId}`,
    );

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body.submissionId).toBe(submissionId);
  });

  test("should return 401 without authentication", async ({
    request,
  }) => {
    const response = await request.get(
      "api/directory-profile/123",
    );

    expect(response.status()).toBe(401);
  });
});