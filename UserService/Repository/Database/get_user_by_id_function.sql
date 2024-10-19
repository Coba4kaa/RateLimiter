CREATE OR REPLACE FUNCTION get_user_by_id(user_id INTEGER)
    RETURNS TABLE(id INTEGER, login VARCHAR, password VARCHAR, name VARCHAR, surname VARCHAR, age INTEGER) AS $$
BEGIN
    SELECT Users.id, Users.login, Users.password, Users.name, Users.surname, Users.age
    INTO id, login, password, name, surname, age
    FROM Users
    WHERE Users.id = user_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Пользователь с id % не найден', user_id;
    END IF;

    RETURN NEXT;
END;
$$ LANGUAGE plpgsql;