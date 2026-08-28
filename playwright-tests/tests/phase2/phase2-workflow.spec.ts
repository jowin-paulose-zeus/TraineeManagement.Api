import { test, expect } from "@playwright/test";
import { ApiClient } from "../utils/apiClient";
import { getAdminToken } from "../utils/authHelper";
import { TestDataFactory } from "../utils/testDataFactory";

test.describe("Complete Trainee Management Workflow", () => {
  test("should complete trainee to review workflow", async ({ request }) => {

    // 1. Login

    const token = await getAdminToken(request);

    expect(token).toBeDefined();
    expect(token.length).toBeGreaterThan(0);

    const api = new ApiClient(request, token);

    // 2. Create Trainee

    const traineeData = TestDataFactory.trainee({
      firstName: "E2E",
      lastName: "Trainee",
    });

    const traineeResponse = await api.post("api/Trainee", {
      ...traineeData,
      status: 0,
    });

    expect(traineeResponse.status()).toBe(201);

    const trainee = await traineeResponse.json();

    expect(trainee.id).toBeDefined();
    expect(trainee.firstName).toBe("E2E");
    expect(trainee.lastName).toBe("Trainee");

    // 3. Create Mentor   
    
    const mentorData = TestDataFactory.mentor({
      firstName: "E2E",
      lastName: "Mentor",
    });

    const mentorResponse = await api.post("api/Mentor", {
      ...mentorData,
      status: 0,
    });

    expect(mentorResponse.status()).toBe(201);

    const mentor = await mentorResponse.json();

    expect(mentor.id).toBeDefined();
    expect(mentor.firstName).toBe("E2E");
    expect(mentor.lastName).toBe("Mentor");

    // 4. Create Learning Task

    const taskData = TestDataFactory.learningTask({
      title: `E2E Learning Task ${Date.now()}`,
      description: "Learning task created for E2E testing.",
      expectedTechStack: "TypeScript",
    });

    const taskResponse = await api.post("api/LearningTask", {
      ...taskData,
      status: 0,
    });

    expect(taskResponse.status()).toBe(201);

    const task = await taskResponse.json();

    expect(task.id).toBeDefined();
    expect(task.title).toBe(taskData.title);
    expect(task.expectedTechStack).toBe("TypeScript");

    // 5. Create Task Assignment

    const assignmentData = TestDataFactory.taskAssignment(
      trainee.id,
      mentor.id,
      task.id,
    );

    const assignmentResponse = await api.post(
      "api/TaskAssignment",
      assignmentData,
    );

    expect(assignmentResponse.status()).toBe(201);

    const assignment = await assignmentResponse.json();

    expect(assignment.id).toBeDefined();
    expect(assignment.traineeId).toBe(trainee.id);
    expect(assignment.mentorId).toBe(mentor.id);
    expect(assignment.learningTaskId).toBe(task.id);
    expect(assignment.status).toBe("Assigned");

    // 6. Create Submission

    const submissionData = TestDataFactory.submission(assignment.id);

    const submissionResponse = await api.post("api/Submission", submissionData);

    expect(submissionResponse.status()).toBe(201);

    const submission = await submissionResponse.json();

    expect(submission.id).toBeDefined();
    expect(submission.taskAssignmentId).toBe(assignment.id);
    expect(submission.submissionUrl).toBe("https://test.url.com");
    expect(submission.status).toBe("Submitted");

    // 7. Create Review

    const reviewData = TestDataFactory.review(submission.id, mentor.id);

    const reviewResponse = await api.post("api/Review", reviewData);

    expect(reviewResponse.status()).toBe(201);

    const review = await reviewResponse.json();

    expect(review.id).toBeDefined();
    expect(review.submissionId).toBe(submission.id);
    expect(review.mentorName).toContain("E2E");
    expect(review.feedback).toBe(
      "Review created through Playwright API testing.",
    );
    expect(review.score).toBe(9);
    expect(review.status).toBe("Accepted");

    // 8. Verify Final Review
 
    const finalReviewResponse = await api.get(`api/Review/${review.id}`);

    expect(finalReviewResponse.status()).toBe(200);

    const finalReview = await finalReviewResponse.json();

    expect(finalReview.id).toBe(review.id);
    expect(finalReview.submissionId).toBe(submission.id);
    expect(finalReview.status).toBe("Accepted");
    expect(finalReview.score).toBe(9);
  });
});
