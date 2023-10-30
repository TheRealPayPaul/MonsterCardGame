CREATE TABLE users (
	user_id SERIAL PRIMARY KEY,
	username VARCHAR UNIQUE NOT NULL,
	password VARCHAR NOT NULL,
	coins INT NOT NULL DEFAULT 0,
	elo INT NOT NULL DEFAULT 0,
	wins INT NOT NULL DEFAULT 0,
	losses INT NOT NULL DEFAULT 0
);

CREATE TABLE cards (
	card_id SERIAL PRIMARY KEY,
	name VARCHAR NOT NULL,
	damage INT NOT NULL,
	element_type VARCHAR NOT NULL,
	card_type VARCHAR NOT NULL,
	deck_pos INT,
	description TEXT NOT NULL,
	fk_owner_id INT REFERENCES users (user_id)
);

CREATE TABLE trades (
	trade_id SERIAL PRIMARY KEY,
	fk_offered_card_id INT REFERENCES cards (card_id),
	wanted_card_type VARCHAR NOT NULL,
	wanted_min_damage INT NOT NULL
);

CREATE TABLE transactions (
	transaction_id SERIAL PRIMARY KEY,
	fk_user_id INT REFERENCES users (user_id),
	coins_spent INT NOT NULL,
	description TEXT NOT NULL,
	timestamp TIMESTAMPTZ NOT NULL
);

CREATE VIEW trade_overviews AS
	SELECT
		trades.trade_id, trades.wanted_card_type, trades.wanted_min_damage,
		cards.card_id, cards.name, cards.damage, cards.element_type, cards.card_type, cards.description,
		users.user_id, users.username
	FROM trades
	JOIN cards ON card_id = trades.fk_offered_card_id
	JOIN users ON user_id = cards.fk_owner_id;