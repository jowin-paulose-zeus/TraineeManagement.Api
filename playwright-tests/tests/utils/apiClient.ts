import { APIRequestContext, APIResponse } from "@playwright/test";
import fs from "fs";

export class ApiClient {
  private readonly request: APIRequestContext;
  private token?: string;

  constructor(request: APIRequestContext, token?: string) {
    this.request = request;
    this.token = token;
  }

  private getHeaders(): Record<string, string> {
    if (!this.token) {
      return {};
    }

    return {
      Authorization: `Bearer ${this.token}`,
    };
  }

  public async get(url: string): Promise<APIResponse> {
    return await this.request.get(url, {
      headers: this.getHeaders(),
    });
  }

  public async post(url: string, data?: unknown): Promise<APIResponse> {
    return await this.request.post(url, {
      headers: this.getHeaders(),
      data,
    });
  }

  public async put(url: string, data?: unknown): Promise<APIResponse> {
    return await this.request.put(url, {
      headers: this.getHeaders(),
      data,
    });
  }

  public async delete(url: string): Promise<APIResponse> {
    return await this.request.delete(url, {
      headers: this.getHeaders(),
    });
  }

  public async uploadFile(url: string, filePath: string): Promise<APIResponse> {
    return await this.request.post(url, {
      headers: this.getHeaders(),
      multipart: {
        file: {
          name: filePath.split("/").pop() ?? "test-file.txt",
          mimeType: "text/plain",
          buffer: fs.readFileSync(filePath),
        },
      },
    });
  }
}
