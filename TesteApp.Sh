#!/bin/bash

# Verifica se o número correto de argumentos foi fornecido
if [ "$#" -ne 2 ]; then
    echo "Uso: $0 <caminho_do_binario> <vezes>"
    exit 1
fi

# Atribui os argumentos a variáveis
BINARIO=$1
VEZES=$2

# Verifica se o binário existe e é executável
if [ ! -x "$BINARIO" ]; then
    echo "Erro: O binário '$BINARIO' não existe ou não é executável."
    exit 1
fi

# Verifica se o número de vezes é um número válido
if ! [[ "$VEZES" =~ ^[0-9]+$ ]]; then
    echo "Erro: O número de vezes deve ser um valor inteiro positivo."
    exit 1
fi

# Loop para executar o binário X vezes
for (( i=1; i<=$VEZES; i++ ))
do
    echo "Executando o binário '$BINARIO' (execução $i de $VEZES)..."
    "$BINARIO" &

    # Verifica se o binário foi executado com sucesso
    if [ $? -ne 0 ]; then
        echo "Erro: Falha ao executar o binário na execução $i."
        exit 1
    fi
done

echo "Binário executado $VEZES vezes com sucesso (cada execução como uma nova instância)."