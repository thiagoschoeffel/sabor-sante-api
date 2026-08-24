CREATE TABLE clientes_enderecos
(
    id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    cliente_id INTEGER NOT NULL,
    identificacao VARCHAR(50) NOT NULL,
    logradouro VARCHAR(150) NOT NULL,
    numero VARCHAR(20) NOT NULL,
    complemento VARCHAR(100),
    bairro VARCHAR(100) NOT NULL,
    cidade VARCHAR(100) NOT NULL,
    cep VARCHAR(20),
    ativo BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_clientes_enderecos_cliente
        FOREIGN KEY (cliente_id)
        REFERENCES clientes (id)
);