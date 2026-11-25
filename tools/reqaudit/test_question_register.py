#!/usr/bin/env python3
"""Regression tests for the source-linked question register (requirements-governance tooling only).

Each test copies the repository's requirement data into a temporary directory, perturbs it, and runs
the generator. Run: python3 -I -B tools/reqaudit/test_question_register.py <repo-root>
"""
import csv
import io
import json
import shutil
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

ROOT = Path(sys.argv.pop(1) if len(sys.argv) > 1 else ".").resolve()
TOOL = ROOT / "tools/reqaudit/render_questions.py"


def sandbox():
    d = Path(tempfile.mkdtemp(prefix="qreg-"))
    shutil.copytree(ROOT / "requirements", d / "requirements")
    return d


def run(d, *extra):
    return subprocess.run([sys.executable, "-I", "-B", str(TOOL), str(d), *extra], capture_output=True, text=True)


def append_event(d, event):
    p = d / "requirements/questions/resolution-events.jsonl"
    p.write_text(p.read_text(encoding="utf-8") + json.dumps(event) + "\n", encoding="utf-8")


def read_csv(d):
    return tuple(csv.DictReader(io.StringIO((d / "requirements/open-questions.csv").read_text(encoding="utf-8"))))


class RegisterTests(unittest.TestCase):
    def test_regeneration_is_idempotent_and_keeps_answers(self):
        d = sandbox()
        first = (run(d), (d / "requirements/open-questions.csv").read_text(encoding="utf-8"))
        second = (run(d), (d / "requirements/open-questions.csv").read_text(encoding="utf-8"))
        self.assertEqual(first[0].returncode, 0, first[0].stderr)
        self.assertEqual(first[1], second[1])
        self.assertEqual(len(read_csv(d)), 316)
        self.assertTrue(all(r["answer"].strip() for r in read_csv(d)))

    def test_hand_edited_answer_is_not_silently_discarded(self):
        d = sandbox()
        rows = read_csv(d)
        rows = (dict(rows[0], answer="hand answer"),) + rows[1:]
        buf = io.StringIO()
        w = csv.DictWriter(buf, fieldnames=list(rows[0].keys()), quoting=csv.QUOTE_ALL, lineterminator="\n")
        w.writeheader(); w.writerows(rows)
        (d / "requirements/open-questions.csv").write_text(buf.getvalue(), encoding="utf-8")
        r = run(d)
        self.assertEqual(r.returncode, 3, r.stderr)
        self.assertIn("hand answer", (d / "requirements/open-questions.csv").read_text(encoding="utf-8"))

    def test_orphaned_resolution_fails(self):
        d = sandbox()
        append_event(d, {"event_id": "QE-999999", "question_id": "UNB-NOPE-001#Q1", "on": "2026-11-24", "type": "proposed",
                         "artifact": "test", "actor": "test", "set": {"answer": "x"}})
        r = run(d)
        self.assertNotEqual(r.returncode, 0)
        self.assertIn("unregistered-question", r.stderr)

    def test_duplicate_event_id_fails(self):
        d = sandbox()
        append_event(d, {"event_id": "QE-000001", "question_id": "SPD-CFG-001", "on": "2026-11-24", "type": "proposed",
                         "artifact": "test", "actor": "test", "set": {"answer": "x"}})
        r = run(d)
        self.assertNotEqual(r.returncode, 0)
        self.assertIn("duplicate-event-id", r.stderr)

    def test_changed_question_text_fails(self):
        d = sandbox()
        p = d / "requirements/discovery/2026-11-24-simplification-economics/data/decisions.csv"
        rows = tuple(csv.DictReader(io.StringIO(p.read_text(encoding="utf-8"))))
        rows = tuple(dict(r, question=r["question"] + " (edited)") if r["decision_id"] == "SPD-CFG-001" else r for r in rows)
        buf = io.StringIO()
        w = csv.DictWriter(buf, fieldnames=list(rows[0].keys()), lineterminator="\n")
        w.writeheader(); w.writerows(rows)
        p.write_text(buf.getvalue(), encoding="utf-8")
        r = run(d)
        self.assertNotEqual(r.returncode, 0)
        self.assertIn("changed-question-text", r.stderr)

    def test_invalid_closure_fails(self):
        d = sandbox()
        append_event(d, {"event_id": "QE-999998", "question_id": "SPD-CFG-001", "on": "2026-11-24", "type": "resolved",
                         "artifact": "test", "actor": "test",
                         "set": {"resolution_status": "resolved", "disposition": "external-authorization-required"}})
        r = run(d)
        self.assertNotEqual(r.returncode, 0)
        self.assertIn("invalid-closure", r.stderr)

    def test_closed_source_decision_stays_registered(self):
        d = sandbox()
        p = d / "requirements/discovery/2026-11-24-simplification-economics/data/decisions.csv"
        rows = tuple(csv.DictReader(io.StringIO(p.read_text(encoding="utf-8"))))
        rows = tuple(dict(r, status="decided-provisional") if r["decision_id"] == "SPD-CFG-001" else r for r in rows)
        buf = io.StringIO()
        w = csv.DictWriter(buf, fieldnames=list(rows[0].keys()), lineterminator="\n")
        w.writeheader(); w.writerows(rows)
        p.write_text(buf.getvalue(), encoding="utf-8")
        r = run(d)
        self.assertEqual(r.returncode, 0, r.stderr)
        self.assertIn("SPD-CFG-001", {x["question_id"] for x in read_csv(d)})


if __name__ == "__main__":
    unittest.main(verbosity=2)
