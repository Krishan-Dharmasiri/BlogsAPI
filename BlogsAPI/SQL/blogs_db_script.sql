create schema blogs;

use blogs;

CREATE TABLE `blogs` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `title` varchar(1000) NOT NULL,
  `content` varchar(10000) not null,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `spBlogsGetAll`()
BEGIN
	select title, content 
    from blogs;
END$$
DELIMITER ;

insert into blogs(title,content) values ('test 1','test blog one');
insert into blogs(title,content) values ('test 1','test blog one');

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `spBlogsInsert`(
	title varchar(1000),
    content varchar(10000)
)
BEGIN
	start transaction;
		insert into blogs (title,content)
        values (title,content);
        
    if last_insert_id() > 0 then
		select last_insert_id();
		commit;
    else
		rollback;
    end if;    
END$$
DELIMITER ;


