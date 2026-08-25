import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { CleanupHelper } from "../utils/cleanupHelper";
import { TestDataFactory } from "../utils/testDataFactory";

test.describe("Learning Task API", () => {
  let api: ApiClient;
  let cleanup: CleanupHelper;

  test.beforeEach(async ({ request }) => {
    const token = await getAdminToken(request);

    api = new ApiClient(request, token);
    cleanup = new CleanupHelper(api);
  });

  test("should get all learning tasks", async () => {
    const response = await api.get("api/LearningTask?pageNumber=1&pageSize=10");

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

  test("should create a learning task", async () => {
    const taskData = TestDataFactory.learningTask({
      title: `Playwright Task ${Date.now()}`,
      description: "Learning task created through Playwright testing.",
      expectedTechStack: "TypeScript",
    });

    const response = await api.post("api/LearningTask", {
      ...taskData,
      status: 0,
    });

    expect(response.status()).toBe(201);

    const task = await response.json();

    expect(task.id).toBeDefined();
    expect(task.title).toBe(taskData.title);
    expect(task.description).toBe(taskData.description);
    expect(task.expectedTechStack).toBe("TypeScript");
    expect(task.status).toBe("Draft");

    await cleanup.deleteLearningTask(task.id);
  });

  test("should get learning task by id", async () => {
    const taskData = TestDataFactory.learningTask({
      title: `Get Task ${Date.now()}`,
      expectedTechStack: "C#",
    });

    const createResponse = await api.post("api/LearningTask", {
      ...taskData,
      status: 0,
    });

    expect(createResponse.status()).toBe(201);

    const createdTask = await createResponse.json();
    const taskId = createdTask.id;

    try {
      const response = await api.get(`api/LearningTask/id?id=${taskId}`);

      expect(response.status()).toBe(200);

      const task = await response.json();

      expect(task).toBeDefined();
      expect(task.id).toBe(taskId);
      expect(task.title).toBe(taskData.title);
      expect(task.description).toBe(taskData.description);
      expect(task.expectedTechStack).toBe("C#");
      expect(task.status).toBe("Draft");
    } finally {
      await cleanup.deleteLearningTask(taskId);
    }
  });

  test("should update a learning task", async () => {
    const taskData = TestDataFactory.learningTask({
      title: `Before Update ${Date.now()}`,
      description: "Task before update.",
      expectedTechStack: "C#",
    });

    const createResponse = await api.post("api/LearningTask", {
      ...taskData,
      status: 0,
    });

    expect(createResponse.status()).toBe(201);

    const createdTask = await createResponse.json();
    const taskId = createdTask.id;

    try {
      const updateResponse = await api.put(`api/LearningTask/id?id=${taskId}`, {
        title: `After Update ${Date.now()}`,
        description: "Task after update.",
        expectedTechStack: "TypeScript",
        dueDate: TestDataFactory.futureDate(14),
        status: 1,
      });

      expect(updateResponse.status()).toBe(200);

      const updatedTask = await updateResponse.json();

      expect(updatedTask.id).toBe(taskId);
      expect(updatedTask.description).toBe("Task after update.");
      expect(updatedTask.expectedTechStack).toBe("TypeScript");
      expect(updatedTask.status).toBe("Published");
    } finally {
      await cleanup.deleteLearningTask(taskId);
    }
  });

  test("should delete a learning task", async () => {
    const taskData = TestDataFactory.learningTask({
      title: `Delete Task ${Date.now()}`,
    });

    const createResponse = await api.post("api/LearningTask", {
      ...taskData,
      status: 0,
    });

    expect(createResponse.status()).toBe(201);

    const createdTask = await createResponse.json();
    const taskId = createdTask.id;

    const deleteResponse = await api.delete(`api/LearningTask/id?id=${taskId}`);

    expect(deleteResponse.status()).toBe(204);

    const getResponse = await api.get(`api/LearningTask/id?id=${taskId}`);

    expect(getResponse.status()).toBe(404);
  });

  test("should return 404 for non-existing learning task", async () => {
    const response = await api.get("api/LearningTask/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 404 when updating non-existing learning task", async () => {
    const response = await api.put("api/LearningTask/999999999", {
      ...TestDataFactory.learningTask(),
      status: 0,
    });

    expect(response.status()).toBe(404);
  });

  test("should return 404 when deleting non-existing learning task", async () => {
    const response = await api.delete("api/LearningTask/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 400 for invalid learning task data", async () => {
    const response = await api.post("api/LearningTask", {
      title: "",
      description: "",
      expectedTechStack: "",
      dueDate: TestDataFactory.invalidDueDate(),
      status: TestDataFactory.invalidStatus(),
    });

    expect(response.status()).toBe(400);

    const body = await response.json();

    expect(body).toBeDefined();
  });

  test("should return 400 when title is missing", async () => {
    const response = await api.post("api/LearningTask", {
      description: "Learning task without title.",
      expectedTechStack: "C#",
      dueDate: TestDataFactory.futureDate(7),
      status: 0,
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid learning task status", async () => {
    const response = await api.post("api/LearningTask", {
      title: `Invalid Status ${Date.now()}`,
      description: "Testing invalid status.",
      expectedTechStack: "C#",
      dueDate: TestDataFactory.futureDate(7),
      status: 99,
    });

    expect(response.status()).toBe(400);
  });

  test("should return 401 without authentication", async ({ request }) => {
    const response = await request.get("api/LearningTask");

    expect(response.status()).toBe(401);
  });
});
