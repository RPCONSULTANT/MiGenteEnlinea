import fs from "node:fs";
import path from "node:path";

const input = path.join("artifacts", "e2e", "playwright-report.json");
const output = path.join("artifacts", "e2e", "summary.json");
const issuesDir = path.join("artifacts", "e2e");

if (!fs.existsSync(input)) {
  console.error(`Missing report file: ${input}`);
  process.exit(1);
}

const report = JSON.parse(fs.readFileSync(input, "utf8"));

let total = 0;
let passed = 0;
let failed = 0;
let skipped = 0;
const failedTests = [];

function walkSuite(suite) {
  for (const spec of suite.specs ?? []) {
    for (const test of spec.tests ?? []) {
      for (const result of test.results ?? []) {
        total += 1;
        if (result.status === "passed") passed += 1;
        else if (result.status === "failed") {
          failed += 1;
          const firstError = result.error?.message || result.errors?.[0]?.message || "No error message";
          failedTests.push({
            title: test.title,
            file: spec.file,
            message: String(firstError).split("\n")[0]
          });
        }
        else if (result.status === "skipped") skipped += 1;
      }
    }
  }

  for (const child of suite.suites ?? []) {
    walkSuite(child);
  }
}

for (const suite of report.suites ?? []) {
  walkSuite(suite);
}

const summary = {
  generatedAt: new Date().toISOString(),
  total,
  passed,
  failed,
  skipped,
  status: failed > 0 ? "failed" : "passed",
  failedTests: failedTests.slice(0, 20),
  runtimeIssues: {
    total: 0,
    byType: {},
    topMessages: []
  }
};

if (fs.existsSync(issuesDir)) {
  const issueFiles = fs
    .readdirSync(issuesDir)
    .filter((name) => name.endsWith(".json") && name !== "playwright-report.json" && name !== "summary.json");

  const byType = {};
  const byMessage = {};
  let runtimeTotal = 0;

  for (const file of issueFiles) {
    const fullPath = path.join(issuesDir, file);
    const payload = JSON.parse(fs.readFileSync(fullPath, "utf8"));
    const issues = Array.isArray(payload.issues) ? payload.issues : [];

    for (const issue of issues) {
      runtimeTotal += 1;
      const type = issue.type ?? "unknown";
      byType[type] = (byType[type] ?? 0) + 1;
      const message = String(issue.message ?? "unknown");
      byMessage[message] = (byMessage[message] ?? 0) + 1;
    }
  }

  summary.runtimeIssues.total = runtimeTotal;
  summary.runtimeIssues.byType = byType;
  summary.runtimeIssues.topMessages = Object.entries(byMessage)
    .sort((a, b) => b[1] - a[1])
    .slice(0, 12)
    .map(([message, count]) => ({ message, count }));
}

fs.mkdirSync(path.dirname(output), { recursive: true });
fs.writeFileSync(output, JSON.stringify(summary, null, 2));
console.log(JSON.stringify(summary, null, 2));
