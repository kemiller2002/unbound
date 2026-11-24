-- Hold publication on a charity (registry version bump); conflicts with releases that read the row FOR SHARE.
\set charity random(:charity_lo, :charity_hi)
UPDATE unb.charity_hold SET held = NOT held, hold_version = hold_version + 1 WHERE charity_id = :charity;
