import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { CleanupHelper } from "../utils/cleanupHelper";
import { TestDataFactory } from "../utils/testDataFactory";

test.describe("Trainee API", () => {
  let api: ApiClient;
  let cleanup: CleanupHelper;

  test.beforeEach(async ({ request }) => {
    const token = await getAdminToken(request);

    api = new ApiClient(request, token);
    cleanup = new CleanupHelper(api);
  });

  test("should get all trainees", async () => {
    const response = await api.get("api/Trainee?pageNumber=1&pageSize=10");

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body).toBeDefined();
    expect(body).toHaveProperty("pageNumber");
    expect(body).toHaveProperty("pageSize");
    expect(body).toHaveProperty("totalRecords");
    expect(body).toHaveProperty("data");

    expect(Array.isArray(body.data)).toBeTruthy();
  });

  test("should get trainee by id", async () => {
    const traineeData = TestDataFactory.trainee({
      firstName: "Playwright",
      lastName: "Test",
      techStack: "TypeScript",
    });

    const createResponse = await api.post("api/Trainee", traineeData);

    expect(createResponse.status()).toBe(201);

    const createdTrainee = await createResponse.json();
    const traineeId = createdTrainee.id;

    try {
      const response = await api.get(`api/Trainee/id?id=${traineeId}`);

      expect(response.status()).toBe(200);

      const trainee = await response.json();

      expect(trainee).toBeDefined();
      expect(trainee.id).toBe(traineeId);
      expect(trainee.firstName).toBe("Playwright");
      expect(trainee.lastName).toBe("Test");
      expect(trainee.techStack).toBe("TypeScript");
      expect(trainee.status).toBe("Active");
    } finally {
      await cleanup.deleteTrainee(traineeId);
    }
  });

  test("should create a trainee", async () => {
    const traineeData = TestDataFactory.trainee({
      firstName: "Playwright",
      lastName: "Create",
    });

    const response = await api.post("api/Trainee", traineeData);

    expect(response.status()).toBe(201);

    const trainee = await response.json();

    expect(trainee.id).toBeDefined();
    expect(trainee.firstName).toBe("Playwright");
    expect(trainee.lastName).toBe("Create");
    expect(trainee.email).toBe(traineeData.email);
    expect(trainee.techStack).toBe("TypeScript");
    expect(trainee.status).toBe("Active");

    await cleanup.deleteTrainee(trainee.id);
  });

  test("should update a trainee", async () => {
    const traineeData = TestDataFactory.trainee({
      firstName: "Before",
      lastName: "Update",
      techStack: "C#",
    });

    const createResponse = await api.post("api/Trainee", traineeData);

    expect(createResponse.status()).toBe(201);

    const createdTrainee = await createResponse.json();
    const traineeId = createdTrainee.id;

    try {
      const updatedEmail = TestDataFactory.uniqueEmail("trainee.updated");

      const updateResponse = await api.put(`api/Trainee/id?id=${traineeId}`, {
        firstName: "After",
        lastName: "Update",
        email: updatedEmail,
        techStack: "TypeScript",
        status: 1,
      });

      expect(updateResponse.status()).toBe(200);

      const updatedTrainee = await updateResponse.json();

      expect(updatedTrainee.id).toBe(traineeId);
      expect(updatedTrainee.firstName).toBe("After");
      expect(updatedTrainee.lastName).toBe("Update");
      expect(updatedTrainee.techStack).toBe("TypeScript");
      expect(updatedTrainee.status).toBe("Inactive");
    } finally {
      await cleanup.deleteTrainee(traineeId);
    }
  });

  test("should delete a trainee", async () => {
    const traineeData = TestDataFactory.trainee({
      firstName: "Delete",
      lastName: "Test",
      status: "Inactive",
    });

    const createResponse = await api.post("api/Trainee", {
      ...traineeData,
      status: 1,
    });

    expect(createResponse.status()).toBe(201);

    const createdTrainee = await createResponse.json();
    const traineeId = createdTrainee.id;

    const deleteResponse = await api.delete(`api/Trainee/id?id=${traineeId}`);

    expect(deleteResponse.status()).toBe(204);

    const getResponse = await api.get(`api/Trainee/id?id=${traineeId}`);

    expect(getResponse.status()).toBe(404);
  });

  test("should return 404 for non-existing trainee", async () => {
    const response = await api.get("api/Trainee/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 404 when updating non-existing trainee", async () => {
    const response = await api.put("api/Trainee/999999999", {
      ...TestDataFactory.trainee(),
      status: 0,
    });

    expect(response.status()).toBe(404);
  });

  test("should return 404 when deleting non-existing trainee", async () => {
    const response = await api.delete("api/Trainee/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 400 for invalid trainee data", async () => {
    const response = await api.post("api/Trainee", {
      firstName: "",
      lastName: "",
      email: TestDataFactory.invalidEmail(),
      techStack: "",
      status: TestDataFactory.invalidStatus(),
    });

    expect(response.status()).toBe(400);

    const body = await response.json();

    expect(body).toBeDefined();
  });

  test("should return 400 when first name is missing", async () => {
    const response = await api.post("api/Trainee", {
      lastName: "Test",
      email: TestDataFactory.uniqueEmail("missing"),
      techStack: "Dotnet",
      status: 0,
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid email", async () => {
    const response = await api.post("api/Trainee", {
      firstName: "Invalid",
      lastName: "Email",
      email: TestDataFactory.invalidEmail(),
      techStack: "Dotnet",
      status: 0,
    });

    expect(response.status()).toBe(400);
  });

  test("should search trainees", async () => {
    const uniqueName = `Search${Date.now()}`;

    const traineeData = TestDataFactory.trainee({
      firstName: uniqueName,
      lastName: "Trainee",
      email: `${uniqueName.toLowerCase()}@test.com`,
    });

    const createResponse = await api.post("api/Trainee", traineeData);

    expect(createResponse.status()).toBe(201);

    const createdTrainee = await createResponse.json();
    const traineeId = createdTrainee.id;

    try {
      const response = await api.get(
        `api/Trainee?pageNumber=1&pageSize=10&search=${uniqueName}`,
      );

      expect(response.status()).toBe(200);

      const body = await response.json();

      expect(body.data).toBeDefined();
      expect(Array.isArray(body.data)).toBeTruthy();

      expect(
        body.data.some(
          (trainee: { firstName: string }) => trainee.firstName === uniqueName,
        ),
      ).toBeTruthy();
    } finally {
      await cleanup.deleteTrainee(traineeId);
    }
  });

  test("should return 401 without authentication", async ({ request }) => {
    const response = await request.get("api/Trainee");

    expect(response.status()).toBe(401);
  });
});
