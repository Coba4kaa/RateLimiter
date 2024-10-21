CREATE OR REPLACE PROCEDURE create_user(
    _login VARCHAR,
    _password VARCHAR,
    _name VARCHAR,
    _surname VARCHAR,
    _age INT
)
    LANGUAGE plpgsql
AS $$
BEGIN
    IF EXISTS (SELECT 1 FROM Users WHERE login = _login) THEN
        RAISE EXCEPTION 'Логин "%" уже существует', _login;
    END IF;

    INSERT INTO Users (login, password, name, surname, age)
    VALUES (_login, _password, _name, _surname, _age);
END;
$$;
