SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL';

CREATE SCHEMA IF NOT EXISTS `WPFMESS` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci ;

-- -----------------------------------------------------
-- Usuário
-- -----------------------------------------------------
DROP TABLE IF EXISTS `WPFMESS`.`USUARIO` ;

CREATE  TABLE IF NOT EXISTS `WPFMESS`.`USUARIO` (
  `cd_usuario` INT NOT NULL AUTO_INCREMENT,
  `ds_login` VARCHAR(50) ,
  `nm_usuario` VARCHAR(50) ,
  `ds_pwhash` VARCHAR(50) ,
  `dt_validade` DATE ,
  `nr_prazoAlerta` INT ,
  `fl_bloqueada` CHAR ,
  `dt_liberacaoBloqueio` DATE ,
  `ds_configMbID` VARCHAR(50) ,
  PRIMARY KEY (`cd_usuario`) )
ENGINE = InnoDB;

DROP TABLE IF EXISTS `WPFMESS`.`CONTATO` ;

CREATE TABLE IF NOT EXISTS `WPFMESS`.`CONTATO` (
  `cd_usuario` INT NOT NULL,
  `cd_contato` INT NOT NULL,
  PRIMARY KEY (`cd_usuario`, `cd_contato`),
  CONSTRAINT `fk_contato_usuario`
    FOREIGN KEY (`cd_usuario`)
    REFERENCES `WPFMESS`.`USUARIO` (`cd_usuario`),
  CONSTRAINT `fk_contato_contato`
    FOREIGN KEY (`cd_contato`)
    REFERENCES `WPFMESS`.`USUARIO` (`cd_usuario`),
)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Recursos, operações e permissões
-- -----------------------------------------------------

DROP TABLE IF EXISTS `WPFMESS`.`RECURSO` ;

CREATE TABLE IF NOT EXISTS `WPFMESS`.`RECURSO` (
  `cd_recurso` INT NOT NULL AUTO_INCREMENT,
  `ds_recurso` VARCHAR(50),
  PRIMARY KEY (`cd_recurso`)
)
ENGINE = InnoDB;

DROP TABLE IF EXISTS `WPFMESS`.`OPERACAO` ;

CREATE TABLE IF NOT EXISTS `WPFMESS`.`OPERACAO` (
  `cd_operacao` INT NOT NULL AUTO_INCREMENT,
  `ds_operacao` VARCHAR(50),
  `cd_recurso` INT NOT NULL,
  PRIMARY KEY (`cd_operacao`),
  CONSTRAINT `fk_operacao_recurso`
    FOREIGN KEY (`cd_recurso`)
    REFERENCES `WPFMESS`.`RECURSO` (`cd_recurso`)
)
ENGINE = InnoDB;

DROP TABLE IF EXISTS `WPFMESS`.`PERMISSAO` ;

CREATE TABLE IF NOT EXISTS `WPFMESS`.`PERMISSAO` (
  `cd_usuario` INT NOT NULL,
  `cd_operacao` INT NOT NULL,
  PRIMARY KEY (`cd_usuario`, `cd_operacao`),
  CONSTRAINT `fk_permissao_usuario`
    FOREIGN KEY (`cd_usuario`)
    REFERENCES `WPFMESS`.`USUARIO` (`cd_usuario`),
  CONSTRAINT `fk_permissao_operacao`
    FOREIGN KEY (`cd_operacao`)
    REFERENCES `WPFMESS`.`OPERACAO` (`cd_operacao`)
)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Auditoria
-- -----------------------------------------------------

DROP TABLE IF EXISTS `WPFMESS`.`AUDITORIA` ;

CREATE TABLE IF NOT EXISTS `WPFMESS`.`AUDITORIA` (
  `dt_auditoria` DATETIME ,
  `ds_auditoria` TEXT NOT NULL
)
ENGINE = InnoDB;

-- !!!!!!!!!!!!!!!!!!!!!!!!!
-- INSERTS Usuario
-- !!!!!!!!!!!!!!!!!!!!!!!!!

INSERT INTO usuario (ds_login, nm_usuario,ds_pwhash,dt_validade,nr_prazoAlerta,fl_bloqueada,dt_liberacaoBloqueio, ds_configMbID)
VALUES ('admin', 'Administrador', '21232F297A57A5A743894A0E4A801FC3', NULL, 0, FALSE, NULL, NULL);

INSERT INTO usuario (ds_login, nm_usuario,ds_pwhash,dt_validade,nr_prazoAlerta,fl_bloqueada,dt_liberacaoBloqueio, ds_configMbID)
VALUES ('moore', 'Alan Moore', 'a595aab986c3206e1a637f4f18d41f62', NULL, 0, FALSE, NULL, NULL);

