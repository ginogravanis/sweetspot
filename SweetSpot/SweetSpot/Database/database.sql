CREATE TABLE "calibration" (
	"device_id" VARCHAR NOT NULL  DEFAULT (null),
	"axis_tilt" VARCHAR NOT NULL  DEFAULT (null),
	"translate_x" VARCHAR NOT NULL  DEFAULT (null),
	"translate_y" VARCHAR NOT NULL  DEFAULT (null),
	"translate_z" VARCHAR NOT NULL  DEFAULT (null),
	"created" DATETIME NOT NULL  DEFAULT (CURRENT_TIMESTAMP)
	);
CREATE TABLE "demographics" (
	"test_subject" INTEGER PRIMARY KEY  NOT NULL  UNIQUE,
	"sex" INTEGER NOT NULL,
	"age" INTEGER NOT NULL,
	"profession" VARCHAR NOT NULL,
	"has_kinect" BOOL NOT NULL,
	"needs_aid" BOOL NOT NULL,
	"wears_aid" BOOL NOT NULL,
	"snellen" INTEGER NOT NULL,
	"ishihara" INTEGER NOT NULL,
	"pelli_robson" INTEGER NOT NULL
	);
CREATE TABLE "questionnaire" (
	"test_subject" INTEGER NOT NULL,
	"cue" VARCHAR NOT NULL,
	"answer_1" INTEGER NOT NULL,
	"answer_2" INTEGER NOT NULL,
	"answer_3" INTEGER NOT NULL,
	PRIMARY KEY ("test_subject", "cue")
	);
CREATE TABLE "ranking" (
	"test_subject" INTEGER NOT NULL,
	"ranking_type" INTEGER NOT NULL,
	"rank" INTEGER NOT NULL,
	"cue" VARCHAR NOT NULL,
	PRIMARY KEY ("test_subject", "ranking_type", "rank")
	);
CREATE TABLE "sweetspot_bounds" (
	"x" VARCHAR DEFAULT (null),
	"y" VARCHAR DEFAULT (null)
	);
CREATE TABLE "test" (
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
CREATE TABLE "user_position" (
	"test_id" INTEGER DEFAULT (null),
	"timestamp" INTEGER,
	"x" VARCHAR DEFAULT (null),
	"y" VARCHAR DEFAULT (null)
	);
