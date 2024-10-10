CREATE OR REPLACE PROCEDURE delete_user(_id INT)
    LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM Users WHERE id = _id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Пользователь с ID % не найден', _id;
    END IF;
END;
$$;
