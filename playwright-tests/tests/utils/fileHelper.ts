import * as fs from "node:fs";
import * as path from "node:path";

export class FileHelper {
  public static createTestFile(
    fileName: string = "playwright-test.pdf",
    content: string = "File created for Playwright API testing.",
  ): string {
    const directory = path.join(process.cwd(), "test-files");

    if (!fs.existsSync(directory)) {
      fs.mkdirSync(directory, { recursive: true });
    }

    const filePath = path.join(directory, fileName);

    fs.writeFileSync(filePath, content);

    return filePath;
  }

  public static deleteTestFile(filePath: string): void {
    if (fs.existsSync(filePath)) {
      fs.unlinkSync(filePath);
    }
  }
}
