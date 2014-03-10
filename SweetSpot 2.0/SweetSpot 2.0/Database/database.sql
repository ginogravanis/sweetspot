CREATE TABLE "calibration" ("device" VARCHAR NOT NULL , "axis_tilt" FLOAT NOT NULL , "translate_x" FLOAT NOT NULL , "translate_y" FLOAT NOT NULL );
CREATE TABLE "sweetspot" ("id" INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL  UNIQUE , "x" FLOAT, "y" FLOAT);
CREATE TABLE "test" ("id" INTEGER PRIMARY KEY  NOT NULL ,"subject" INTEGER NOT NULL ,"screen" VARCHAR NOT NULL ,"begin" DATETIME NOT NULL  DEFAULT (CURRENT_TIMESTAMP) ,"sweetspot" INT DEFAULT (null) );
CREATE TABLE "user_position" ("test" INTEGER,"timestamp" INTEGER,"x" FLOAT,"y" FLOAT);