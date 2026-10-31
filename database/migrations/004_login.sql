USE autotrack;

ALTER TABLE Usuario
    ADD COLUMN ClaveHash VARCHAR(255) NULL;
