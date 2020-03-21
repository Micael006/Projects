#!/bin/bash
# Проверить было ли передано два параметра 
if [[ $# -ne 2 ]]
then
    # Если передан знак вопроса вызываем помощь
    if [[ `echo "$1"` == "?" ]]
    then  
    echo "Usage: <suffix>"
    echo "       <username>"
    fi
    echo "Please, enter the suffix and your username below:"
    # Ввод данных
    read suffix
    read username
else
    suffix=$1
    username=$2
fi

if [ `echo ${suffix} | cut -c 1` == "." ]
then
    echo "Please, remove the dot at the beginning of suffix and write down it below again:"
    read suffix
fi
 
if [ ! -d "./synonyms" ]; then
    # Создать папку для ссылок (синонимов), только если ее не было
    mkdir synonyms
fi
 
##suffix=$1
##username=$2
 
# С помощью данной команды создастся файл list с построчно разделенными именами
# файлов, имеющих больше 1 связей и с наименованием, оканчивающимся на ".$suffix"
ls -l | grep -v "1 ${username}" | find *.${suffix} > list
 
# С помощью данного цикла переменная filename принимает поочередно
# все имена из полученного ранее файла list
for filename in $(cat list)
do
    # Промежуточная переменная resname отлична от filename
    # лишь тем, что не содержит ".$suffix" на конце.
    # Процедура удаления суффикса выполняется при помощи утилиты sed, заменяя
    # вхождение ".$suffix" в названии файла на "".
    resname=`echo $filename | sed "s/.$suffix//g"`
    # Создание ссылки, синонима файла filename
    # путем переноса в начало resname "$suffix"
    ln "$filename" "./synonyms/${suffix}$resname"
done
