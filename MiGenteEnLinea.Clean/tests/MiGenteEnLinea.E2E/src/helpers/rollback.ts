import { APIRequestContext } from "@playwright/test";
import { sleep } from "./url";

type RollbackAction = () => Promise<void>;

export class RollbackManager {
  private readonly actions: RollbackAction[] = [];

  register(action: RollbackAction): void {
    this.actions.push(action);
  }

  async executeAll(maxRetries = 3): Promise<void> {
    const errors: string[] = [];

    while (this.actions.length > 0) {
      const action = this.actions.pop()!;
      let success = false;

      for (let i = 0; i < maxRetries; i++) {
        try {
          await action();
          success = true;
          break;
        } catch (err) {
          if (i < maxRetries - 1) {
            await sleep((i + 1) * 500);
          } else {
            errors.push(String(err));
          }
        }
      }

      if (!success) {
        // Continue and report all failures at the end.
      }
    }

    if (errors.length > 0) {
      throw new Error(`ROLLBACK_FAILED: ${errors.join(" | ")}`);
    }
  }
}

export async function safeDeleteById(api: APIRequestContext, url: string, token: string): Promise<void> {
  const response = await api.fetch(url, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${token}`
    }
  });

  if (response.status() === 404) {
    return;
  }

  if (!response.ok()) {
    throw new Error(`Failed to delete resource at ${url}. Status ${response.status()}`);
  }
}
