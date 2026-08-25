import { ApiClient } from "./apiClient";

export class CleanupHelper {
  private readonly api: ApiClient;

  constructor(api: ApiClient) {
    this.api = api;
  }

  public async deleteTrainee(traineeId: number): Promise<void> {
    const response = await this.api.delete(`api/Trainee/id?id=${traineeId}`);

    if (response.status() !== 204 && response.status() !== 404) {
      throw new Error(
        `Failed to cleanup trainee ${traineeId}. Status: ${response.status()}`,
      );
    }
  }

  public async deleteMentor(mentorId: number): Promise<void> {
    const response = await this.api.delete(`api/Mentor/id?id=${mentorId}`);

    if (response.status() !== 204 && response.status() !== 404) {
      throw new Error(
        `Failed to cleanup mentor ${mentorId}. Status: ${response.status()}`,
      );
    }
  }

  public async deleteLearningTask(taskId: number): Promise<void> {
    const response = await this.api.delete(`api/LearningTask/id?id=${taskId}`);

    if (response.status() !== 204 && response.status() !== 404) {
      throw new Error(
        `Failed to cleanup learning task ${taskId}. Status: ${response.status()}`,
      );
    }
  }

  public async deleteTaskAssignment(assignmentId: number): Promise<void> {
    const response = await this.api.delete(
      `api/TaskAssignment/id?id=${assignmentId}`,
    );

    if (response.status() !== 204 && response.status() !== 404) {
      throw new Error(
        `Failed to cleanup task assignment ${assignmentId}. Status: ${response.status()}`,
      );
    }
  }
}