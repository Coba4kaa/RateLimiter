CREATE OR REPLACE FUNCTION get_user_by_name(_name VARCHAR, _surname VARCHAR)
    RETURNS TABLE(id INT, login VARCHAR, password VARCHAR, name VARCHAR, surname VARCHAR, age INT) AS $$
DECLARE
    user_record RECORD;
BEGIN
    FOR user_record IN
        SELECT Users.id, Users.login, Users.password, Users.name, Users.surname, Users.age
        FROM Users
        WHERE Users.name = _name AND Users.surname = _surname
        LOOP
            id := user_record.id;
            login := user_record.login;
            password := user_record.password;
            name := user_record.name;
            surname := user_record.surname;
            age := user_record.age;
            RETURN NEXT;
        END LOOP;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Пользователь с именем % и фамилией % не найден', _name, _surname;
    END IF;
END;
$$ LANGUAGE plpgsql;
