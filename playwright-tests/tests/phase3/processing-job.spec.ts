import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { TestDataFactory } from "../utils/testDataFactory";
import { FileHelper } from "../utils/fileHelper";

test.describe("Processing Job API", () => {
  let api: ApiClient;

  test.beforeEach(async ({ request }) => {
    const token = await getAdminToken(request);

    api = new ApiClient(request, token);
  });

  test("should process uploaded submission successfully", async () => {
    // ----------------------------------------
    // Create trainee
    // ----------------------------------------

    const traineeResponse = await api.post("api/Trainee", {
      ...TestDataFactory.trainee({
        firstName: "Processing",
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
        firstName: "Processing",
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
        title: `Processing Task ${Date.now()}`,
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
      TestDataFactory.taskAssignment(
        trainee.id,
        mentor.id,
        task.id,
      ),
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
    // Create test file
    // ----------------------------------------

    const filePath = FileHelper.createTestFile(
      "processing-test.pdf",
      "File created for processing test.",
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

      expect(uploadBody.trackingId).toBeDefined();

      expect(uploadBody.submissionId).toBe(
        submission.id,
      );

      expect(uploadBody.processingJobID).toBeDefined();

      expect(uploadBody.status).toBe("Queued");

      const processingJobId =
        uploadBody.processingJobID;

      // ----------------------------------------
      // Verify initial processing job
      // ----------------------------------------

      const initialJobResponse = await api.get(
        `api/processing-jobs/${processingJobId}`,
      );

      expect(initialJobResponse.status()).toBe(200);

      const initialJob =
        await initialJobResponse.json();

      expect(initialJob.id).toBe(
        processingJobId,
      );

      expect(initialJob.submissionId).toBe(
        submission.id,
      );

      expect(initialJob.correlationId).toBe(
        uploadBody.trackingId,
      );

      // ----------------------------------------
      // Wait for worker to complete processing
      // ----------------------------------------

      await expect
        .poll(
          async () => {
            const response = await api.get(
              `api/processing-jobs/${processingJobId}`,
            );

            expect(response.status()).toBe(200);

            const job = await response.json();

            return job.status;
          },
          {
            timeout: 30000,
            intervals: [500, 1000, 2000],
          },
        )

      // ----------------------------------------
      // Get final processing job
      // ----------------------------------------

      const finalJobResponse = await api.get(
        `api/processing-jobs/${processingJobId}`,
      );

      expect(finalJobResponse.status()).toBe(200);

      const finalJob =
        await finalJobResponse.json();

      // ----------------------------------------
      // Verify completed job
      // ----------------------------------------

      expect(finalJob.id).toBe(
        processingJobId,
      );

      expect(finalJob.submissionId).toBe(
        submission.id,
      );

      expect(finalJob.correlationId).toBe(
        uploadBody.trackingId,
      );

      expect(finalJob.startedAt).toBeDefined();

      expect(finalJob.completedAt).toBeDefined();

      expect(finalJob.errorSummary).toBeNull();
    } finally {
      // ----------------------------------------
      // Cleanup local test file
      // ----------------------------------------

      FileHelper.deleteTestFile(filePath);
    }
  });

  test("should return 404 for non-existing processing job", async () => {
    const response = await api.get(
      "api/processing-jobs/999999999",
    );

    expect(response.status()).toBe(404);
  });

  test("should return 401 without authentication", async ({
    request,
  }) => {
    const response = await request.get(
      "api/processing-jobs/999999999",
    );

    expect(response.status()).toBe(401);
  });
});