INSERT INTO usuario (ds_login, nm_usuario,ds_pwhash,dt_validade,nr_prazoAlerta,fl_bloqueada,dt_liberacaoBloqueio, ds_configMbID)
VALUES ('blair', 'Bruxa de Blair', '695d3555929f09cca1f9cc2295df8ca2', NULL, 0, FALSE, NULL, NULL);

INSERT INTO usuario (ds_login, nm_usuario,ds_pwhash,dt_validade,nr_prazoAlerta,fl_bloqueada,dt_liberacaoBloqueio, ds_configMbID)
VALUES ('adams', 'Família Adams', '3cc4a9a458d45578ecd7bbab6ec2aee5', NULL, 0, FALSE, NULL, NULL);

INSERT INTO usuario (ds_login, nm_usuario,ds_pwhash,dt_validade,nr_prazoAlerta,fl_bloqueada,dt_liberacaoBloqueio, ds_configMbID)
VALUES ('anna', 'Anna Hickmann', 'a70f9e38ff015afaa9ab0aacabee2e13', NULL, 0, FALSE, NULL, NULL);

-- !!!!!!!!!!!!!!!!!!!!!!!!!
-- INSERTS Contatos
-- !!!!!!!!!!!!!!!!!!!!!!!!!

-- Contatos ADMIN (fala com todos)
INSERT INTO contato (cd_usuario, cd_contato) VALUES (1,2);
INSERT INTO contato (cd_usuario, cd_contato) VALUES (1,3);
INSERT INTO contato (cd_usuario, cd_contato) VALUES (1,4);
INSERT INTO contato (cd_usuario, cd_contato) VALUES (1,5);

-- Contatos MOORE (fala com admin e blair)
INSERT INTO contato (cd_usuario, cd_contato) VALUES (2,1);
INSERT INTO contato (cd_usuario, cd_contato) VALUES (2,3);

-- Contatos BLAIR (fala com admin e moore)
INSERT INTO contato (cd_usuario, cd_contato) VALUES (3,1);
INSERT INTO contato (cd_usuario, cd_contato) VALUES (3,2);

-- Contatos ADAMS (fala com admin e anna)
INSERT INTO contato (cd_usuario, cd_contato) VALUES (4,1);
INSERT INTO contato (cd_usuario, cd_contato) VALUES (4,5);

-- Contatos ANNA (fala com admin e admas)
INSERT INTO contato (cd_usuario, cd_contato) VALUES (5,1);
INSERT INTO contato (cd_usuario, cd_contato) VALUES (5,4);

-- !!!!!!!!!!!!!!!!!!!!!!!!!
-- INSERTS Permissões
-- !!!!!!!!!!!!!!!!!!!!!!!!!

INSERT INTO `WPFMESS`.`RECURSO` VALUES (1, 'main');
INSERT INTO `WPFMESS`.`RECURSO` VALUES (2, 'chat');
INSERT INTO `WPFMESS`.`RECURSO` VALUES (3, 'contactlist');

INSERT INTO `WPFMESS`.`OPERACAO` VALUES (1, 'changeprop', 1);
INSERT INTO `WPFMESS`.`OPERACAO` VALUES (2, 'regusers', 1);
INSERT INTO `WPFMESS`.`OPERACAO` VALUES (3, 'auditor', 1);
INSERT INTO `WPFMESS`.`OPERACAO` VALUES (4, 'sendmsg', 2);
INSERT INTO `WPFMESS`.`OPERACAO` VALUES (5, 'recmsg', 2);
INSERT INTO `WPFMESS`.`OPERACAO` VALUES (6, 'sendemoticons', 2);
INSERT INTO `WPFMESS`.`OPERACAO` VALUES (7, 'recemoticons', 2);
INSERT INTO `WPFMESS`.`OPERACAO` VALUES (8, 'sendmsgoffuser', 3);

-- ADMIN
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (1, 1);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (1, 2);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (1, 3);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (1, 4);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (1, 5);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (1, 6);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (1, 7);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (1, 8);

-- MOORE (faz tudo)
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (2, 4);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (2, 5);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (2, 6);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (2, 7);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (2, 8);

-- BLAIR (não recebe emoticons)
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (3, 4);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (3, 5);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (3, 6);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (3, 7);

-- ADAMS (não envia emoticons)
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (4, 4);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (4, 5);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (4, 6);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (4, 8);

-- ANNA( não recebe msg)
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (5, 4);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (5, 5);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (5, 7);
INSERT INTO `WPFMESS`.`PERMISSAO` VALUES (5, 8);



SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;