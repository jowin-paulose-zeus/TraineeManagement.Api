import { test, expect } from "@playwright/test";

test.describe("Health Checks", () => {
  test("should return healthy liveness status", async ({ request }) => {
    const response = await request.get("health/live");

    expect(response.status()).toBe(200);
  });

  test("should return healthy readiness status", async ({ request }) => {
    const response = await request.get("health/ready");

    expect(response.status()).toBe(200);
  });
});
