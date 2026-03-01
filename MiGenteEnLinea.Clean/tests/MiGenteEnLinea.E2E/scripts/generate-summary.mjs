import fs from "node:fs";
import path from "node:path";

const input = path.join("artifacts", "e2e", "playwright-report.json");
const output = path.join("artifacts", "e2e", "summary.json");

if (!fs.existsSync(input)) {
  console.error(`Missing report file: ${input}`);
  process.exit(1);
}

const report = JSON.parse(fs.readFileSync(input, "utf8"));

let total = 0;
let passed = 0;
let failed = 0;
let skipped = 0;

function walkSuite(suite) {
  for (const spec of suite.specs ?? []) {
    for (const test of spec.tests ?? []) {
      for (const result of test.results ?? []) {
        total += 1;
        if (result.status === "passed") passed += 1;
        else if (result.status === "failed") failed += 1;
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
  status: failed > 0 ? "failed" : "passed"
};

fs.mkdirSync(path.dirname(output), { recursive: true });
fs.writeFileSync(output, JSON.stringify(summary, null, 2));
console.log(JSON.stringify(summary, null, 2));
