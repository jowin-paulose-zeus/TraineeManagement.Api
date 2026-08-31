import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { TestDataFactory } from "../utils/testDataFactory";
import { FileHelper } from "../utils/fileHelper";

test.describe("RabbitMQ Processing", () => {
  let api: ApiClient;

  test.beforeEach(async ({ request }) => {
    const token = await getAdminToken(request);

    api = new ApiClient(request, token);
  });

  test("should publish submission processing message after file upload", async () => {
    // Create trainee
    const traineeResponse = await api.post("api/Trainee", {
      ...TestDataFactory.trainee({
        firstName: "RabbitMQ",
        lastName: "Trainee",
      }),
      status: 0,
    });

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    // Create mentor
    const mentorResponse = await api.post("api/Mentor", {
      ...TestDataFactory.mentor({
        firstName: "RabbitMQ",
        lastName: "Mentor",
      }),
      status: 0,
    });

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    // Create learning task
    const taskResponse = await api.post("api/LearningTask", {
      ...TestDataFactory.learningTask({
        title: `RabbitMQ Task ${Date.now()}`,
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

    // Create file
    const filePath = FileHelper.createTestFile(
      "rabbitmq-test.pdf",
      "RabbitMQ Playwright test file.",
    );

    try {
      // Upload file
      const uploadResponse = await api.uploadFile(
        `api/submissionfile/${submission.id}/files`,
        filePath,
      );

      expect(uploadResponse.status()).toBe(202);

      const uploadBody = await uploadResponse.json();

      expect(uploadBody.trackingId).toBeDefined();
      expect(uploadBody.submissionId).toBe(submission.id);
      expect(uploadBody.processingJobID).toBeDefined();
      expect(uploadBody.status).toBe("Queued");

      // Verify processing job was created
      const jobResponse = await api.get(
        `api/processing-jobs/${uploadBody.processingJobID}`,
      );

      expect(jobResponse.status()).toBe(200);

      const job = await jobResponse.json();

      expect(job.status).toBe("Queued");
      expect(job.submissionId).toBe(submission.id);
      expect(job.correlationId).toBe(uploadBody.trackingId);
    } finally {
      FileHelper.deleteTestFile(filePath);
    }
  });

  test("should process RabbitMQ message successfully", async () => {
    // Create trainee
    const traineeResponse = await api.post("api/Trainee", {
      ...TestDataFactory.trainee({
        firstName: "RabbitMQProcess",
        lastName: "Trainee",
      }),
      status: 0,
    });

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    // Create mentor
    const mentorResponse = await api.post("api/Mentor", {
      ...TestDataFactory.mentor({
        firstName: "RabbitMQProcess",
        lastName: "Mentor",
      }),
      status: 0,
    });

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    // Create learning task
    const taskResponse = await api.post("api/LearningTask", {
      ...TestDataFactory.learningTask({
        title: `RabbitMQ Processing ${Date.now()}`,
      }),
      status: 0,
    });

    expect(taskResponse.status()).toBe(201);

    const task = await taskResponse.json();

    // Create assignment
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

    // Create file
    const filePath = FileHelper.createTestFile(
      "rabbitmq-processing.pdf",
      "RabbitMQ processing test.",
    );

    try {
      // Upload and publish RabbitMQ message
      const uploadResponse = await api.uploadFile(
        `api/submissionfile/${submission.id}/files`,
        filePath,
      );

      expect(uploadResponse.status()).toBe(202);

      const uploadBody = await uploadResponse.json();

      const processingJobId = uploadBody.processingJobID;

      expect(processingJobId).toBeDefined();

      // Wait for Worker to consume RabbitMQ message
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
        .toBe("Queued");

      // Get final job
      const finalResponse = await api.get(
        `api/processing-jobs/${processingJobId}`,
      );

      expect(finalResponse.status()).toBe(200);

      const finalJob = await finalResponse.json();

      expect(finalJob.status).toBe("Queued");

      expect(finalJob.submissionId).toBe(submission.id);

      expect(finalJob.messageId).toBeDefined();

      expect(finalJob.correlationId).toBe(uploadBody.trackingId);

      expect(finalJob.startedAt).toBeDefined();

      expect(finalJob.completedAt).toBeDefined();

      expect(finalJob.errorSummary).toBeNull();
    } finally {
      FileHelper.deleteTestFile(filePath);
    }
  });

  test("should process multiple RabbitMQ messages independently", async () => {
    const processingJobs: number[] = [];

    for (let i = 1; i <= 2; i++) {
      // Create trainee
      const traineeResponse = await api.post("api/Trainee", {
        ...TestDataFactory.trainee({
          firstName: `RabbitMQ${i}`,
          lastName: "Trainee",
        }),
        status: 0,
      });

      expect(traineeResponse.status()).toBe(201);

      const trainee = await traineeResponse.json();

      // Create mentor
      const mentorResponse = await api.post("api/Mentor", {
        ...TestDataFactory.mentor({
          firstName: `RabbitMQ${i}`,
          lastName: "Mentor",
        }),
        status: 0,
      });

      expect(mentorResponse.status()).toBe(201);

      const mentor = await mentorResponse.json();

      // Create learning task
      const taskResponse = await api.post("api/LearningTask", {
        ...TestDataFactory.learningTask({
          title: `RabbitMQ Multiple ${i} ${Date.now()}`,
        }),
        status: 0,
      });

      expect(taskResponse.status()).toBe(201);

      const task = await taskResponse.json();

      // Create assignment
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

      // Create file
      const filePath = FileHelper.createTestFile(
        `rabbitmq-multiple-${i}.pdf`,
        `RabbitMQ multiple message test ${i}.`,
      );

      try {
        const uploadResponse = await api.uploadFile(
          `api/submissionfile/${submission.id}/files`,
          filePath,
        );

        expect(uploadResponse.status()).toBe(202);

        const uploadBody = await uploadResponse.json();

        expect(uploadBody.processingJobID).toBeDefined();

        processingJobs.push(uploadBody.processingJobID);
      } finally {
        FileHelper.deleteTestFile(filePath);
      }
    }

    // Verify both messages are processed independently
    for (const processingJobID of processingJobs) {
      await expect
        .poll(
          async () => {
            const response = await api.get(
              `api/processing-jobs/${processingJobID}`,
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
        .toBe("Completed");

      const response = await api.get(`api/processing-jobs/${processingJobID}`);

      expect(response.status()).toBe(200);

      const job = await response.json();

      expect(job.status).toBe("Completed");
    }
  });

  test("should return 404 for non-existing processing job", async () => {
    const response = await api.get("api/processing-jobs/999999999");

    expect(response.status()).toBe(404);
  });
});
