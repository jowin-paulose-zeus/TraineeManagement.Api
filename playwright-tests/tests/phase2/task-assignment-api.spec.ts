import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { CleanupHelper } from "../utils/cleanupHelper";
import { TestDataFactory } from "../utils/testDataFactory";

test.describe("Task Assignment API", () => {
  let api: ApiClient;
  let cleanup: CleanupHelper;

  test.beforeEach(async ({ request }) => {
    const token = await getAdminToken(request);

    api = new ApiClient(request, token);
    cleanup = new CleanupHelper(api);
  });

  test("should get all task assignments", async () => {
    const response = await api.get("api/TaskAssignment");

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body).toBeDefined();
    expect(Array.isArray(body)).toBeTruthy();
  });

  test("should create a task assignment", async () => {
    const traineeData = TestDataFactory.trainee({
      firstName: "Assignment",
      lastName: "Trainee",
    });

    const mentorData = TestDataFactory.mentor({
      firstName: "Assignment",
      lastName: "Mentor",
    });

    const taskData = TestDataFactory.learningTask({
      title: `Assignment Task ${Date.now()}`,
    });

    const traineeResponse = await api.post("api/Trainee", {
      ...traineeData,
      status: 0,
    });

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    const mentorResponse = await api.post("api/Mentor", {
      ...mentorData,
      status: 0,
    });

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    const taskResponse = await api.post("api/LearningTask", {
      ...taskData,
      status: 0,
    });

    expect(taskResponse.status()).toBe(201);

    const task = await taskResponse.json();

    const assignmentData = TestDataFactory.taskAssignment(
      trainee.id,
      mentor.id,
      task.id,
    );

    const response = await api.post("api/TaskAssignment", assignmentData);

    expect(response.status()).toBe(201);

    const assignment = await response.json();

    expect(assignment.id).toBeDefined();
    expect(assignment.traineeId).toBe(trainee.id);
    expect(assignment.mentorId).toBe(mentor.id);
    expect(assignment.learningTaskId).toBe(task.id);

    expect(assignment.traineeName).toContain("Assignment");
    expect(assignment.mentorName).toContain("Assignment");
    expect(assignment.learningTaskTitle).toBe(task.title);

    expect(assignment.status).toBe("Assigned");

    expect(assignment.remarks).toBe("Created by Playwright API testing.");
  });

  test("should update task assignment status", async () => {
    const traineeData = TestDataFactory.trainee({
      firstName: "Update",
      lastName: "Trainee",
    });

    const mentorData = TestDataFactory.mentor({
      firstName: "Update",
      lastName: "Mentor",
    });

    const taskData = TestDataFactory.learningTask({
      title: `Update Assignment ${Date.now()}`,
    });

    const traineeResponse = await api.post("api/Trainee", {
      ...traineeData,
      status: 0,
    });

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    const mentorResponse = await api.post("api/Mentor", {
      ...mentorData,
      status: 0,
    });

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    const taskResponse = await api.post("api/LearningTask", {
      ...taskData,
      status: 0,
    });

    expect(taskResponse.status()).toBe(201);

    const task = await taskResponse.json();

    const assignmentResponse = await api.post(
      "api/TaskAssignment",
      TestDataFactory.taskAssignment(trainee.id, mentor.id, task.id),
    );

    expect(assignmentResponse.status()).toBe(201);

    const assignment = await assignmentResponse.json();

    const updateResponse = await api.put(
      `api/TaskAssignment/${assignment.id}`,
      TestDataFactory.taskAssignmentUpdate(trainee.id, mentor.id, task.id),
    );

    expect(updateResponse.status()).toBe(200);

    const updatedAssignment = await updateResponse.json();

    expect(updatedAssignment.id).toBe(assignment.id);
    expect(updatedAssignment.status).toBe("InProgress");
  });

  test("should return 400 for invalid trainee id", async () => {
    const mentorData = TestDataFactory.mentor();
    const taskData = TestDataFactory.learningTask();

    const mentorResponse = await api.post("api/Mentor", {
      ...mentorData,
      status: 0,
    });

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    const taskResponse = await api.post("api/LearningTask", {
      ...taskData,
      status: 0,
    });

    expect(taskResponse.status()).toBe(201);

    const task = await taskResponse.json();

    try {
      const response = await api.post(
        "api/TaskAssignment",
        TestDataFactory.taskAssignment(999999999, mentor.id, task.id),
      );

      expect(response.status()).toBe(400);
    } finally {
      await cleanup.deleteMentor(mentor.id);
      await cleanup.deleteLearningTask(task.id);
    }
  });

  test("should return 400 for invalid mentor id", async () => {
    const traineeData = TestDataFactory.trainee();
    const taskData = TestDataFactory.learningTask();

    const traineeResponse = await api.post("api/Trainee", {
      ...traineeData,
      status: 0,
    });

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    const taskResponse = await api.post("api/LearningTask", {
      ...taskData,
      status: 0,
    });

    expect(taskResponse.status()).toBe(201);

    const task = await taskResponse.json();

    try {
      const response = await api.post(
        "api/TaskAssignment",
        TestDataFactory.taskAssignment(trainee.id, 999999999, task.id),
      );

      expect(response.status()).toBe(400);
    } finally {
      await cleanup.deleteTrainee(trainee.id);
      await cleanup.deleteLearningTask(task.id);
    }
  });

  test("should return 400 for invalid learning task id", async () => {
    const traineeData = TestDataFactory.trainee();
    const mentorData = TestDataFactory.mentor();

    const traineeResponse = await api.post("api/Trainee", {
      ...traineeData,
      status: 0,
    });

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    const mentorResponse = await api.post("api/Mentor", {
      ...mentorData,
      status: 0,
    });

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    try {
      const response = await api.post(
        "api/TaskAssignment",
        TestDataFactory.taskAssignment(trainee.id, mentor.id, 999999999),
      );

      expect(response.status()).toBe(400);
    } finally {
      await cleanup.deleteTrainee(trainee.id);
      await cleanup.deleteMentor(mentor.id);
    }
  });

  test("should return 401 without authentication", async ({ request }) => {
    const response = await request.get("api/TaskAssignment");

    expect(response.status()).toBe(401);
  });
});
