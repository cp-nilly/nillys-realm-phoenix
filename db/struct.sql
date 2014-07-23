-- --------------------------------------------------------
-- Host:                         162.244.26.153
-- Server version:               5.6.16 - MySQL Community Server (GPL)
-- Server OS:                    Win32
-- HeidiSQL Version:             8.0.0.4396
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Dumping database structure for rotmg
CREATE DATABASE IF NOT EXISTS `rotmg` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `rotmg`;


-- Dumping structure for table rotmg.accounts
CREATE TABLE IF NOT EXISTS `accounts` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `uuid` varchar(128) NOT NULL,
  `password` varchar(256) NOT NULL,
  `name` varchar(64) NOT NULL DEFAULT 'DEFAULT',
  `rank` int(11) NOT NULL DEFAULT '0',
  `tag` varchar(12) NOT NULL DEFAULT 'P',
  `namechosen` tinyint(1) NOT NULL DEFAULT '0',
  `verified` tinyint(1) NOT NULL DEFAULT '1',
  `guild` int(11) NOT NULL,
  `guildFame` int(11) NOT NULL,
  `guildRank` int(11) NOT NULL,
  `vaultCount` int(11) NOT NULL DEFAULT '1',
  `maxCharSlot` int(11) NOT NULL DEFAULT '2',
  `regTime` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `guest` tinyint(1) NOT NULL DEFAULT '0',
  `banned` tinyint(1) NOT NULL DEFAULT '0',
  `locked` varchar(512) NOT NULL,
  `ignored` varchar(512) NOT NULL,
  `bonuses` varchar(512) NOT NULL,
  PRIMARY KEY (`id`,`tag`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- Data exporting was unselected.


-- Dumping structure for table rotmg.arenalb
CREATE TABLE IF NOT EXISTS `arenalb` (
  `id` int(11) NOT NULL,
  `wave` int(11) NOT NULL,
  `players` varchar(256) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- Data exporting was unselected.


-- Dumping structure for table rotmg.backpacks
CREATE TABLE IF NOT EXISTS `backpacks` (
  `accId` int(11) NOT NULL,
  `charId` int(11) NOT NULL,
  `num` int(11) NOT NULL,
  `items` varchar(128) NOT NULL DEFAULT '-1, -1, -1, -1, -1, -1, -1, -1',
  PRIMARY KEY (`accId`,`charId`,`num`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- Data exporting was unselected.


-- Dumping structure for table rotmg.boards
CREATE TABLE IF NOT EXISTS `boards` (
  `guildId` int(11) NOT NULL,
  `text` varchar(1024) NOT NULL,
  PRIMARY KEY (`guildId`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- Data exporting was unselected.


-- Dumping structure for table rotmg.characters
CREATE TABLE IF NOT EXISTS `characters` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `accId` int(11) NOT NULL,
  `charId` int(11) NOT NULL,
  `charType` int(11) NOT NULL DEFAULT '782',
  `level` int(11) NOT NULL DEFAULT '1',
  `exp` int(11) NOT NULL DEFAULT '0',
  `fame` int(11) NOT NULL DEFAULT '0',
  `items` varchar(128) NOT NULL DEFAULT '-1, -1, -1, -1',
  `hp` int(11) NOT NULL DEFAULT '1',
  `mp` int(11) NOT NULL DEFAULT '1',
  `stats` varchar(64) NOT NULL DEFAULT '1, 1, 1, 1, 1, 1, 1, 1',
  `dead` tinyint(1) NOT NULL DEFAULT '0',
  `tex1` int(11) NOT NULL DEFAULT '0',
  `tex2` int(11) NOT NULL DEFAULT '0',
  `pet` int(11) NOT NULL DEFAULT '-1',
  `fameStats` varchar(128) NOT NULL,
  `createTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `deathTime` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `totalFame` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- Data exporting was unselected.


-- Dumping structure for table rotmg.classstats
CREATE TABLE IF NOT EXISTS `classstats` (
  `accId` int(11) NOT NULL,
  `objType` int(11) NOT NULL,
  `bestLv` int(11) NOT NULL DEFAULT '1',
  `bestFame` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`accId`,`objType`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- Data exporting was unselected.


-- Dumping structure for table rotmg.clinews
CREATE TABLE IF NOT EXISTS `clinews` (
  `name` varchar(128) NOT NULL,
  `contents` varchar(256) NOT NULL,
  `date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `newsType` int(11) NOT NULL,
  `cliVersion` varchar(128) NOT NULL,
  `link` varchar(256) NOT NULL,
  PRIMARY KEY (`date`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- Data exporting was unselected.


-- Dumping structure for table rotmg.death
CREATE TABLE IF NOT EXISTS `death` (
  `accId` int(11) NOT NULL,
  `chrId` int(11) NOT NULL,
  `name` varchar(64) NOT NULL DEFAULT 'DEFAULT',
  `charType` int(11) NOT NULL DEFAULT '782',
  `tex1` int(11) NOT NULL DEFAULT '0',
  `tex2` int(11) NOT NULL DEFAULT '0',
  `items` varchar(128) NOT NULL DEFAULT '-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1',
  `fame` int(11) NOT NULL DEFAULT '0',
  `fameStats` varchar(128) NOT NULL,
  `totalFame` int(11) NOT NULL DEFAULT '0',
  `firstBorn` tinyint(1) NOT NULL,
  `killer` varchar(128) NOT NULL,
  `time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`accId`,`chrId`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- Data exporting was unselected.


-- Dumping structure for table rotmg.guilds
CREATE TABLE IF NOT EXISTS `guilds` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(128) NOT NULL DEFAULT 'DEFAULT_GUILD',
  `members` varchar(128) NOT NULL,
  `guildFame` int(11) NOT NULL,
  `totalGuildFame` int(11) NOT NULL,
  `level` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- Data exporting was unselected.


-- Dumping structure for table rotmg.ips
CREATE TABLE IF NOT EXISTS `ips` (
  `ip` varchar(128) NOT NULL,
  `banned` tinyint(1) NOT NULL DEFAULT '0',
  `gifts` varchar(256) NOT NULL DEFAULT '',
  PRIMARY KEY (`ip`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- Data exporting was unselected.


-- Dumping structure for table rotmg.news
CREATE TABLE IF NOT EXISTS `news` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `icon` varchar(16) NOT NULL DEFAULT 'info',
  `title` varchar(128) NOT NULL DEFAULT 'Default news title',
  `text` varchar(128) NOT NULL DEFAULT 'Default news text',
  `link` varchar(256) NOT NULL DEFAULT 'http://mmoe.net/',
  `date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- Data exporting was unselected.


-- Dumping structure for table rotmg.stats
CREATE TABLE IF NOT EXISTS `stats` (
  `accId` int(11) NOT NULL,
  `fame` int(11) NOT NULL,
  `totalFame` int(11) NOT NULL,
  `credits` int(11) NOT NULL,
  `totalCredits` int(11) NOT NULL,
  `deaths` int(11) NOT NULL,
  `totalDeaths` int(11) NOT NULL,
  PRIMARY KEY (`accId`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- Data exporting was unselected.


-- Dumping structure for table rotmg.vaults
CREATE TABLE IF NOT EXISTS `vaults` (
  `accId` int(11) NOT NULL,
  `chestId` int(11) NOT NULL AUTO_INCREMENT,
  `items` varchar(128) NOT NULL,
  PRIMARY KEY (`accId`,`chestId`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- Data exporting was unselected.
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
