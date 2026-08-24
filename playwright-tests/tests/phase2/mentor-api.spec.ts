import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { CleanupHelper } from "../utils/cleanupHelper";
import { TestDataFactory } from "../utils/testDataFactory";

test.describe("Mentor API", () => {
  let api: ApiClient;
  let cleanup: CleanupHelper;

  test.beforeEach(async ({ request }) => {
    const token = await getAdminToken(request);

    api = new ApiClient(request, token);
    cleanup = new CleanupHelper(api);
  });

  test("should get all mentors", async () => {
    const response = await api.get("api/Mentor?pageNumber=1&pageSize=10");

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
    const mentorData = TestDataFactory.mentor({
      firstName: "Playwright",
      lastName: "Mentor",
      expertise: "TypeScript",
    });

    const response = await api.post("api/Mentor", {
      ...mentorData,
      status: 0,
    });

    expect(response.status()).toBe(201);

    const mentor = await response.json();

    expect(mentor.id).toBeDefined();
    expect(mentor.firstName).toBe("Playwright");
    expect(mentor.lastName).toBe("Mentor");
    expect(mentor.email).toBe(mentorData.email);
    expect(mentor.expertise).toBe("TypeScript");
    expect(mentor.status).toBe("Active");

    await cleanup.deleteMentor(mentor.id);
  });

  test("should get mentor by id", async () => {
    const mentorData = TestDataFactory.mentor({
      firstName: "Get",
      lastName: "Mentor",
      expertise: "C#",
    });

    const createResponse = await api.post("api/Mentor", {
      ...mentorData,
      status: 0,
    });

    expect(createResponse.status()).toBe(201);

    const createdMentor = await createResponse.json();
    const mentorId = createdMentor.id;

    try {
      const response = await api.get(`api/Mentor/id?id=${mentorId}`);

      expect(response.status()).toBe(200);

      const mentor = await response.json();

      expect(mentor).toBeDefined();
      expect(mentor.id).toBe(mentorId);
      expect(mentor.firstName).toBe("Get");
      expect(mentor.lastName).toBe("Mentor");
      expect(mentor.email).toBe(mentorData.email);
      expect(mentor.expertise).toBe("C#");
      expect(mentor.status).toBe("Active");
    } finally {
      await cleanup.deleteMentor(mentorId);
    }
  });

  test("should update a mentor", async () => {
    const mentorData = TestDataFactory.mentor({
      firstName: "Before",
      lastName: "Update",
      expertise: "C#",
    });

    const createResponse = await api.post("api/Mentor", {
      ...mentorData,
      status: 0,
    });

    expect(createResponse.status()).toBe(201);

    const createdMentor = await createResponse.json();
    const mentorId = createdMentor.id;

    try {
      const updatedEmail = TestDataFactory.uniqueEmail("mentor.updated");

      const updateResponse = await api.put(`api/Mentor/id?id=${mentorId}`, {
        firstName: "After",
        lastName: "Update",
        email: updatedEmail,
        expertise: "TypeScript",
        status: 1,
      });

      expect(updateResponse.status()).toBe(200);

      const updatedMentor = await updateResponse.json();

      expect(updatedMentor.id).toBe(mentorId);
      expect(updatedMentor.firstName).toBe("After");
      expect(updatedMentor.lastName).toBe("Update");
      expect(updatedMentor.email).toBe(updatedEmail);
      expect(updatedMentor.expertise).toBe("TypeScript");
      expect(updatedMentor.status).toBe("Inactive");
    } finally {
      await cleanup.deleteMentor(mentorId);
    }
  });

  test("should delete a mentor", async () => {
    const mentorData = TestDataFactory.mentor({
      firstName: "Delete",
      lastName: "Mentor",
      expertise: "C#",
    });

    const createResponse = await api.post("api/Mentor", {
      ...mentorData,
      status: 1,
    });

    expect(createResponse.status()).toBe(201);

    const createdMentor = await createResponse.json();
    const mentorId = createdMentor.id;

    const deleteResponse = await api.delete(`api/Mentor/id?id=${mentorId}`);

    expect(deleteResponse.status()).toBe(204);

    const getResponse = await api.get(`api/Mentor/id?id=${mentorId}`);

    expect(getResponse.status()).toBe(404);
  });

  test("should return 404 for non-existing mentor", async () => {
    const response = await api.get("api/Mentor/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 404 when updating non-existing mentor", async () => {
    const response = await api.put("api/Mentor/999999999", {
      ...TestDataFactory.mentor(),
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
      email: TestDataFactory.invalidEmail(),
      expertise: "",
      status: TestDataFactory.invalidStatus(),
    });

    expect(response.status()).toBe(400);

    const body = await response.json();

    expect(body).toBeDefined();
  });

  test("should return 400 when first name is missing", async () => {
    const response = await api.post("api/Mentor", {
      lastName: "Mentor",
      email: TestDataFactory.uniqueEmail("mentor.missing"),
      expertise: "C#",
      status: 0,
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid email", async () => {
    const response = await api.post("api/Mentor", {
      firstName: "Invalid",
      lastName: "Email",
      email: TestDataFactory.invalidEmail(),
      expertise: "C#",
      status: 0,
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid mentor status", async () => {
    const response = await api.post("api/Mentor", {
      firstName: "Invalid",
      lastName: "Status",
      email: TestDataFactory.uniqueEmail("mentor.status"),
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
