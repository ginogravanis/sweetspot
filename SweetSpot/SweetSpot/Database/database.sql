CREATE TABLE "calibration" (
	"device_id" VARCHAR NOT NULL  DEFAULT (null),
	"axis_tilt" VARCHAR NOT NULL  DEFAULT (null),
	"translate_x" VARCHAR NOT NULL  DEFAULT (null),
	"translate_y" VARCHAR NOT NULL  DEFAULT (null),
	"translate_z" VARCHAR NOT NULL  DEFAULT (null),
	"created" DATETIME NOT NULL  DEFAULT (CURRENT_TIMESTAMP)
	);
CREATE TABLE "sweetspot_bounds" (
	"x" VARCHAR DEFAULT (null),
	"y" VARCHAR DEFAULT (null)
	);
CREATE TABLE "test" (
	"id" INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL  UNIQUE,
	"subject" INTEGER NOT NULL,
	"cue" VARCHAR NOT NULL,
	"sweetspot_x" VARCHAR NOT NULL,
	"sweetspot_y" VARCHAR NOT NULL,
	"begin" DATETIME NOT NULL  DEFAULT CURRENT_TIMESTAMP,
	"begin_ms" INTEGER NOT NULL,
	"task_completed" INTEGER
	);
CREATE TABLE "user_position" (
	"test_id" INTEGER DEFAULT (null),
	"timestamp" INTEGER,
	"x" VARCHAR DEFAULT (null),
	"y" VARCHAR DEFAULT (null)
	);
