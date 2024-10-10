CREATE OR REPLACE FUNCTION get_user_by_name(_name VARCHAR, _surname VARCHAR)
    RETURNS TABLE(id INT, login VARCHAR, password VARCHAR, name VARCHAR, surname VARCHAR, age INT) AS $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE Users.name = _name AND Users.surname = _surname) THEN
        RAISE EXCEPTION 'Пользователь с именем % и фамилией % не найден', _name, _surname;
    END IF;

    RETURN QUERY
        SELECT Users.id, Users.login, Users.password, Users.name, Users.surname, Users.age
        FROM Users
        WHERE Users.name = _name AND Users.surname = _surname;
END;
$$ LANGUAGE plpgsql;
