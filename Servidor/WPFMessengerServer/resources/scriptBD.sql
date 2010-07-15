SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL';

CREATE SCHEMA IF NOT EXISTS `WPFMESS` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci ;

-- -----------------------------------------------------
-- Table `WPFMESS`.`USUARIO`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `WPFMESS`.`USUARIO` ;

CREATE  TABLE IF NOT EXISTS `WPFMESS`.`USUARIO` (
  `cd_usuario` INT NOT NULL AUTO_INCREMENT,
  `nm_usuario` VARCHAR(50) ,
  `ds_pwhash` VARCHAR(50) ,
  `dt_validade` DATE ,
  `nr_prazoAlerta` INT ,
  `fl_bloqueada` CHAR ,
  `dt_liberacaoBloqueio` DATE ,
  PRIMARY KEY (`cd_usuario`) )
ENGINE = InnoDB;

DROP TABLE IF EXISTS `WPFMESS`.`MENSAGEMOFF` ;

CREATE TABLE IF NOT EXISTS `WPFMESS`.`MENSAGEMOFF` (
  `cd_origem` INT NOT NULL ,
  `cd_destino` INT NOT NULL ,
  `ds_mensagem` TEXT ,
  CONSTRAINT `fk_msg_origem`
    FOREIGN KEY (`cd_origem` )
    REFERENCES `WPFMESS`.`USUARIO` (`cd_usuario` ),
  CONSTRAINT `fk_msg_destino`
    FOREIGN KEY (`cd_destino` )
    REFERENCES `WPFMESS`.`USUARIO` (`cd_usuario` )
)
ENGINE = InnoDB;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
