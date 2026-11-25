#!/usr/bin/env python3
"""Tests for the open-questions register: stable keys, answer joins, orphan detection, deterministic output.

    python3 -I -B tools/reqaudit/test_render_questions.py
"""
import sys
import unittest
from pathlib import Path

HERE = Path(__file__).resolve().parent
sys.path.insert(0, str(HERE))
import render_questions as RQ  # noqa: E402

ROOT = HERE.parent.parent


def q(record, text, kind=RQ.KINDS[2]):
    return RQ.row(record, kind, text, f"{record}#Q1", question=text, owner="Product owner", owner_group="Product owner")


def answer(row, rid="QR-9001", gate="Counsel.", gate_type="external"):
    return {"resolution_id": rid, "question_key": row["question_key"], "record_id": row["record_id"], "kind": row["kind"],
            "question": row["question"], "disposition": "approved", "answer": "Do X.", "gate": gate, "gate_type": gate_type,
            "answered_by": "Product owner", "answered_on": "2026-11-24"}


class Keys(unittest.TestCase):
    def test_key_ignores_whitespace_and_case(self):
        self.assertEqual(RQ.question_key("R-1", "k", "Which  provider?"), RQ.question_key("R-1", "k", " which provider? "))

    def test_key_changes_with_text_record_or_kind(self):
        base = RQ.question_key("R-1", "k", "Which provider?")
        self.assertNotEqual(base, RQ.question_key("R-1", "k", "Which providers?"))
        self.assertNotEqual(base, RQ.question_key("R-2", "k", "Which provider?"))
        self.assertNotEqual(base, RQ.question_key("R-1", "j", "Which provider?"))


class Join(unittest.TestCase):
    def test_answer_survives_reordering_and_insertion(self):
        a, b = q("R-1", "First?"), q("R-1", "Second?")
        res = (answer(b),)
        rows, issues = RQ.join((a, b), res)
        rows2, issues2 = RQ.join((q("R-1", "New first?"), b, a), res)
        pick = lambda rs: next(r for r in rs if r["question_key"] == b["question_key"])
        self.assertEqual(issues, ())
        self.assertEqual(issues2, ())
        self.assertEqual(pick(rows)["answer"], pick(rows2)["answer"])
        self.assertEqual(next(r for r in rows2 if r["question_key"] == a["question_key"])["answer"], "")

    def test_edited_question_orphans_its_answer(self):
        original = q("R-1", "Which provider?")
        rows, issues = RQ.join((q("R-1", "Which providers are acceptable?"),), (answer(original),))
        self.assertEqual(len(issues), 1)
        self.assertIn("orphan resolution QR-9001", issues[0])
        self.assertEqual(rows[0]["resolution_id"], "")

    def test_duplicate_resolution_ids_and_keys_are_errors(self):
        a = q("R-1", "Same?")
        _, issues = RQ.join((a, a), (answer(a), answer(a)))
        self.assertTrue(any("duplicate question key" in i for i in issues))
        self.assertTrue(any("duplicate resolution id" in i for i in issues))

    def test_later_resolution_supersedes_earlier(self):
        a = q("R-1", "Which?")
        later = dict(answer(a, rid="QR-9002"), answer="Do Y.")
        rows, _ = RQ.join((a,), (answer(a), later))
        self.assertEqual((rows[0]["resolution_id"], rows[0]["answer"]), ("QR-9002", "Do Y."))

    def test_state_follows_gate_type(self):
        a = q("R-1", "Which?")
        states = [RQ.state_of(RQ.join((a,), res)[0][0]) for res in
                  ((), (answer(a),), (answer(a, gate_type="validation"),), (answer(a, gate_type="none"),))]
        self.assertEqual(states, list(RQ.STATES))


class Repository(unittest.TestCase):
    def test_repository_register_is_complete_and_deterministic(self):
        rows, issues = RQ.join(RQ.collect(ROOT), RQ.load_resolutions(ROOT))
        self.assertEqual(issues, ())
        self.assertEqual(RQ.render(rows, issues), RQ.render(*RQ.join(RQ.collect(ROOT), RQ.load_resolutions(ROOT))))
        self.assertEqual(RQ.to_csv(rows), RQ.to_csv(RQ.join(RQ.collect(ROOT), RQ.load_resolutions(ROOT))[0]))

    def test_committed_register_matches_generator(self):
        rows, issues = RQ.join(RQ.collect(ROOT), RQ.load_resolutions(ROOT))
        self.assertEqual((ROOT / "requirements/OPEN-QUESTIONS.md").read_text(encoding="utf-8"), RQ.render(rows, issues))
        self.assertEqual((ROOT / "requirements/open-questions.csv").read_text(encoding="utf-8"), RQ.to_csv(rows))


if __name__ == "__main__":
    unittest.main(verbosity=1)
