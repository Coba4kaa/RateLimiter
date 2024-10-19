CREATE OR REPLACE FUNCTION get_user_by_name(_name VARCHAR, _surname VARCHAR)
    RETURNS TABLE(id INT, login VARCHAR, password VARCHAR, name VARCHAR, surname VARCHAR, age INT) AS $$
SELECT Users.id, Users.login, Users.password, Users.name, Users.surname, Users.age
FROM Users
WHERE Users.name = _name AND Users.surname = _surname
$$ LANGUAGE sql;