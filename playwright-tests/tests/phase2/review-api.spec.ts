import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { CleanupHelper } from "../utils/cleanupHelper";
import { TestDataFactory } from "../utils/testDataFactory";

test.describe("Review API", () => {
  let api: ApiClient;
  let cleanup: CleanupHelper;

  test.beforeEach(async ({ request }) => {
    const token = await getAdminToken(request);

    api = new ApiClient(request, token);
    cleanup = new CleanupHelper(api);
  });

  test("should get all reviews", async () => {
    const response = await api.get("api/Review");

    expect(response.status()).toBe(200);

    const body = await response.json();

    expect(body).toBeDefined();
    expect(Array.isArray(body)).toBeTruthy();
  });

  test("should create a review", async () => {
    const traineeData = TestDataFactory.trainee({
      firstName: "Review",
      lastName: "Trainee",
    });

    const mentorData = TestDataFactory.mentor({
      firstName: "Review",
      lastName: "Mentor",
    });

    const taskData = TestDataFactory.learningTask({
      title: `Review Task ${Date.now()}`,
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
        TestDataFactory.taskAssignment(trainee.id, mentor.id, task.id),
      );

      expect(assignmentResponse.status()).toBe(201);

      const assignment = await assignmentResponse.json();

      const submissionResponse = await api.post(
        "api/Submission",
        TestDataFactory.submission(assignment.id),
      );

      expect(submissionResponse.status()).toBe(201);

      const submission = await submissionResponse.json();

      const reviewData = TestDataFactory.review(submission.id, mentor.id);

      const response = await api.post("api/Review", reviewData);

      expect(response.status()).toBe(201);

      const review = await response.json();

      expect(review.id).toBeDefined();
      expect(review.submissionId).toBe(submission.id);
      expect(review.mentorName).toContain("Review");
      expect(review.feedback).toBe(
        "Review created through Playwright API testing.",
      );
      expect(review.score).toBe(9);
      expect(review.status).toBe("Accepted");
      expect(review.reviewedDate).toBeDefined();
    } finally {
      /*
       * Review, Submission and TaskAssignment do not
       * expose DELETE endpoints.
       */
    }
  });

  test("should get review by id", async () => {
    const traineeData = TestDataFactory.trainee({
      firstName: "GetReview",
      lastName: "Trainee",
    });

    const mentorData = TestDataFactory.mentor({
      firstName: "GetReview",
      lastName: "Mentor",
    });

    const taskData = TestDataFactory.learningTask({
      title: `Get Review Task ${Date.now()}`,
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
        TestDataFactory.taskAssignment(trainee.id, mentor.id, task.id),
      );

      expect(assignmentResponse.status()).toBe(201);

      const assignment = await assignmentResponse.json();

      const submissionResponse = await api.post(
        "api/Submission",
        TestDataFactory.submission(assignment.id),
      );

      expect(submissionResponse.status()).toBe(201);

      const submission = await submissionResponse.json();

      const reviewResponse = await api.post(
        "api/Review",
        TestDataFactory.review(submission.id, mentor.id),
      );

      expect(reviewResponse.status()).toBe(201);

      const createdReview = await reviewResponse.json();

      const response = await api.get(`api/Review/${createdReview.id}`);

      expect(response.status()).toBe(200);

      const review = await response.json();

      expect(review).toBeDefined();
      expect(review.id).toBe(createdReview.id);
      expect(review.submissionId).toBe(submission.id);
      expect(review.mentorName).toContain("GetReview");
      expect(review.feedback).toBe(
        "Review created through Playwright API testing.",
      );
      expect(review.score).toBe(9);
      expect(review.status).toBe("Accepted");
      expect(review.reviewedDate).toBeDefined();
    } finally {
      /*
       * No DELETE endpoints for these resources.
       */
    }
  });

  test("should return 404 for non-existing review", async () => {
    const response = await api.get("api/Review/999999999");

    expect(response.status()).toBe(404);
  });

  test("should return 400 for invalid submission id", async () => {
    const mentorData = TestDataFactory.mentor();

    const mentorResponse = await api.post("api/Mentor", {
      ...mentorData,
      status: 0,
    });

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    try {
      const response = await api.post("api/Review", {
        submissionId: 999999999,
        mentorId: mentor.id,
        feedback: "Invalid submission test.",
        score: 5,
        status: 0,
      });

      expect(response.status()).toBe(400);
    } finally {
      await cleanup.deleteMentor(mentor.id);
    }
  });

  test("should return 400 for invalid mentor id", async () => {
    const response = await api.post("api/Review", {
      submissionId: 999999999,
      mentorId: 999999999,
      feedback: "Invalid mentor test.",
      score: 5,
      status: 0,
    });

    expect(response.status()).toBe(400);
  });

  test("should return 400 for invalid review status", async () => {
    const response = await api.post("api/Review", {
      submissionId: 999999999,
      mentorId: 999999999,
      feedback: "Invalid status test.",
      score: 5,
      status: 99,
    });

    expect(response.status()).toBe(400);
  });

  test("should return 401 without authentication", async ({ request }) => {
    const response = await request.get("api/Review");

    expect(response.status()).toBe(401);
  });
});
