CREATE OR REPLACE PROCEDURE update_user(
    _id INT,
    _password VARCHAR,
    _name VARCHAR,
    _surname VARCHAR,
    _age INT
)
    LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE Users
    SET
        password = _password,
        name = _name,
        surname = _surname,
        age = _age
    WHERE id = _id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Пользователь с ID % не найден', _id;
    END IF;
END;
$$;
