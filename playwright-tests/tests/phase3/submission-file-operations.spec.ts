import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { TestDataFactory } from "../utils/testDataFactory";
import { FileHelper } from "../utils/fileHelper";

test.describe("Submission File Operations API", () => {
  let api: ApiClient;

  test.beforeEach(async ({ request }) => {
    const token = await getAdminToken(request);

    api = new ApiClient(request, token);
  });

  test("should download an uploaded submission file", async () => {
    // ----------------------------------------
    // Create trainee
    // ----------------------------------------

    const traineeResponse = await api.post("api/Trainee", {
      ...TestDataFactory.trainee({
        firstName: "Download",
        lastName: "Trainee",
      }),
      status: 0,
    });

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    // ----------------------------------------
    // Create mentor
    // ----------------------------------------

    const mentorResponse = await api.post("api/Mentor", {
      ...TestDataFactory.mentor({
        firstName: "Download",
        lastName: "Mentor",
      }),
      status: 0,
    });

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    // ----------------------------------------
    // Create learning task
    // ----------------------------------------

    const taskResponse = await api.post("api/LearningTask", {
      ...TestDataFactory.learningTask({
        title: `Download Task ${Date.now()}`,
      }),
      status: 0,
    });

    expect(taskResponse.status()).toBe(201);

    const task = await taskResponse.json();

    // ----------------------------------------
    // Create task assignment
    // ----------------------------------------

    const assignmentResponse = await api.post(
      "api/TaskAssignment",
      TestDataFactory.taskAssignment(trainee.id, mentor.id, task.id),
    );

    expect(assignmentResponse.status()).toBe(201);

    const assignment = await assignmentResponse.json();

    // ----------------------------------------
    // Create submission
    // ----------------------------------------

    const submissionResponse = await api.post(
      "api/Submission",
      TestDataFactory.submission(assignment.id),
    );

    expect(submissionResponse.status()).toBe(201);

    const submission = await submissionResponse.json();

    // ----------------------------------------
    // Create file
    // ----------------------------------------

    const fileContent = "Playwright download test file.";

    const filePath = FileHelper.createTestFile(
      "download-test.pdf",
      fileContent,
    );

    try {
      // ----------------------------------------
      // Upload file
      // ----------------------------------------

      const uploadResponse = await api.uploadFile(
        `api/submissionfile/${submission.id}/files`,
        filePath,
      );

      expect(uploadResponse.status()).toBe(202);

      const uploadBody = await uploadResponse.json();

      expect(uploadBody.id).toBeDefined();
      expect(uploadBody.submissionId).toBe(submission.id);

      const submissionFileId = uploadBody.id;

      // ----------------------------------------
      // Download file
      // ----------------------------------------

      const downloadResponse = await api.get(
        `api/submissionfile/${submissionFileId}/download`,
      );

      expect(downloadResponse.status()).toBe(200);

      // ----------------------------------------
      // Verify response headers
      // ----------------------------------------

      expect(downloadResponse.headers()["content-type"]).toContain(
        "text/plain",
      );

      expect(downloadResponse.headers()["content-disposition"]).toBeDefined();

      // ----------------------------------------
      // Verify file content
      // ----------------------------------------

      const downloadedContent = await downloadResponse.body();

      expect(downloadedContent.toString()).toBe(fileContent);
    } finally {
      FileHelper.deleteTestFile(filePath);
    }
  });

  test("should delete an uploaded submission file", async () => {
    // ----------------------------------------
    // Create trainee
    // ----------------------------------------

    const traineeResponse = await api.post("api/Trainee", {
      ...TestDataFactory.trainee({
        firstName: "DeleteFile",
        lastName: "Trainee",
      }),
      status: 0,
    });

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    // ----------------------------------------
    // Create mentor
    // ----------------------------------------

    const mentorResponse = await api.post("api/Mentor", {
      ...TestDataFactory.mentor({
        firstName: "DeleteFile",
        lastName: "Mentor",
      }),
      status: 0,
    });

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    // ----------------------------------------
    // Create learning task
    // ----------------------------------------

    const taskResponse = await api.post("api/LearningTask", {
      ...TestDataFactory.learningTask({
        title: `Delete File Task ${Date.now()}`,
      }),
      status: 0,
    });

    expect(taskResponse.status()).toBe(201);

    const task = await taskResponse.json();

    // ----------------------------------------
    // Create task assignment
    // ----------------------------------------

    const assignmentResponse = await api.post(
      "api/TaskAssignment",
      TestDataFactory.taskAssignment(trainee.id, mentor.id, task.id),
    );

    expect(assignmentResponse.status()).toBe(201);

    const assignment = await assignmentResponse.json();

    // ----------------------------------------
    // Create submission
    // ----------------------------------------

    const submissionResponse = await api.post(
      "api/Submission",
      TestDataFactory.submission(assignment.id),
    );

    expect(submissionResponse.status()).toBe(201);

    const submission = await submissionResponse.json();

    // ----------------------------------------
    // Create file
    // ----------------------------------------

    const filePath = FileHelper.createTestFile(
      "delete-test.pdf",
      "Playwright delete test file.",
    );

    try {
      // ----------------------------------------
      // Upload file
      // ----------------------------------------

      const uploadResponse = await api.uploadFile(
        `api/submissionfile/${submission.id}/files`,
        filePath,
      );

      expect(uploadResponse.status()).toBe(202);

      const uploadBody = await uploadResponse.json();

      expect(uploadBody.id).toBeDefined();

      const submissionFileId = uploadBody.id;

      // ----------------------------------------
      // Delete file
      // ----------------------------------------

      const deleteResponse = await api.delete(
        `api/submissionfile/${submissionFileId}`,
      );

      expect(deleteResponse.status()).toBe(204);

      // ----------------------------------------
      // Verify file no longer exists
      // ----------------------------------------

      const downloadResponse = await api.get(
        `api/submissionfile/${submissionFileId}/download`,
      );

      expect(downloadResponse.status()).toBe(404);
    } finally {
      FileHelper.deleteTestFile(filePath);
    }
  });

  test("should return 404 when downloading non-existing file", async () => {
    const response = await api.get("api/submissionfile/999999999/download");

    expect(response.status()).toBe(404);
  });

  test("should return 404 when deleting non-existing file", async () => {
    const response = await api.delete("api/submissionfile/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 401 when downloading without authentication", async ({
    request,
  }) => {
    const response = await request.get("api/submissionfile/999999999/download");

    expect(response.status()).toBe(401);
  });
});
