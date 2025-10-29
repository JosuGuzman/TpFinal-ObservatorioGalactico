### Desplegar BD

1. Entrar con el terminal al directorio `/scripts sql`.
1. Tipear `mysql --default-character-set=utf8mb4 -u SuUsuarioSQL -p`
1. Luego ingresar la contraseña de tu usuario.
1. Ejecutar el comando:
    ```bash
        mysql> SOURCE Install.sql
    ```