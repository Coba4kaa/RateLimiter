CREATE OR REPLACE FUNCTION get_user_by_id(user_id INTEGER)
    RETURNS TABLE(id INTEGER, login VARCHAR, password VARCHAR, name VARCHAR, surname VARCHAR, age INTEGER) AS $$
DECLARE
    user_record RECORD;
BEGIN
    SELECT Users.id, Users.login, Users.password, Users.name, Users.surname, Users.age INTO user_record
    FROM Users
    WHERE Users.id = user_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Пользователь с id % не найден', user_id;
    END IF;

    id := user_record.id;
    login := user_record.login;
    password := user_record.password;
    name := user_record.name;
    surname := user_record.surname;
    age := user_record.age;

    RETURN NEXT;

END;
$$ LANGUAGE plpgsql;
