import os
import re
import sys

SUBJECT_RE = re.compile(r"^(feat|fix|refactor|test|build|ci|docs|chore)\((backend|android|deploy|github|readme|repo)\): [a-z]")
TRAILER_RE = re.compile(r"^Jira: PIT-(\d+)$")
BRANCH_RE = re.compile(r"^PIT-(\d+)-[a-z0-9]+(?:-[a-z0-9]+)*$")
TITLE_MAX = 66
LINE_MAX = 72


def title_problems(title):
    problems = []
    if not SUBJECT_RE.match(title):
        problems.append('the title must be "<type>(<scope>): <lowercase verb> ...", '
                        'e.g. "feat(backend): add the User entity and migration"')
    if len(title) > TITLE_MAX:
        problems.append(f"the title has {len(title)} characters, at most {TITLE_MAX}")
    if title.endswith("."):
        problems.append("the title must not end with a period")
    if ";" in title:
        problems.append("the title must not contain a semicolon")
    if not title.isascii():
        problems.append("the title must be plain ASCII English")
    return problems


def body_problems(body, branch):
    body = body.replace("\r", "")
    lines = body.rstrip().split("\n") if body.strip() else []
    problems = []
    trailer = TRAILER_RE.match(lines[-1]) if lines else None
    branch_key = BRANCH_RE.match(branch)
    if not trailer:
        problems.append('the last line of the body must be the trailer "Jira: PIT-<n>"')
    if not branch_key:
        problems.append(f'the branch "{branch}" must be "PIT-<n>-<slug>"')
    if trailer and branch_key and trailer.group(1) != branch_key.group(1):
        problems.append(f"the trailer says PIT-{trailer.group(1)}, but the branch is {branch}")
    long_lines = [n for n, line in enumerate(lines, 1) if len(line) > LINE_MAX]
    if long_lines:
        problems.append(f"body lines {long_lines} have more than {LINE_MAX} characters")
    if any(re.match(r"^(#|[-*] |\d+\. )", line) for line in lines):
        problems.append("the body is plain text: no Markdown headings or lists")
    return problems


def main():
    if os.environ.get("PR_AUTHOR") == "dependabot[bot]":
        print("A Dependabot pull request: the title and body checks are skipped.")
        return 0
    problems = title_problems(os.environ.get("PR_TITLE", ""))
    problems += body_problems(os.environ.get("PR_BODY", ""), os.environ.get("HEAD_REF", ""))
    for problem in problems:
        print(f"::error::{problem}")
    if problems:
        return 1
    print("The pull request title and body follow the rules.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
