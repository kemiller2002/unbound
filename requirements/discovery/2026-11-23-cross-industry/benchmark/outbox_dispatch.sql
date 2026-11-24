-- Claim-and-mark delivery with SKIP LOCKED (UNB-XIN-EVA-007); never an id watermark.
WITH c AS (SELECT id FROM unb.outbox WHERE delivered_at IS NULL ORDER BY id LIMIT 200 FOR UPDATE SKIP LOCKED)
UPDATE unb.outbox o SET delivered_at = clock_timestamp(), claimed_by = 'd' || :client_id FROM c WHERE o.id = c.id;
