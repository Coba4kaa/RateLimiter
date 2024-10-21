CREATE OR REPLACE FUNCTION get_user_by_id(user_id INTEGER)
    RETURNS TABLE(id INT, login VARCHAR, password VARCHAR, name VARCHAR, surname VARCHAR, age INT) AS $$
SELECT Users.id, Users.login, Users.password, Users.name, Users.surname, Users.age
FROM Users
WHERE Users.id = user_id
$$ LANGUAGE sql;