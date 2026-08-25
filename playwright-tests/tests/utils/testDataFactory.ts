export class TestDataFactory {
  public static uniqueEmail(prefix: string = "playwright"): string {
    return `${prefix}.${Date.now()}@test.com`;
  }

  public static futureDate(daysFromNow: number = 7): string {
    const date = new Date();

    date.setDate(date.getDate() + daysFromNow);

    return date.toISOString();
  }

  public static pastDate(daysAgo: number = 1): string {
    const date = new Date();

    date.setDate(date.getDate() - daysAgo);

    return date.toISOString();
  }

  public static today(): string {
    return new Date().toISOString();
  }

  public static trainee(overrides?: {
    firstName?: string;
    lastName?: string;
    email?: string;
    techStack?: string;
    status?: string;
  }) {
    return {
      firstName: overrides?.firstName ?? "Playwright",
      lastName: overrides?.lastName ?? "Test",
      email: overrides?.email ?? this.uniqueEmail("trainee"),
      techStack: overrides?.techStack ?? "TypeScript",
      status: overrides?.status ?? 0,
    };
  }

  public static mentor(overrides?: {
    firstName?: string;
    lastName?: string;
    email?: string;
    expertise?: string;
    status?: string;
  }) {
    return {
      firstName: overrides?.firstName ?? "Playwright",
      lastName: overrides?.lastName ?? "Mentor",
      email: overrides?.email ?? this.uniqueEmail("mentor"),
      expertise: overrides?.expertise ?? "TypeScript",
      status: overrides?.status ?? 0,
    };
  }

  public static learningTask(overrides?: {
    title?: string;
    description?: string;
    expectedTechStack?: string;
    dueDate?: string;
    status?: string;
  }) {
    return {
      title: overrides?.title ?? `Playwright Learning Task ${this.today()}`,
      description:
        overrides?.description ?? "Learning task created for API testing.",
      expectedTechStack: overrides?.expectedTechStack ?? "TypeScript",
      dueDate: overrides?.dueDate ?? this.futureDate(7),
      status: overrides?.status ?? 0,
    };
  }

  public static taskAssignment(
    traineeId: number,
    mentorId: number,
    learningTaskId: number,
  ) {
    const assignedDate = new Date();
    const dueDate = new Date(assignedDate);

    dueDate.setDate(dueDate.getDate() + 7);

    return {
      traineeId,
      mentorId,
      learningTaskId,
      assignedDate: assignedDate.toISOString(),
      dueDate: dueDate.toISOString(),
      status: 0,
      remarks: "Created by Playwright API testing.",
    };
  }

  public static taskAssignmentUpdate() {
    return {
      status: 1,
    };
  }
  
  public static submission(taskAssignmentId: number) {
    return {
      taskAssignmentId,
      submissionUrl: "https://test.url.com",
      notes: "Submission created through Playwright API testing.",
      status: 0,
    };
  }

  public static review(submissionId: number, mentorId: number) {
    return {
      submissionId,
      mentorId,
      feedback: "Review created through Playwright API testing.",
      reviewStatus: 0,
    };
  }

  public static invalidEmail(): string {
    return "invalid-email";
  }

  public static invalidStatus(): string {
    return "invalid-status";
  }

  public static invalidDueDate(): string {
    return this.pastDate(1);
  }
}
