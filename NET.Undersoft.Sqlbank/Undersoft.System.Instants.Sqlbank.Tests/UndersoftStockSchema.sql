USE [master]
GO

IF EXISTS(select * from sys.databases where name='UndersoftStock')
	ALTER DATABASE UndersoftStock SET SINGLE_USER  WITH ROLLBACK IMMEDIATE;
	GO
IF EXISTS(select * from sys.databases where name='UndersoftStock')
	DROP DATABASE UndersoftStock
	GO

CREATE DATABASE UndersoftStock
GO

USE UndersoftStock

CREATE TABLE Countries
(
country_name VARCHAR(25) NOT NULL UNIQUE
CHECK (LEN(country_name) > 0),
country_code CHAR(2) NOT NULL PRIMARY KEY
)

CREATE TABLE Currencies
(
country_code CHAR(2) NOT NULL UNIQUE
REFERENCES Countries (country_code),
currency_name VARCHAR(25) NOT NULL CHECK (LEN(currency_name) > 0),
currency_code CHAR(3) NOT NULL PRIMARY KEY,
)

CREATE TABLE StockMarkets
(
market_name VARCHAR(25) NOT NULL PRIMARY KEY
CHECK (LEN(market_name) > 0),
currency_code CHAR(3) NOT NULL REFERENCES Currencies (currency_code)
)

CREATE TABLE StockMarketCalendars
(
market_name VARCHAR(25) NOT NULL
REFERENCES StockMarkets (market_name),
trading_date SMALLDATETIME NOT NULL
CHECK (trading_date = CONVERT(CHAR(8), trading_date, 112)),
PRIMARY KEY (market_name, trading_date)
)

CREATE TABLE PublicCorporations
(
corporation_name VARCHAR(25) NOT NULL
CHECK (LEN(corporation_name) > 0),
country_code CHAR(2) NOT NULL REFERENCES Countries (country_code),
permno INT NOT NULL PRIMARY KEY,
permco INT NOT NULL,
UNIQUE (country_code, corporation_name)
)

CREATE TABLE StockListings
(
permno INT NOT NULL REFERENCES PublicCorporations (permno),
permco INT NOT NULL,
market_name VARCHAR(25) NOT NULL,
traded_from_date SMALLDATETIME NOT NULL,
traded_to_date SMALLDATETIME NULL,
primary_listing INT NOT NULL DEFAULT 1 CHECK (primary_listing IN (0, 1)),
CHECK (traded_to_date >= traded_from_date OR traded_to_date IS NULL),
PRIMARY KEY (permno, market_name, traded_from_date),
FOREIGN KEY (market_name, traded_from_date)
REFERENCES StockMarketCalendars (market_name, trading_date),
FOREIGN KEY (market_name, traded_to_date)
REFERENCES StockMarketCalendars (market_name, trading_date)
)

CREATE TABLE TickerAssociations
(
permno INT NOT NULL REFERENCES PublicCorporations (permno),
permco INT NOT NULL,
market_name VARCHAR(25) NOT NULL,
ticker CHAR(5) NOT NULL,
start_date SMALLDATETIME NOT NULL,
end_date SMALLDATETIME NULL,
CHECK (end_date >= start_date OR end_date IS NULL),
PRIMARY KEY (permno, market_name, start_date),
UNIQUE (ticker, market_name, start_date),
FOREIGN KEY (market_name, start_date)
REFERENCES StockMarketCalendars (market_name, trading_date),
FOREIGN KEY (market_name, end_date)
REFERENCES StockMarketCalendars (market_name, trading_date)
)

CREATE TABLE StockTradingActivity
(
permno INT NOT NULL REFERENCES PublicCorporations (permno),
permco INT NOT NULL,
market_name VARCHAR(25) NOT NULL,
trading_date SMALLDATETIME NOT NULL
CHECK (trading_date < CURRENT_TIMESTAMP),
open_price DECIMAL (10, 2) NOT NULL CHECK (open_price > 0),
hi_price DECIMAL (10, 2) NOT NULL CHECK (hi_price > 0),
lo_price DECIMAL (10, 2) NOT NULL CHECK (lo_price > 0),
close_price DECIMAL (10, 2) NOT NULL CHECK (close_price > 0),
trading_volume INT NOT NULL CHECK (trading_volume >= 0),
PRIMARY KEY (permno, market_name, trading_date),
CHECK (lo_price <= hi_price AND
open_price BETWEEN lo_price AND hi_price AND
close_price BETWEEN lo_price AND hi_price),
FOREIGN KEY (market_name, trading_date)
REFERENCES StockMarketCalendars (market_name, trading_date)
)

CREATE TABLE SplitFactors
(
permno INT NOT NULL REFERENCES PublicCorporations (permno),
permco INT NOT NULL,
market_name VARCHAR(25) NOT NULL,
split_date SMALLDATETIME NOT NULL,
split_factor DECIMAL (6, 3) NOT NULL
CHECK (split_factor > 0 AND split_factor <> 1),
PRIMARY KEY (permno, market_name, split_date),
FOREIGN KEY (market_name, split_date)
REFERENCES StockMarketCalendars (market_name, trading_date)
)