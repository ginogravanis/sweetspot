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
	"answer_text" VARCHAR,
	"answer_filename" VARCHAR
    );
CREATE TABLE IF NOT EXISTS "sweetspot_bounds" (
	"x" VARCHAR DEFAULT (null),
	"y" VARCHAR DEFAULT (null)
	);
CREATE TABLE IF NOT EXISTS "game_round" (
	"round_id" INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL  UNIQUE,
	"game_id" INTEGER NOT NULL,
	"cue" VARCHAR NOT NULL,
	"mapping" VARCHAR NOT NULL,
	"sweetspot_x" REAL,
	"sweetspot_y" REAL,
	"begin" DATETIME NOT NULL  DEFAULT CURRENT_TIMESTAMP,
	"begin_ms" INTEGER NOT NULL,
	"task_completed" INTEGER
	);
CREATE TABLE IF NOT EXISTS "user_position" (
	"round_id" INTEGER DEFAULT (null),
	"timestamp" INTEGER,
	"x" VARCHAR DEFAULT (null),
	"y" VARCHAR DEFAULT (null)
	);

INSERT OR IGNORE INTO question (id, question, answer_text, answer_filename) VALUES
	(1, "What city was R.M.S. Titanic built in?", "In Belfast.", "1"),
	(2, "How many flowers must honeybees visit to make 500g of honey?", "2 Million.", "2"),
	(3, "What is so special about a violin?", "It is made up of 70 separate pieces of wood.", "3"),
	(4, "How many eggs does a female mackerel lay at a time?", "500,000 eggs.", "4"),
	(5, "What was the most popular male children's name in 2013 in Germany?", "Ben.", "5"),
	(6, "What was the most popular female children's name in 2013 in Germany?", "Mia.", "6"),
	(7, "How many steps would you have to take before you reach the first level of the Eiffel Tower?", "300 steps.", "7"),
	(8, "How many tentacles does a squid have?", "Squids have 10 appendages: 2 tentacles and 8 arms.", "8"),
	(9, "The llama belongs to the family of animals commonly called what?", "The camel family.", "9"),
	(10, "Where would you find the Sea of Tranquility?", "On the moon.", "10"),
	(11, "How many times a day does lightning strike in a typical lightning storm in Catatumbo, Venezuela?", "280 times per hour for up to 10 hours.", "11"),
	(12, "How much does the average cloud weigh?", "500 tons.", "12"),
	(13, "How are the Japanese reusing their old phone booths?", "As fish tanks.", "13"),
	(14, "Which two surnames cover 85% of Chinese people?", "Li and Zhang.", "14"),
	(15, "How much gold do all Indian housewives own combined?", "11% of the world's gold.", "15"),
	(16, "How were inmates of Alcatraz deterred from escaping through the cold San Francisco Bay?", "The showers ran on hot water.", "16"),
	(17, "For how many years was the seal on Tutankhamun's tomb left untouched?", "3,245 years.", "17"),
	(18, "How much of San Francisco's air pollution comes from China?", "About 30%", "18"),
	(19, "For how many years did the price of 5 cents for Coca Cola stay the same?", "For 70 years (1886-1959).", "19"),
	(20, "Why did Prince Ludwig of Bavaria hold the first Oktoberfest in 1810?", "To celebrate his marriage.", "20"),
	(21, "What is the most common form of transportation in Woodstock, Columbia?", "Golf carts.", "21");