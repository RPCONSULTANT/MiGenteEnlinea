import fs from "node:fs";
import path from "node:path";

const input = path.join("artifacts", "e2e", "playwright-report.json");
const output = path.join("artifacts", "e2e", "summary.json");
const backlogOutput = path.join("artifacts", "e2e", "improvement-backlog.md");
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
  },
  improvementBacklog: {
    total: 0,
    byCategory: {}
  }
};

function classifyIssue(message) {
  const text = String(message || "").toLowerCase();
  if (text.includes("cors") || text.includes("failed to fetch") || text.includes("500") || text.includes("timeout")) {
    return "bug";
  }
  if (text.includes("no contract buttons") || text.includes("no contact request buttons") || text.includes("not found")) {
    return "gap_requerimiento";
  }
  return "mejora_ux";
}

if (fs.existsSync(issuesDir)) {
  const issueFiles = fs
    .readdirSync(issuesDir)
    .filter((name) => name.endsWith(".json") && name !== "playwright-report.json" && name !== "summary.json");

  const byType = {};
  const byMessage = {};
  const byCategory = {};
  let runtimeTotal = 0;
  const backlogRows = [];

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
      const category = classifyIssue(message);
      byCategory[category] = (byCategory[category] ?? 0) + 1;
      backlogRows.push({
        category,
        type,
        message,
        test: payload.test ?? "unknown",
        file: payload.file ?? "unknown",
        url: issue.url ?? ""
      });
    }
  }

  summary.runtimeIssues.total = runtimeTotal;
  summary.runtimeIssues.byType = byType;
  summary.runtimeIssues.topMessages = Object.entries(byMessage)
    .sort((a, b) => b[1] - a[1])
    .slice(0, 12)
    .map(([message, count]) => ({ message, count }));

  summary.improvementBacklog.total = backlogRows.length;
  summary.improvementBacklog.byCategory = byCategory;

  const markdownLines = [];
  markdownLines.push("# E2E Improvement Backlog");
  markdownLines.push("");
  markdownLines.push(`Generated: ${new Date().toISOString()}`);
  markdownLines.push("");
  markdownLines.push("## Summary");
  markdownLines.push("");
  markdownLines.push(`- Total findings: ${backlogRows.length}`);
  markdownLines.push(`- bug: ${byCategory.bug ?? 0}`);
  markdownLines.push(`- gap_requerimiento: ${byCategory.gap_requerimiento ?? 0}`);
  markdownLines.push(`- mejora_ux: ${byCategory.mejora_ux ?? 0}`);
  markdownLines.push("");
  markdownLines.push("## Findings");
  markdownLines.push("");
  markdownLines.push("| Category | Type | File | Test | URL | Message |");
  markdownLines.push("|---|---|---|---|---|---|");

  for (const row of backlogRows.slice(0, 300)) {
    const safe = (value) => String(value ?? "").replace(/\|/g, "\\|").replace(/\n/g, " ").trim();
    markdownLines.push(
      `| ${safe(row.category)} | ${safe(row.type)} | ${safe(row.file)} | ${safe(row.test)} | ${safe(row.url)} | ${safe(
        row.message
      )} |`
    );
  }

  fs.writeFileSync(backlogOutput, markdownLines.join("\n"));
}

fs.mkdirSync(path.dirname(output), { recursive: true });
fs.writeFileSync(output, JSON.stringify(summary, null, 2));
console.log(JSON.stringify(summary, null, 2));
