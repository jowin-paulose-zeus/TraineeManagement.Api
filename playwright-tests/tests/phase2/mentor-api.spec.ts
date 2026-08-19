import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";

test.describe("Mentor API", () => {
  let api: ApiClient;

  test.beforeEach(async ({ request }) => {
    const token = await getAdminToken(request);

    api = new ApiClient(request, token);
  });

test("should get all mentors", async () => {
  const response = await api.get(
    "api/Mentor?pageNumber=1&pageSize=10",
  );

  expect(response.status()).toBe(200);

  const body = await response.json();

  expect(body).toBeDefined();
  expect(body).toHaveProperty("pageNumber");
  expect(body).toHaveProperty("pageSize");
  expect(body).toHaveProperty("totalRecords");
  expect(body).toHaveProperty("data");

  expect(body.pageNumber).toBe(1);
  expect(body.pageSize).toBe(10);
  expect(Array.isArray(body.data)).toBeTruthy();
});

  test("should create a mentor", async () => {
    const email = `mentor.${Date.now()}@test.com`;

    const response = await api.post("api/Mentor", {
      firstName: "Playwright",
      lastName: "Mentor",
      email,
      expertise: "TypeScript",
      status: 0,
    });

    expect(response.status()).toBe(201);

    const mentor = await response.json();

    expect(mentor.id).toBeDefined();
    expect(mentor.firstName).toBe("Playwright");
    expect(mentor.lastName).toBe("Mentor");
    expect(mentor.email).toBe(email);
    expect(mentor.expertise).toBe("TypeScript");
    expect(mentor.status).toBe("Active");
  });

  test("should get mentor by id", async () => {
    const email = `mentor.get.${Date.now()}@test.com`;

    const createResponse = await api.post("api/Mentor", {
      firstName: "Get",
      lastName: "Mentor",
      email,
      expertise: "C#",
      status: 0,
    });

    expect(createResponse.status()).toBe(201);

    const createdMentor = await createResponse.json();
    const mentorId = createdMentor.id;

    const response = await api.get(`api/Mentor/id?id=${mentorId}`);

    expect(response.status()).toBe(200);

    const mentor = await response.json();

    expect(mentor).toBeDefined();
    expect(mentor.id).toBe(mentorId);
    expect(mentor.firstName).toBe("Get");
    expect(mentor.lastName).toBe("Mentor");
    expect(mentor.email).toBe(email);
    expect(mentor.expertise).toBe("C#");
    expect(mentor.status).toBe("Active");
  });

  test("should update a mentor", async () => {
    const createResponse = await api.post("api/Mentor", {
      firstName: "Before",
      lastName: "Update",
      email: `mentor.update.${Date.now()}@test.com`,
      expertise: "C#",
      status: 0,
    });

    expect(createResponse.status()).toBe(201);

    const createdMentor = await createResponse.json();
    const mentorId = createdMentor.id;

    const updateResponse = await api.put(`api/Mentor/id?id=${mentorId}`, {
      firstName: "After",
      lastName: "Update",
      email: `mentor.updated.${Date.now()}@test.com`,
      expertise: "TypeScript",
      status: 1,
    });

    expect(updateResponse.status()).toBe(200);

    const updatedMentor = await updateResponse.json();

    expect(updatedMentor.id).toBe(mentorId);
    expect(updatedMentor.firstName).toBe("After");
    expect(updatedMentor.lastName).toBe("Update");
    expect(updatedMentor.expertise).toBe("TypeScript");
    expect(updatedMentor.status).toBe("Inactive");
  });

  test("should delete a mentor", async () => {
    const createResponse = await api.post("api/Mentor", {
      firstName: "Delete",
      lastName: "Mentor",
      email: `mentor.delete.${Date.now()}@test.com`,
      expertise: "C#",
      status: 1,
    });

    expect(createResponse.status()).toBe(201);

    const createdMentor = await createResponse.json();
    const mentorId = createdMentor.id;

    const deleteResponse = await api.delete(
      `api/Mentor/id?id=${mentorId}`,
    );

    expect(deleteResponse.status()).toBe(204);

    const getResponse = await api.get(
      `api/Mentor/id?id=${mentorId}`,
    );

    expect(getResponse.status()).toBe(404);
  });

  test("should return 404 for non-existing mentor", async () => {
    const response = await api.get("api/Mentor/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 404 when updating non-existing mentor", async () => {
    const response = await api.put("api/Mentor/999999999", {
      firstName: "Test",
      lastName: "Mentor",
      email: `mentor.notfound.${Date.now()}@test.com`,
      expertise: "TypeScript",
      status: 0,
    });

    expect(response.status()).toBe(404);
  });

  test("should return 404 when deleting non-existing mentor", async () => {
    const response = await api.delete("api/Mentor/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 400 for invalid mentor data", async () => {
    const response = await api.post("api/Mentor", {
      firstName: "",
      lastName: "",
      email: "invalid-email",
      expertise: "",
      status: "InvalidStatus",
    });

    expect(response.status()).toBe(400);

    const body = await response.json();

    expect(body).toBeDefined();
  });

  test("should return 400 when first name is missing", async () => {
    const response = await api.post("api/Mentor", {
      lastName: "Mentor",
      email: `mentor.missing.${Date.now()}@test.com`,
      expertise: "C#",
      status: 0,
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid email", async () => {
    const response = await api.post("api/Mentor", {
      firstName: "Invalid",
      lastName: "Email",
      email: "invalid-email",
      expertise: "C#",
      status: 0,
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid mentor status", async () => {
    const response = await api.post("api/Mentor", {
      firstName: "Invalid",
      lastName: "Status",
      email: `mentor.status.${Date.now()}@test.com`,
      expertise: "C#",
      status: 99,
    });

    expect(response.status()).toBe(400);
  });

  test("should return 401 without authentication", async ({ request }) => {
    const response = await request.get("api/Mentor");

    expect(response.status()).toBe(401);
  });
});