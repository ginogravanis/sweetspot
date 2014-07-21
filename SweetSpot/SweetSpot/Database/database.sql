CREATE TABLE IF NOT EXISTS "calibration" (
	"device_id" VARCHAR NOT NULL  DEFAULT (null),
	"axis_tilt" VARCHAR NOT NULL  DEFAULT (null),
	"translate_x" VARCHAR NOT NULL  DEFAULT (null),
	"translate_y" VARCHAR NOT NULL  DEFAULT (null),
	"translate_z" VARCHAR NOT NULL  DEFAULT (null),
	"created" DATETIME NOT NULL  DEFAULT (CURRENT_TIMESTAMP)
	);
CREATE TABLE IF NOT EXISTS "question" (
	"id" INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL  UNIQUE,
	"question" VARCHAR,
	"answer" VARCHAR
    );
CREATE TABLE IF NOT EXISTS "sweetspot_bounds" (
	"x" VARCHAR DEFAULT (null),
	"y" VARCHAR DEFAULT (null)
	);
CREATE TABLE IF NOT EXISTS "test" (
	"id" INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL  UNIQUE,
	"subject" INTEGER NOT NULL,
	"cue" VARCHAR NOT NULL,
	"mapping" VARCHAR NOT NULL,
	"sweetspot_x" VARCHAR,
	"sweetspot_y" VARCHAR,
	"begin" DATETIME NOT NULL  DEFAULT CURRENT_TIMESTAMP,
	"begin_ms" INTEGER NOT NULL,
	"task_completed" INTEGER
	);
CREATE TABLE IF NOT EXISTS "user_position" (
	"test_id" INTEGER DEFAULT (null),
	"timestamp" INTEGER,
	"x" VARCHAR DEFAULT (null),
	"y" VARCHAR DEFAULT (null)
	);

INSERT OR IGNORE INTO question (id, question, answer) VALUES
	(1, "What is the tallest building in Munich?", "1");
