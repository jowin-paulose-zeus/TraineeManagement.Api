import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { CleanupHelper } from "../utils/cleanupHelper";
import { TestDataFactory } from "../utils/testDataFactory";

test.describe("Submission API", () => {
  let api: ApiClient;
  let cleanup: CleanupHelper;

  test.beforeEach(async ({ request }) => {
    const token = await getAdminToken(request);

    api = new ApiClient(request, token);
    cleanup = new CleanupHelper(api);
  });

  test("should get all submissions", async () => {
    const response = await api.get("api/Submission");

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body).toBeDefined();
    expect(Array.isArray(body)).toBeTruthy();
  });

  test("should create a submission", async () => {
    const traineeData = TestDataFactory.trainee({
      firstName: "Submission",
      lastName: "Trainee",
    });

    const mentorData = TestDataFactory.mentor({
      firstName: "Submission",
      lastName: "Mentor",
    });

    const taskData = TestDataFactory.learningTask({
      title: `Submission Task ${Date.now()}`,
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

    let assignmentId: number | undefined;

    try {
      const assignmentResponse = await api.post(
        "api/TaskAssignment",
        TestDataFactory.taskAssignment(
          trainee.id,
          mentor.id,
          task.id,
        ),
      );

      expect(assignmentResponse.status()).toBe(201);

      const assignment = await assignmentResponse.json();

      assignmentId = assignment.id;

      const submissionData = TestDataFactory.submission(
        assignment.id,
      );

      const response = await api.post(
        "api/Submission",
        submissionData,
      );

      expect(response.status()).toBe(201);

      const submission = await response.json();

      expect(submission.id).toBeDefined();
      expect(submission.taskAssignmentId).toBe(
        assignment.id,
      );

      expect(submission.traineeName).toContain(
        "Submission",
      );

      expect(submission.taskTitle).toBe(task.title);

      expect(submission.submissionUrl).toBe(
        "https://test.url.com",
      );

      expect(submission.status).toBe("Submitted");

      expect(submission.notes).toBe(
        "Submission created through Playwright API testing.",
      );

      expect(submission.submissionDate).toBeDefined();
    } finally {
      /*
       * TaskAssignment does not expose DELETE.
       * Therefore the assignment and its related entities
       * are not cleaned up here.
       */
    }
  });

  test("should get submission by id", async () => {
    const traineeData = TestDataFactory.trainee({
      firstName: "Get",
      lastName: "Submission",
    });

    const mentorData = TestDataFactory.mentor({
      firstName: "Get",
      lastName: "Mentor",
    });

    const taskData = TestDataFactory.learningTask({
      title: `Get Submission Task ${Date.now()}`,
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

    try {
      const assignmentResponse = await api.post(
        "api/TaskAssignment",
        TestDataFactory.taskAssignment(
          trainee.id,
          mentor.id,
          task.id,
        ),
      );

      expect(assignmentResponse.status()).toBe(201);

      const assignment = await assignmentResponse.json();

      const submissionResponse = await api.post(
        "api/Submission",
        TestDataFactory.submission(
          assignment.id,
        ),
      );

      expect(submissionResponse.status()).toBe(201);

      const createdSubmission =
        await submissionResponse.json();

      const submissionId = createdSubmission.id;

      const response = await api.get(
        `api/Submission/${submissionId}`,
      );

      expect(response.status()).toBe(200);

      const submission = await response.json();

      expect(submission).toBeDefined();
      expect(submission.id).toBe(submissionId);
      expect(submission.taskAssignmentId).toBe(
        assignment.id,
      );

      expect(submission.taskTitle).toBe(
        task.title,
      );

      expect(submission.submissionUrl).toBe(
        "https://test.url.com",
      );

      expect(submission.status).toBe("Submitted");

      expect(submission.notes).toBe(
        "Submission created through Playwright API testing.",
      );

      expect(submission.submissionDate).toBeDefined();
    } finally {
      /*
       * No DELETE endpoint for TaskAssignment/Submission.
       */
    }
  });

  test("should return 404 for non-existing submission", async () => {
    const response = await api.get(
      "api/Submission/999999999",
    );

    expect(response.status()).toBe(404);
  });

  test("should return 400 for invalid task assignment id", async () => {
    const response = await api.post(
      "api/Submission",
      TestDataFactory.submission(999999999),
    );

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid submission data", async () => {
    const response = await api.post("api/Submission", {
      taskAssignmentId: 999999999,
      submissionUrl: "",
      notes: "",
      status: 99,
    });

    expect(response.status()).toBe(400);

    const body = await response.json();

    expect(body).toBeDefined();
  });

  test("should return 400 when submission URL is empty", async () => {
    const response = await api.post("api/Submission", {
      taskAssignmentId: 999999999,
      submissionUrl: "",
      notes: "Invalid submission URL.",
      status: 0,
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid submission status", async () => {
    const response = await api.post("api/Submission", {
      taskAssignmentId: 999999999,
      submissionUrl: "https://test.url.com",
      notes: "Invalid status test.",
      status: 99,
    });

    expect(response.status()).toBe(400);
  });

  test("should return 401 without authentication", async ({ request }) => {
    const response = await request.get("api/Submission");

    expect(response.status()).toBe(401);
  });
});