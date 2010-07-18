SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL';

CREATE SCHEMA IF NOT EXISTS `WPFMESS` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci ;

-- -----------------------------------------------------
-- Table `WPFMESS`.`USUARIO`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `WPFMESS`.`PERMISSAO`;
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
  PRIMARY KEY (`cd_usuario`) )
ENGINE = InnoDB;

DROP TABLE IF EXISTS `WPFMESS`.`MENSAGEMOFF` ;

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

DROP TABLE IF EXISTS `WPFMESS`.`AUDITORIA` ;

CREATE TABLE IF NOT EXISTS `WPFMESS`.`AUDITORIA` (
  `cd_usuario` INT NOT NULL,
  `dt_auditoria` DATE ,
  `ds_auditoria` TEXT NOT NULL,
  CONSTRAINT `fk_auditoria_usuario`,
    FOREIGN KEY (`cd_usuario`)
    REFERENCES `WPFMESS`.`USUARIO` (`cd_usuario`)
)
ENGINE = InnoDB;

INSERT INTO usuario (ds_login, nm_usuario,ds_pwhash,dt_validade,nr_prazoAlerta,fl_bloqueada,dt_liberacaoBloqueio)
VALUES ('admin', 'Administrador', '21232F297A57A5A743894A0E4A801FC3', NULL, 0, FALSE, NULL);

INSERT INTO usuario (ds_login, nm_usuario,ds_pwhash,dt_validade,nr_prazoAlerta,fl_bloqueada,dt_liberacaoBloqueio)
VALUES ('moore', 'Alan Moore', 'a595aab986c3206e1a637f4f18d41f62', NULL, 0, FALSE, NULL);

INSERT INTO usuario (ds_login, nm_usuario,ds_pwhash,dt_validade,nr_prazoAlerta,fl_bloqueada,dt_liberacaoBloqueio)
VALUES ('blair', 'Bruxa de Blair', '695d3555929f09cca1f9cc2295df8ca2', NULL, 0, FALSE, NULL);

INSERT INTO usuario (ds_login, nm_usuario,ds_pwhash,dt_validade,nr_prazoAlerta,fl_bloqueada,dt_liberacaoBloqueio)
VALUES ('adams', 'Família Adams', '3cc4a9a458d45578ecd7bbab6ec2aee5', NULL, 0, FALSE, NULL);

INSERT INTO usuario (ds_login, nm_usuario,ds_pwhash,dt_validade,nr_prazoAlerta,fl_bloqueada,dt_liberacaoBloqueio)
VALUES ('anna', 'Anna Hickmann', 'a70f9e38ff015afaa9ab0aacabee2e13', NULL, 0, FALSE, NULL);

INSERT INTO `WPFMESS`.`RECURSO` VALUES (1, 'main');
INSERT INTO `WPFMESS`.`RECURSO` VALUES (2, 'chat');

INSERT INTO `WPFMESS`.`OPERACAO` VALUES (1, 'changeprop', 1);
INSERT INTO `WPFMESS`.`OPERACAO` VALUES (2, 'regusers', 1);
INSERT INTO `WPFMESS`.`OPERACAO` VALUES (3, 'auditor', 1);
INSERT INTO `WPFMESS`.`OPERACAO` VALUES (4, 'sendmsg', 2);
INSERT INTO `WPFMESS`.`OPERACAO` VALUES (5, 'sendmsgoffuser', 2);
INSERT INTO `WPFMESS`.`OPERACAO` VALUES (6, 'recmsg', 2);
INSERT INTO `WPFMESS`.`OPERACAO` VALUES (7, 'sendemoticons', 2);
INSERT INTO `WPFMESS`.`OPERACAO` VALUES (8, 'recemoticons', 2);

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