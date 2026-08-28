import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { TestDataFactory } from "../utils/testDataFactory";
import { FileHelper } from "../utils/fileHelper";

test.describe("Submission File Upload API", () => {
  let api: ApiClient;

  test.beforeEach(async ({ request }) => {
    const token = await getAdminToken(request);

    api = new ApiClient(request, token);
  });

  test("should upload a file for a submission", async () => {
    // Create trainee
    const traineeResponse = await api.post("api/Trainee", {
      ...TestDataFactory.trainee({
        firstName: "Upload",
        lastName: "Trainee",
      }),
      status: 0,
    });

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    // Create mentor
    const mentorResponse = await api.post("api/Mentor", {
      ...TestDataFactory.mentor({
        firstName: "Upload",
        lastName: "Mentor",
      }),
      status: 0,
    });

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    // Create learning task
    const taskResponse = await api.post("api/LearningTask", {
      ...TestDataFactory.learningTask({
        title: `Upload Task ${Date.now()}`,
      }),
      status: 0,
    });

    expect(taskResponse.status()).toBe(201);

    const task = await taskResponse.json();

    // Create task assignment
    const assignmentResponse = await api.post(
      "api/TaskAssignment",
      TestDataFactory.taskAssignment(trainee.id, mentor.id, task.id),
    );

    expect(assignmentResponse.status()).toBe(201);

    const assignment = await assignmentResponse.json();

    // Create submission
    const submissionResponse = await api.post(
      "api/Submission",
      TestDataFactory.submission(assignment.id),
    );

    expect(submissionResponse.status()).toBe(201);

    const submission = await submissionResponse.json();

    // Create test file
    const filePath = FileHelper.createTestFile(
      "playwright-test.pdf",
      "File created for Playwright API testing.",
    );

    try {
      // Upload file
      const response = await api.uploadFile(
        `api/submissionfile/${submission.id}/files`,
        filePath,
      );

      expect(response.status()).toBe(202);

      const body = await response.json();

      // Verify response
      expect(body).toBeDefined();
      expect(body.trackingId).toBeDefined();
      expect(body.submissionId).toBe(submission.id);
      expect(body.status).toBe("Queued");
    } finally {
      FileHelper.deleteTestFile(filePath);
    }
  });

  test("should return 400 when submission does not exist", async () => {
    const filePath = FileHelper.createTestFile(
      "invalid-submission.pdf",
      "File created for invalid submission test.",
    );

    try {
      const response = await api.uploadFile(
        "api/submissionfile/999999999/files",
        filePath,
      );

      expect(response.status()).toBe(400);

      const body = await response.json();

      expect(body).toBeDefined();
      expect(body.message).toBe("Submission record not found.");
    } finally {
      FileHelper.deleteTestFile(filePath);
    }
  });

  test("should return 400 for an empty file", async () => {
    const filePath = FileHelper.createTestFile("empty.pdf", "");

    try {
      const response = await api.uploadFile(
        "api/submissionfile/999999999/files",
        filePath,
      );

      expect(response.status()).toBe(400);

      const body = await response.json();

      expect(body).toBeDefined();
      expect(body.message).toBe("File is missing or empty.");
    } finally {
      FileHelper.deleteTestFile(filePath);
    }
  });

  test("should return 400 for unsupported file extension", async () => {
    const filePath = FileHelper.createTestFile(
      "unsupported.txt",
      "Unsupported file type.",
    );

    try {
      const response = await api.uploadFile(
        "api/submissionfile/999999999/files",
        filePath,
      );

      expect(response.status()).toBe(400);

      const body = await response.json();

      expect(body).toBeDefined();
      expect(body.message).toBe("File type extention is not allowed");
    } finally {
      FileHelper.deleteTestFile(filePath);
    }
  });

  test("should return 401 without authentication", async ({ request }) => {
    const filePath = FileHelper.createTestFile(
      "unauthorized.pdf",
      "Unauthorized upload test.",
    );

    try {
      const response = await request.post(
        "api/submissionfile/999999999/files",
        {
          multipart: {
            file: {
              name: "unauthorized.pdf",
              mimeType: "text/plain",
              buffer: Buffer.from("Unauthorized upload test."),
            },
          },
        },
      );

      expect(response.status()).toBe(401);
    } finally {
      FileHelper.deleteTestFile(filePath);
    }
  });
});
