CREATE OR REPLACE FUNCTION get_user_by_id(user_id INTEGER)
    RETURNS TABLE(id INTEGER, login VARCHAR, password VARCHAR, name VARCHAR, surname VARCHAR, age INTEGER) AS $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE Users.id = user_id) THEN
        RAISE EXCEPTION 'Пользователь с id % не найден', user_id;
    END IF;

    RETURN QUERY
        SELECT Users.id, Users.login, Users.password, Users.name, Users.surname, Users.age
        FROM Users
        WHERE Users.id = user_id;
END;
$$ LANGUAGE plpgsql;
