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
	"profession" VARCHAR NOT NULL,
	"has_kinect" BOOL NOT NULL,
	"needs_aid" BOOL NOT NULL,
	"wears_aid" BOOL NOT NULL,
	"snellen" INTEGER NOT NULL,
	"ishihara" INTEGER NOT NULL,
	"pelli_robson" INTEGER NOT NULL
	);
CREATE TABLE "questionnaire" (
	"test_id" INTEGER NOT NULL,
	"cue_number" INTEGER NOT NULL,
	"question" INTEGER NOT NULL,
	"answer" INTEGER NOT NULL,
	PRIMARY KEY ("test_id", "cue_number", "question")
	);
CREATE TABLE "ranking" (
	"test_subject" INTEGER NOT NULL,
	"ranking_type" INTEGER NOT NULL,
	"rank_1" VARCHAR,
	"rank_2" VARCHAR,
	"rank_3" VARCHAR,
	"rank_4" VARCHAR,
	"rank_5" VARCHAR,
	"rank_6" VARCHAR,
	"rank_7" VARCHAR,
	"rank_8" VARCHAR,
	PRIMARY KEY ("test_subject", "ranking_type")
	);
CREATE TABLE "sweetspot_bounds" (
	"x" VARCHAR DEFAULT (null),
	"y" VARCHAR DEFAULT (null)
	);
CREATE TABLE "test" (
	"id" INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL  UNIQUE,
	"subject" INTEGER NOT NULL,
	"cue" VARCHAR NOT NULL,
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
