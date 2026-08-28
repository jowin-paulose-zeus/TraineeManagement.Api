import { test, expect } from "@playwright/test";

test.describe("Training Directory API", () => {
  test("should return processing profile", async ({ request }) => {
    const submissionId = 123;

    const response = await request.get(
      `http://localhost:5044/api/directory/${submissionId}`,
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

  test("should return processing profile for another submission", async ({
    request,
  }) => {
    const submissionId = 999;

    const response = await request.get(
      `http://localhost:5044/api/directory/${submissionId}`,
    );

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body.submissionId).toBe(submissionId);
  });
});
