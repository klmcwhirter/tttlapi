#!/bin/bash
#*----------------------------------------------------------------------------*
#* Name: play_N_games.sh
#* Description: plays NGAMES tic tac toe games given the configured strategies.
#*
#* With NGAMES=10000 the following were measured:
#*----------------------------------------------------------------------------*
# Strategies        X Wins  X %     O Wins  O %     Ties    Ties %
# Rules - Random    9744    97.44%  0       0%      256     2.56%
# Random - Rules    215     2.15%   8287    82.9%   1498    15%
# Random - Random   5854    58.54%  2899    28.99%  1247    12.47%
# Rules - Rules     1079    10.79%  0       0%      8921    89.21%
#*----------------------------------------------------------------------------*
NGAMES=10000

#            0       1          2         3        4
STRATEGIES=('Human' 'Learning' 'Minimax' 'Random' 'Rules')

GAMESTARTURL='http://tictactoelearning-dev1/api/v1/games/start'

#*----------------------------------------------------------------------------*
#* Variables

#*----------------------------------------------------------------------------*
function get_player
{
    num=$1
    name=$2
    strategy=$3

    printf '"number":"%s","name":"%s","strategy":"%s"' ${num} ${name} ${strategy}
}
#*----------------------------------------------------------------------------*
function get_players
{
    playerX=$(get_player 0 "X" ${STRATEGIES[4]})
    playerO=$(get_player 1 "O" ${STRATEGIES[4]})

    printf '[{%s},{%s}]' "${playerX}" "${playerO}"
}
#*----------------------------------------------------------------------------*
function start_game
{
    players="$1"

    curl -s -X POST "${GAMESTARTURL}" -H  "accept: text/plain" -H  "Content-Type: application/json-patch+json" -d "${players}" >/dev/null
}
#*----------------------------------------------------------------------------*
#*  M  A  I  N    P  R  O  G  R  A  M
#*----------------------------------------------------------------------------*

i=0
while [ ${i} -lt ${NGAMES} ]
do
    if [ $((${i} % 1000)) -eq 0 ]
    then
        echo Game ${i}
    fi

    players=$(get_players)
    start_game "${players}"

    let i=i+1
done

#*----------------------------------------------------------------------------*
